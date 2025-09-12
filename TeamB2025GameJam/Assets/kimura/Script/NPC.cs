using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("移動設定")]
    public float speed = 3.5f;
    public float detectDistance = 2.0f;
    public float stopDistance = 1.0f;

    [Header("効果音")]
    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip itemGetClip;

    [Header("参照")]
    public Transform goal;
    public Text UI;

    private NavMeshAgent agent;
    private Transform target;
    private GameManager gameManager;

    // アニメーターパラメータのハッシュ化（安全＆高速）
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int Steal = Animator.StringToHash("steal");

    private Animator animator;
    private float stepTimer;
    private float stepInterval = 0.5f;

    public int Possession { get; private set; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        gameManager = FindObjectOfType<GameManager>();

        agent.speed = speed;
        agent.stoppingDistance = stopDistance;

        // 最初のターゲットを Treasure に設定
        GameObject treasureObj = GameObject.FindGameObjectWithTag("Treasure");
        if (treasureObj != null)
        {
            SetTarget(treasureObj.transform);
        }
    }

    void Update()
    {
        CheckObstacle();
        UpdateAnimation();
        HandleFootsteps();
    }

    private void CheckObstacle()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectDistance))
        {
            if (!hit.collider.CompareTag("Treasure"))
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
        else
        {
            agent.isStopped = false;
            if (target != null) agent.SetDestination(target.position);
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = !agent.isStopped && agent.velocity.magnitude > 0.1f;
        animator.SetBool(IsRunning, isMoving);
    }

    private void HandleFootsteps()
    {
        if (animator.GetBool(IsRunning))
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                audioSource.PlayOneShot(walkClip);
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Treasure>(out Treasure treasure))
        {
            // Treasureを取得
            animator.SetTrigger(Steal);
            Possession++;
            Destroy(other.gameObject);

            audioSource.PlayOneShot(itemGetClip);
            SetTarget(goal);
        }

        if (other.CompareTag("Goal"))
        {
            gameManager.EnemyGoal();
            Destroy(gameObject, 1.5f); // 少し遅らせて消す
        }
    }

    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}
