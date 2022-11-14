using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshControl : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        agent.destination = player.position;
    }
}
