using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�ړ�")]
    public float speed = 5f; //�ړ����x
    private Rigidbody rb;    //���W�b�h�{�f�B
    private Vector3 inputMove = Vector3.zero; // Update�œǂݎ��������

    [Header("��]")]
    public float rotationSpeed = 10f; //��]���x
    private Vector3 lastMoveDir = Vector3.zero; //�Ō�ɓ��͂�������������ێ�

    [Header("�ߊl")]
    public float captureRange = 3f; //�ߊl�͈�
    public float captureCT = 5f;    //�ߊl����̃N�[���^�C��
    public float CTCount = 0f;      //�o�ߎ���

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody����]���Ȃ��悤�ɌŒ�i�L�������]����Ȃ��悤�ɂ���j
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        //�ړ�����
        Move();
    }

    private void Update()
    {
        // ���͂� Update �Ŏ擾
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Debug.Log($"Input Check -> h: {h:F2}, v: {v:F2}");

        inputMove = new Vector3(h, 0f, v).normalized;

        // ���͂��������� lastMoveDir ���X�V
        if (inputMove.magnitude > 0.1f)
        {
            lastMoveDir = inputMove;
        }

        // �����̍X�V�i��~���ł��Ō�̕������ێ��j
        if (lastMoveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        //�N�[���^�C�����I����Ă��Ȃ���΁A����
        if (CTCount > 0.0f)
        {
            CTCount -= Time.deltaTime;
        }
        //�N�[���^�C�����I����Ă���΁A�X�y�[�X�L�[�ŕߊl
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Capture();
            }
        }
    }

    /// <summary>
    /// �ړ������iFixedUpdate�ŌĂяo���j
    /// </summary>
    private void Move()
    {
        Vector3 worldMove = inputMove * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + worldMove);
    }

    /// <summary>
    /// �͈͓��̋`����ߊl
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
            // �߂܂����`�����폜�i������ OnCaptured �Ăяo���ɒu���\��j
            Destroy(nearestTarget.gameObject);
            Debug.Log("�ł��߂��G��ߊl���܂����I");
        }
        else
        {
            Debug.Log("�ߊl�͈͓��ɓG�͂��܂���ł����B");
        }

        // �N�[���^�C���J�n
        CTCount = captureCT;
    }

    // Scene�r���[�Ŕ͈͂�������悤��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, captureRange);
    }
}
