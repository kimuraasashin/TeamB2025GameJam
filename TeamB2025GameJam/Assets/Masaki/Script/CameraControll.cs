using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    // 例: (0, 15, 0) とすれば真上15mの位置から見下ろします
    public Vector3 offset = new Vector3(0f, 15f, 0f);

    private void LateUpdate()
    {
        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else return; // まだいないので処理しない
        }

        if (playerTransform == null) return;

        // プレイヤーの上に配置
        transform.position = playerTransform.position + offset;

        // プレイヤーを向く（上下完全に見下ろすなら後述の固定回転の方が良い）
        transform.LookAt(playerTransform.position);
    }
}
