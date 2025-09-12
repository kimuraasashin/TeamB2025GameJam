using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class NPC : MonoBehaviour
{
    // ������: �o�H����p�̕ϐ�
    public Vector3[] point;
    public int cp = 0;
    public float s;

    public int Possession;
    public NavMeshAgent agent;
    public Text UI;
    public Transform goal;
    public bool move = false;
    public AudioSource se;
    public AudioClip itemGet;
    public AudioClip walk;
    public float detectDistance = 2.0f;  // �O���̏�Q�������m���鋗��
    public float stopDistance = 0.1f;    // �ړI�n�̎�O�Ŏ~�܂鋗��

    private Transform target;
    private float stepInterval = 0.5f; // �����̊Ԋu�i�b�j
    private float stepTimer = 0f;
    private GameManager gameManager;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject model;

    // ������: LineRenderer
    public LineRenderer lr;

    private bool isStealing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = s;

        GameObject treasureObj = GameObject.FindGameObjectWithTag("Treasure");
        if (treasureObj != null)
        {
            target = treasureObj.transform;
        }

        gameManager = FindObjectOfType<GameManager>();

        model.SetActive(false);

        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        agent.stoppingDistance = stopDistance;

        // ������: LineRenderer �̌o�H�\��
        if (point.Length > 0)
        {
            lr = GetComponent<LineRenderer>();
            lr.positionCount = point.Length + 1;

            for (int i = 0; i < point.Length; i++)
            {
                lr.SetPosition(i, point[i]);
            }
            lr.SetPosition(point.Length, point[0]);

            transform.position = point[0];
        }
    }

    void Update()
    {
        if (target == null)
        {
            GameObject nextTreasure = GameObject.FindGameObjectWithTag("Treasure");
            if (nextTreasure != null)
            {
                target = nextTreasure.transform; // �܂�Treasure���c���Ă���Ύ���
            }
            else
            {
                target = goal; // �c���ĂȂ���΃S�[����
            }

            // NavMeshAgent�ɐV�����ړI�n���Z�b�g
            if (target != null)
            {
                agent.SetDestination(target.position);
            }
        }

        if (target != null)
        {
            agent.SetDestination(target.position);
            move = true;
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (agent.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                se.PlayOneShot(walk);
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        // ������: ���񏈗��ipoint �����ԂɈړ��j
        if (point.Length > 0)
        {
            Vector3 targetPoint = point[cp];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, s * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
            {
                cp = (cp + 1) % point.Length; // ���̃|�C���g��
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Treasure>())
        {
            if (target != null)
            {
                if (!isStealing)
                {
                    animator.SetTrigger("steal");
                    isStealing = true;
                }
                
                other.gameObject.GetComponent<Treasure>().GetTime -= 1.0f * Time.deltaTime;
                if (other.gameObject.GetComponent<Treasure>().GetTime <= 0.0f)
                {
                    Possession++;
                    Destroy(other.gameObject);
                    se.PlayOneShot(itemGet);
                    target = null;
                    isStealing = false;
                }
            }
        }
        if (other.gameObject.name == "goal")
        {
            Destroy(gameObject);
            gameManager.EnemyGoal();
        }
    }

    public void OnCaptured()
    {
        model.SetActive(true);
    }
}
