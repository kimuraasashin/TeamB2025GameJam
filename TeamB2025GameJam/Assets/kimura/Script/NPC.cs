using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class NPC : MonoBehaviour
{
    /*public Vector3[] point;
    public int cp = 0;
    public float s=3f;*/
    public float speed;
    public int Possession;
    public NavMeshAgent agent;
    public Text UI;
    public Transform goal;
    public bool move = false;
    public AudioSource itemGet;
    public AudioSource walk;
    public float detectDistance = 2.0f;  // �O���̏�Q�������m���鋗��
    public float stopDistance = 1.0f;    // �ړI�n�̎�O�Ŏ~�܂鋗��

    private Transform target;
    private float stepInterval = 0.5f;// �����̊Ԋu�i�b�j
    private float stepTimer = 0f;
    private GameManager gameManager;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject model;
    //public LineRenderer lr;

    void Start()
    {
        if (itemGet != null)
        {
            itemGet.Stop();
        }

        if (walk != null)
        {
            walk.Stop();
        }

        agent = GetComponent<NavMeshAgent>();

        GameObject treasureObj = GameObject.FindGameObjectWithTag("Treasure");
        if (treasureObj != null)
        {
            target = treasureObj.transform;
        }

        gameManager = FindObjectOfType<GameManager>();

        //model.SetActive(false);

        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        agent.stoppingDistance = stopDistance;

        agent.speed = speed;
        /*lr = GetComponent<LineRenderer>();
        lr.positionCount = point.Length+1;
        
        for(int i=0; i < point.Length; i++)
        {
            lr.SetPosition(i, point[i]);
        }
        lr.SetPosition(point.Length, point[0]);

        if (point.Length > 0)
        {
            transform.position = point[0];
        }*/
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // �����ォ�烌�C���΂�
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectDistance))
        {
            if (!hit.collider.CompareTag("Treasure"))
            {
                agent.isStopped = true;// ��Q��������Β�~
            }
            else
            {
                agent.isStopped = false; // ���ύX�_: Treasure�Ȃ�i�ݑ�����
            }
                Debug.DrawRay(rayOrigin, transform.forward * detectDistance, Color.red);
        }
        else
        {
            agent.isStopped = false; // ��Q�����Ȃ���΍ĊJ
            if (target != null)
            {
                agent.SetDestination(target.position);
                walk.Play();
            }
            Debug.DrawRay(rayOrigin, transform.forward * detectDistance, Color.green);
        }

        if (agent.isStopped || agent.velocity.magnitude < 0.1f)
        {
            animator.SetBool("isRunning", false); // ���ύX�_: Idle�A�j���[�V�����ɐ؂�ւ�

        }
        else
        {
            animator.SetBool("isRunning", true);  // ���ύX�_: Run�A�j���[�V�����ɐ؂�ւ�
        }

        if (animator.GetBool("isRunning"))
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
        /*if (point.Length == 0) return;

        Vector3 target = point[cp];

        transform.position = Vector3.MoveTowards(transform.position, target, s * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            cp = (cp + 1) % point.Length;
        }*/
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Treasure>())
        {
            if (target != null)
            {
                move = false;
                other.gameObject.GetComponent<Treasure>().GetTime -= 1.0f * Time.deltaTime;
                if (other.gameObject.GetComponent<Treasure>().GetTime <= 0.0f)
                {
                    itemGet.Play();
                    animator.SetBool("isRunning", false);
                    animator.SetTrigger("steal");
                    Possession++;
                    Destroy(other.gameObject);
                    target = goal;
                }
            }
        }
        if (other.gameObject.name=="goal")
        {
            walk.Stop();
            Destroy(gameObject);
            gameManager.EnemyGoal();
        }
    }

    public void OnCaptured()
    {
        //model.SetActive(true);
    }
}
