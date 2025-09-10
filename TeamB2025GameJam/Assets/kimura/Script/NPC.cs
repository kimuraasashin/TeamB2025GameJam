using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    /*public Vector3[] point;
    public int cp = 0;
    public float s=3f;*/
    public int Possession;
    public NavMeshAgent agent;
    public Text UI;
    public Transform goal;
    private Transform target;
    public bool move = false; 
    //public LineRenderer lr;

    void Start()
    {
        UI.text = " ";

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
