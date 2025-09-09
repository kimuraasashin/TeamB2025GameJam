using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("移動")]
    public float speed = 5f; //移動速度
    private Rigidbody rb;    //リジッドボディ


    [Header("捕獲")]
    public float captureRange = 5f; //捕獲範囲
    public float captureCT = 3f;    //捕獲動作のクールタイム
    public float CTCount = 0f;      //経過時間

    void Start()
    {
        //リジッドボディを取得
        rb = GetComponent<Rigidbody>();

        // Rigidbodyが回転しないように固定（キャラが転がらないようにする）
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        //移動処理
        Move();
    }

    private void Update()
    {
        //クールタイムが終わっていなければ、クールタイムを減少
        if(CTCount >= 0.0f)
        {
            CTCount -= Time.deltaTime;
        }
        //クールタイムが終わっていれば、スペースキーで捕獲
        else
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Capture();
            }
        }
    }

    /// <summary>
    /// シンプルな移動(FixedUpdateで呼び出し)
    /// </summary>
    private void Move()
    {
        // A,Dキーで左右移動、W,Sキーで前後移動
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Debug.Log($"h={h}, v={v}");

        // マップ基準（ワールド座標）の移動ベクトル
        Vector3 move = new Vector3(h, 0, v).normalized * speed;

        // Rigidbodyを使って位置を更新（衝突判定あり）
        rb.MovePosition(transform.position + move * Time.fixedDeltaTime);
    }

    /// <summary>
    /// 範囲内の義賊を捕獲(スペースキーで動作)
    /// </summary>
    private void Capture()
    {
        // 範囲内のColliderを取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, captureRange);

        Collider nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hitColliders)
        {
            // "Enemy"タグのついたオブジェクトが対象
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);

                // より近い対象が見つかったら更新
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestTarget = hit;
                }
            }
        }

        // 一番近い対象が見つかったら捕獲処理
        if (nearestTarget != null)
        {
            //捕まえた義賊を削除(実装が完了し次第、義賊のOnCapturedに変更)
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
}
