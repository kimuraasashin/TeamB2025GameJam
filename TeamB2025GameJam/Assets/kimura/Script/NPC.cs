using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float speed = 3.5f;
    public float detectDistance = 2.0f;
    public float stopDistance = 1.0f;

    [Header("���ʉ�")]
    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip itemGetClip;

    [Header("�Q��")]
    public Transform goal;
    public Text UI;

    private NavMeshAgent agent;
    private Transform target;
    private GameManager gameManager;

    // �A�j���[�^�[�p�����[�^�̃n�b�V�����i���S�������j
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

        // �ŏ��̃^�[�Q�b�g�� Treasure �ɐݒ�
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
            // Treasure���擾
            animator.SetTrigger(Steal);
            Possession++;
            Destroy(other.gameObject);

            audioSource.PlayOneShot(itemGetClip);
            SetTarget(goal);
        }

        if (other.CompareTag("Goal"))
        {
            gameManager.EnemyGoal();
            Destroy(gameObject, 1.5f); // �����x�点�ď���
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
