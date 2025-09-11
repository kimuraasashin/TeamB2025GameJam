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
    public int Possession;
    public NavMeshAgent agent;
    public Text UI;
    public Transform goal;
    public bool move = false;
    public AudioSource se;
    public AudioClip itemGet;
    public AudioClip walk;

    private Transform target;
    private float stepInterval = 0.5f;// ë´âπÇÃä‘äuÅiïbÅj
    private float stepTimer = 0f;
    //public LineRenderer lr;

    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();

        GameObject treasureObj = GameObject.FindGameObjectWithTag("Treasure");
        if (treasureObj != null)
        {
            target = treasureObj.transform;
        }

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
        if (target != null)
        {
            agent.SetDestination(target.position);
            move = true;
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
                other.gameObject.GetComponent<Treasure>().GetTime -= 1.0f * Time.deltaTime;
                if (other.gameObject.GetComponent<Treasure>().GetTime <= 0.0f)
                {
                    Possession++;
                    Destroy(other.gameObject);
                    se.PlayOneShot(itemGet);
                    target = goal;
                }
            }
        }
        if (other.gameObject.name=="goal")
        {
                Destroy(gameObject);
        }
    }
}
