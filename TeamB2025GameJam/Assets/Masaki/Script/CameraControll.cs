using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    // ��: (0, 15, 0) �Ƃ���ΐ^��15m�̈ʒu���猩���낵�܂�
    public Vector3 offset = new Vector3(0f, 15f, 0f);

    private void LateUpdate()
    {
        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else return; // �܂����Ȃ��̂ŏ������Ȃ�
        }

        if (playerTransform == null) return;

        // �v���C���[�̏�ɔz�u
        transform.position = playerTransform.position + offset;

        // �v���C���[�������i�㉺���S�Ɍ����낷�Ȃ��q�̌Œ��]�̕����ǂ��j
        transform.LookAt(playerTransform.position);
    }
}
