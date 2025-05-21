using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Info")]
    [Space(5)]
    public float m_MaxSecondsPerRound = 90;
    [Range(1,6)]
    public int m_MaxRounds = 3;

    [Header("SpawnPoints")]
    public Transform enemySpawn;
    public Transform playerSpawn;

    [Header("UI References")]
    [SerializeField] private Canvas RoundStartCanvas;
    [SerializeField] private TextMeshProUGUI roundTimerTxt;
    [SerializeField] private TextMeshProUGUI playerScoreTxt;
    [SerializeField] private TextMeshProUGUI enemyScoreTxt;

    private float currentSecondsRemaining;
    private int currentRound;

    public event Action<int> OnRoundStarted;
    public event Action<int> OnRoundEnded;

    public EnemyController enemyController;
    public GameObject XROrigin;

    public Player player;

    public int PlayerScore { get; private set; }
    public int EnemyScore { get; private set; }

    public EnemyController[] enemyControllers;
    public int EnemyIndex = 0;

    public Canvas timerCanvas;
    public Canvas selectionCanvas;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }

            return instance;
        }
    }    
    private void Start()
    {
        ResetCharacters();
        OnRoundStarted += OnRoundStart;
        OnRoundEnded += OnRoundEnd;
       ShowStartGameUI();
    }

    private void OnRoundEnd(int round)
    {
        //TODO: AudioManager.Instance.PlayOneShot("", boxingRingCenter);
        // Ring 3x Bell here.

        AudioManager.Instance.PlayOneShot("Box_End", player.transform.position);
    }

    private void OnRoundStart(int round)
    {
        //TODO: AudioManager.Instance.PlayOneShot("", boxingRingCenter);
        // Ring the bell here.

        AudioManager.Instance.PlayOneShot("Box_Begin", player.transform.position);
    }

    private IEnumerator StartGameRoutine()
    {
        // Match starts from here:

        if (enemyController != null)
        {
            Destroy(enemyController.gameObject);
        }

        enemyController = Instantiate(enemyControllers[EnemyIndex]);
        yield return new WaitForEndOfFrame();

        
        PlayerScore = 0;
        EnemyScore = 0;

        playerScoreTxt.text = "0";
        enemyScoreTxt.text = "0";
        

        // Resetting round to 0
        currentRound = 0;

        // TODO: This is bad. Cache it.
        XROrigin.GetComponent<CharacterController>().enabled = true;

        // Round logic starts from here.
        while (currentRound < m_MaxRounds)
        {
            OnRoundStarted?.Invoke(currentRound);
            currentSecondsRemaining = m_MaxSecondsPerRound;

            ResetCharacters();

            RoundStartCanvas.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            RoundStartCanvas.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            enemyController.IsReadyToAttack = true;
            // This is the only game loop. Checking if timer ran out or if someone died, if not keep continuing..
            while (currentSecondsRemaining > 0 && !IsAnyOneDead())
            {
                currentSecondsRemaining -= Time.deltaTime;
                roundTimerTxt.text = currentSecondsRemaining.ToString("0.00");
                yield return null;
            }

            OnRoundEnded?.Invoke(currentRound);
            if(!enemyController.health.IsDead)
            {
                enemyController.m_Target = enemySpawn;
                enemyController.m_ShouldAttackPlayer = false;
            }
            currentRound++;
            CalculateScore();
            roundTimerTxt.text = "Cleaning Up";
            yield return new WaitForSeconds(3);
            roundTimerTxt.text = $"Round {currentRound}";
            yield return new WaitForSeconds(5);
        }

        roundTimerTxt.text = EnemyScore > PlayerScore ? "Bobo Wins" : "You Win!";
    }

    private void ResetCharacters()
    {
        XROrigin.GetComponent<CharacterController>().enabled = false;
        XROrigin.transform.position = playerSpawn.position;
        XROrigin.transform.forward = playerSpawn.forward;

        if (enemyController != null)
        {
            enemyController.transform.position = enemySpawn.position;
            enemyController.transform.forward = enemySpawn.forward;
            enemyController.ResetGame();
        }

        player.ResetGame();
    }

    public void StartGame()
    {
        selectionCanvas.gameObject.SetActive(false);
        timerCanvas.gameObject.SetActive(true);
        StartCoroutine(StartGameRoutine());
    }

    public void ShowStartGameUI()
    {
        if (enemyController != null)
        {
            Destroy(enemyController.gameObject);
        }
        StopAllCoroutines();
        timerCanvas.gameObject.SetActive(false);
        selectionCanvas.gameObject.SetActive(true);
        selectionCanvas.GetComponent<CharacterSelection>().ResetSelection();
    }

    private void CalculateScore()
    {
        if (currentSecondsRemaining <= 0 || player.health.IsDead)
        {
            EnemyScore++;
        }
        else
        {
            PlayerScore++;
        }

        playerScoreTxt.text = PlayerScore.ToString();
        enemyScoreTxt.text = EnemyScore.ToString();
    }

    private bool IsAnyOneDead()
    {
        return player.health.IsDead || enemyController.health.IsDead;
    }
}
