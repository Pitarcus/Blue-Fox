using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BabyFollower : MonoBehaviour
{

    public Transform playerTransform;
    public Transform headAimTarget;
    NavMeshAgent agent;
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        headAimTarget.position = playerTransform.position;
        agent.SetDestination(playerTransform.position);

        if (agent.remainingDistance <= agent.stoppingDistance - 5f || Mathf.Approximately(agent.velocity.magnitude, 0)) 
        {
             animator.SetBool("isWalking", false);
        }
        else 
        {
            animator.SetBool("isWalking", true);
        }
    }
}
