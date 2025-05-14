using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTester : MonoBehaviour
{

    [SerializeField] private Transform enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 position = enemy.position;
        position.y = 0;
        transform.LookAt(position);
    }
}
