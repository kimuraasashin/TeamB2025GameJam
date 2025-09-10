using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    /*public Vector3[] point;
    public int cp = 0;
    public float s=3f;*/
    public int Possession;
    public NavMeshAgent agent;
    private Transform target;
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
            other.gameObject.GetComponent<Treasure>().GetTime -= 1.0f * Time.deltaTime;
            if (other.gameObject.GetComponent<Treasure>().GetTime <= 0.0f)
            {
                Possession++;
                Destroy(other.gameObject);
            }
        }
    }
}
