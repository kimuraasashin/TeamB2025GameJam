using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("移動")]
    public float speed = 5f; //移動速度
    private Rigidbody rb;    //リジッドボディ
    private Vector3 inputMove = Vector3.zero; // Updateで読み取った入力

    [Header("回転")]
    public float rotationSpeed = 10f; //回転速度
    private Vector3 lastMoveDir = Vector3.zero; //最後に入力があった方向を保持

    [Header("捕獲")]
    public float captureRange = 3f; //捕獲範囲
    public float captureCT = 5f;    //捕獲動作のクールタイム
    public float CTCount = 0f;      //経過時間

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbodyが回転しないように固定（キャラが転がらないようにする）
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        //移動処理
        Move();
    }

    private void Update()
    {
        // 入力は Update で取得
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Debug.Log($"Input Check -> h: {h:F2}, v: {v:F2}");

        inputMove = new Vector3(h, 0f, v).normalized;

        // 入力があったら lastMoveDir を更新
        if (inputMove.magnitude > 0.1f)
        {
            lastMoveDir = inputMove;
        }

        // 向きの更新（停止中でも最後の方向を維持）
        if (lastMoveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        //クールタイムが終わっていなければ、減少
        if (CTCount > 0.0f)
        {
            CTCount -= Time.deltaTime;
        }
        //クールタイムが終わっていれば、スペースキーで捕獲
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Capture();
            }
        }
    }

    /// <summary>
    /// 移動処理（FixedUpdateで呼び出し）
    /// </summary>
    private void Move()
    {
        Vector3 worldMove = inputMove * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + worldMove);
    }

    /// <summary>
    /// 範囲内の義賊を捕獲
    /// </summary>
    private void Capture()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, captureRange);

        Collider nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestTarget = hit;
                }
            }
        }

        if (nearestTarget != null)
        {
            // 捕まえた義賊を削除（将来は OnCaptured 呼び出しに置換予定）
            Destroy(nearestTarget.gameObject);
            Debug.Log("最も近い敵を捕獲しました！");
        }
        else
        {
            Debug.Log("捕獲範囲内に敵はいませんでした。");
        }

        // クールタイム開始
        CTCount = captureCT;
    }

    // Sceneビューで範囲が見えるように
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, captureRange);
    }
}
