using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    public float recoverTime = 6f;
    private float recoverTimer = 0f;

    public LayerMask playerMask;
    public EnemyWeakSpot weakSpot;

    [SerializeField]
    private NavMeshAgent agent;
    private Animator enemyAnimator;
    private GameObject player;
    private Collider weakPointCollider;

    private bool following = true;
    private bool recovering = false;
    private bool recovered = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        transform.LookAt(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            agent.SetDestination(player.transform.position);

            Debug.DrawRay(transform.position + new Vector3(0, 5f, 0), transform.forward * 10, Color.green);

            if (Physics.Raycast(transform.position + new Vector3(0, 5f, 0), transform.forward, 10f, playerMask))
            {
                Debug.Log("About to attack");
                following = false;
                recovering = true;
                enemyAnimator.SetTrigger("attack");
                recoverTimer = 0f;
            }
        }
        else if (recovering) 
        {
            agent.isStopped = true;
            recoverTimer += Time.deltaTime;

            // Set vulnerability
            weakSpot.SetVulnerable();

            if (recoverTimer >= recoverTime)
            {
                Recover();
            }
        }
    }

    public void Recover()
    {
        recoverTimer = 0f;
        following = true;
        recovering = false;
        agent.isStopped = false;

        weakSpot.SetInvulnerable();
    }
}
