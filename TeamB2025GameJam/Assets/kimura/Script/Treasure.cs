using UnityEngine;

public class Treasure : MonoBehaviour
{
    public float GetTime = 2.0f;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnDestroy()
    {
        gameManager.Stolen();
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            Destroy(gameObject);
        }
    }
    */
}
