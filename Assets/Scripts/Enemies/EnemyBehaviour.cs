using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    [Header("Assign in Editor")]
    public ParticleSystem smoke;
    public ParticleSystem feathers;
    public LayerMask playerMask;
    public EnemyWeakSpot weakSpot;
    public MistReveal mistObject;

    [Header("Enemy Parameters")]
    public float recoverTime = 5f;
    private float recoverTimer = 0f;

    private MeleeEnemyFX fx;
    public int health = 3;

    [SerializeField]
    private NavMeshAgent agent;
    private Animator enemyAnimator;
    private GameObject player;

    [HideInInspector] public bool dead = false;

    private bool following = true;
    private bool recovering = false;
    private bool vulnerable = false;

    // Player death stuff
    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        transform.LookAt(player.transform.position);
        fx = GetComponent<MeleeEnemyFX>();
    }
    private void OnEnable()
    {
        weakSpot.onWeakSpotHit.AddListener(EnemyHit);
    }

    public void Reset()
    {
        enemyAnimator.ResetTrigger("attack");
        enemyAnimator.ResetTrigger("recharge");

        transform.position = originalPosition;
        gameObject.SetActive(false);

        fx.StopAll();

        health = 3;
        recoverTimer = 0f;
        following = true;
        recovering = false;
    }

    private void OnDisable()
    {
        weakSpot.onWeakSpotHit.RemoveListener(EnemyHit);
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            vulnerable = false;
            agent.SetDestination(player.transform.position);

            Debug.DrawRay(transform.position + new Vector3(0, 5f, 0), transform.forward * 10, Color.green);

            if (Physics.Raycast(transform.position + new Vector3(0, 5f, 0), transform.forward, 10f, playerMask))
            {
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
            if (!vulnerable)
            {
                weakSpot.SetVulnerable();
                vulnerable = true;
            }

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
        enemyAnimator.SetTrigger("recharge");
        fx.PlayRecovery();

        weakSpot.SetInvulnerable();
    }

    void EnemyHit() 
    {
        health--;
        if (health > 0)
        {
            Recover();
        }
        else // Enemy dead
        {
            dead = true;
            Debug.Log("Enemy says is dead: " + dead);

            feathers.Stop();
            smoke.Stop();

            recovering = false;

            weakSpot.DestroyWeakSpot();
            fx.PlayDeath();

            enemyAnimator.SetTrigger("die");

            mistObject.HideMist();

        }
    }
}
