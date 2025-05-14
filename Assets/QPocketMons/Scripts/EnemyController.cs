using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniVRM10;

public class EnemyController : MonoBehaviour
{
    public Transform m_Target;

    public float m_MoveSpeed = 1.0f;
    public float m_AttackSpeed = 0.3f;
    public float m_RotationSpeed = 30.0f;

    public float m_MaxAttackRadius = 0.1f;
    public float m_MinAttackRadius = 0.08f;

    public int attackDamage = 4;

    public float m_DodgeChange;

    public Animator m_Animator;

    private bool IsUnableToProcess;
    public bool IsAttacking = false;
    public bool IsRotating = false;

    public AnimatorEventsHandler animatorEventsHandler;

    public ParticleSystem[] hitEffect;

    [Header("References")]
    public Transform rightHand;
    public Transform leftHand;

    public Transform head;

    [Header("Values")]
    public float handRadius;
    public float kneeRadius;
    public float footRadius;

    private float lastAttackTime;

    public LayerMask ownerMask;

    private List<string> listOfCombos = new();

    private string currentCombo = string.Empty;
    private int currentComboIndex = 0;

    public Health health;

    public FacialExpressionAnimator facialExpressionAnimator;

    List<Collider> colliders = new List<Collider>();
    List<Rigidbody> ragDollRigidbody = new List<Rigidbody>();

    private int numberOfAttacksReceived;
    public bool m_ShouldAttackPlayer;

    private Dictionary<string, string[]> Combos = new()
    {
        { "Combo_1",new string[] { "Boxing_Left_Hook", "Boxing_Right_Hook" } },
        { "Combo_2",new string[] { "Boxing_Right_Hook", "Boxing_Right_Hook" } },
        { "Combo_3",new string[] { "Boxing_Right_Hook", "Boxing_Left_Hook" } },
        { "Combo_4",new string[] { "Boxing_Left_Hook", "Boxing_Left_Hook" } },
    };

    private void Awake()
    {
        colliders = transform.GetComponentsInChildren<Collider>().ToList();
        ragDollRigidbody = transform.GetComponentsInChildren<Rigidbody>().ToList();

        ragDollRigidbody.ForEach(r => r.useGravity = false);
    }


    private bool IsInAttackRange()
    {
        return (DistanceFromPlayer() < m_MaxAttackRadius && DistanceFromPlayer() > m_MinAttackRadius);
    }

    private float DistanceFromPlayer()
    {
        Vector3 currentPos = transform.position;
        currentPos.y = 0;

        Vector3 targetPos = m_Target.position;
        targetPos.y = 0;
        return Vector3.Distance(currentPos, targetPos);
    }

    private void Start()
    {
        animatorEventsHandler.OnAnimationEventCalled += OnAnimationEvents;

        foreach (string combo in Combos.Keys)
        {
            listOfCombos.Add(combo);
        }
    }

    private void PlayHitEffect(Vector3 position)
    {
        foreach (ParticleSystem p in hitEffect)
        {
            p.transform.position = position;
            p.Emit(1);
        }
    }

    private void OnAnimationEvents(string animationEvent)
    {
        switch (animationEvent)
        {
            case "animation_end":
                IsAttacking = false;
                currentComboIndex++;
                if (currentComboIndex >= Combos[currentCombo].Length)
                {
                    // Reset the Combo
                    currentCombo = string.Empty;
                    currentComboIndex = 0;
                }

                lastAttackTime = Time.time;


                break;
            case "Hook_Left_Hand":
                CheckForCollisionAndCreateDamage(leftHand, handRadius);
                break;
            case "Hook_Right_Hand":
                CheckForCollisionAndCreateDamage(rightHand, handRadius);
                break;
            case "hit_impact_done":
                IsUnableToProcess = false;
                IsAttacking = false;
                lastAttackTime = Time.time;
                break;
        }
    }

    private void CheckForCollisionAndCreateDamage(Transform point, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(point.position, radius, ~ownerMask);
        if (colliders.Length > 0)
        {
            if (colliders[0].transform.TryGetComponent(out Player player))
            {
                player.SetDamage(attackDamage);
                numberOfAttacksReceived = 0;
            }
        }
    }

