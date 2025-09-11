using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("移動")]
    public float speed = 5f;
    private Rigidbody rb;
    private Vector3 inputMove = Vector3.zero;

    [Header("回転")]
    public float rotationSpeed = 10f;
    private Vector3 lastMoveDir = Vector3.zero;

    [Header("捕獲")]
    public float captureRange = 3f;
    public float captureCT = 5f;
    public float CTCount = 0f;

    [Header("アニメーション")]
    [SerializeField]
    private Animator animator;

    // 入力判定の閾値
    private const float inputThreshold = 0.1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // animator が Inspector にセットされていなければ子から探す
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void FixedUpdate()
    {
        // 移動（物理）と回転を FixedUpdate で処理
        Move();
        ApplyRotation();
    }

    private void Update()
    {
        // 入力は Update で取得
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 正規化された入力ベクター
        inputMove = new Vector3(h, 0f, v).normalized;

        // isRunning の判定（上下左右・前後すべて対応）
        bool newRunning = inputMove.magnitude > inputThreshold;

        // animator が存在する場合のみ、変化があれば SetBool を呼ぶ
        if (animator != null)
        {
            bool current = animator.GetBool("isRunning");
            if (current != newRunning)
            {
                animator.SetBool("isRunning", newRunning);
            }
        }

        // 入力があったら lastMoveDir を更新
        if (inputMove.magnitude > inputThreshold)
        {
            lastMoveDir = inputMove;
        }

        // クールタイム減少
        if (CTCount > 0.0f)
        {
            CTCount -= Time.deltaTime;
            if (CTCount < 0f) CTCount = 0f;
        }
        else
        {
            // スペース押下で一度だけ捕獲
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Capture();
            }
        }

        //Debug.Log($"Input Check -> h: {h:F2}, v: {v:F2}, running: {newRunning}");
    }

    private void Move()
    {
        // 入力ベクトルを速度に変換
        Vector3 moveVelocity = inputMove * speed;

        // Rigidbody の速度に直接代入
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    private void ApplyRotation()
    {
        // 停止中でも lastMoveDir を向く（最後の方向を維持）
        if (lastMoveDir.magnitude > inputThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void Capture()
    {
        animator.SetTrigger("catch");
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
            Destroy(nearestTarget.gameObject);
            Debug.Log("最も近い敵を捕獲しました！");
        }
        else
        {
            Debug.Log("捕獲範囲内に敵はいませんでした。");
        }

        CTCount = captureCT;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, captureRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Field"))
        {
            Debug.Log("壁にぶつかった！");
        }
    }
}