    private void Update()
    {
        if (IsUnableToProcess || health.IsDead) return;
        if (m_Target == null) return;

        IsRotating = false;
        RotateTowards(m_Target);
        facialExpressionAnimator.SetExpression(ExpressionKey.Angry, 1);
        if (!IsInAttackRange() && !IsRotating && !IsAttacking)
        {
            HandleMovement();
            m_Animator.SetBool("Walk", true);
        }
        else
        {
            m_Animator.SetBool("Walk", false);

            if (IsInAttackRange() && !IsRotating && !IsAttacking && m_ShouldAttackPlayer)
            {
                if (string.IsNullOrEmpty(currentCombo))
                {
                    GetRandomCombo();
                }
                else
                {
                    if (Time.time > lastAttackTime + m_AttackSpeed)
                    {
                        IsAttacking = true;
                        m_Animator.Play(Combos[currentCombo][currentComboIndex]);
                    }
                }
            }
        }
    }

    private void GetRandomCombo()
    {
        int x = UnityEngine.Random.Range(0, listOfCombos.Count);
        currentCombo = listOfCombos[x];
    }

    private void HandleMovement()
    {
        if (DistanceFromPlayer() < m_MinAttackRadius)
        {
            transform.position += -1 * m_MaxAttackRadius * m_MoveSpeed * Time.deltaTime * transform.forward;
        }
        else if (DistanceFromPlayer() >= m_MaxAttackRadius)
        {
            Vector3 direction = (m_Target.position - transform.position).normalized;
            direction.y = 0;
            transform.position += m_MoveSpeed * Time.deltaTime * direction;
        }
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);
        }
    }


    public bool TriggerHit(HitType hitArea)
    {
        string animationToPlay = string.Empty;

        bool PunchConnected = IsSuccessfulHit();

        int damageDealt = 0;

        if (PunchConnected)
        {
            numberOfAttacksReceived++;
            switch (hitArea)
            {
                case HitType.Head:
                    damageDealt = 8;
                    m_Animator.Play("HeadHit", 0);
                    break;
                case HitType.Spine:
                case HitType.Hip:
                    m_Animator.Play("TorsoHit", 0);
                    damageDealt = 4;
                    break;
                case HitType.LeftKnee:
                case HitType.LeftFoot:
                case HitType.LeftElbow:
                case HitType.LeftShoulder:
                    m_Animator.Play("LeftHit", 0);
                    damageDealt = 4;
                    break;
                case HitType.RightFoot:
                case HitType.RightKnee:
                case HitType.RightShoulder:
                case HitType.RightElbow:
                    m_Animator.Play("RightHit", 0);
                    damageDealt = 4;
                    break;
            }

            health.SetDamage(damageDealt);

            if (health.IsDead)
            {
                ragDollRigidbody.ForEach(r => r.useGravity = true);
                m_Animator.enabled = false;
                head.GetComponent<Rigidbody>().AddForce(-1 * 5 * transform.forward);
            }
        }
        else
        {
            m_Animator.Play("Block");
        }
        facialExpressionAnimator.SetExpression(ExpressionKey.Surprised, 1);
        currentCombo = string.Empty;
        IsUnableToProcess = true;
        return PunchConnected;
    }

    public bool IsSuccessfulHit()
    {
        int x = UnityEngine.Random.Range(0, 100);
        return x > m_DodgeChange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_MaxAttackRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_MinAttackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rightHand.position, handRadius);
        Gizmos.DrawWireSphere(leftHand.position, handRadius);
    }
    public void ResetGame()
    {
        health.ResetHealth();
        ragDollRigidbody.ForEach(r => { r.useGravity = false;});
        m_Animator.enabled = true;
        m_ShouldAttackPlayer = true;
        m_Target = FindObjectOfType<Player>().transform;
    }
}

public enum EnemyState
{
    Chasing,
    Deciding,
    Attacking,
    Blocking
}
