using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�ړ�")]
    public float speed = 5f;
    private Rigidbody rb;
    private Vector3 inputMove = Vector3.zero;

    [Header("��]")]
    public float rotationSpeed = 10f;
    private Vector3 lastMoveDir = Vector3.zero;

    [Header("�ߊl")]
    public float captureRange = 3f;
    public float captureCT = 5f;
    public float CTCount = 0f;

    [Header("�A�j���[�V����")]
    [SerializeField]
    private Animator animator;

    // ���͔����臒l
    private const float inputThreshold = 0.1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // animator �� Inspector �ɃZ�b�g����Ă��Ȃ���Ύq����T��
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void FixedUpdate()
    {
        // �ړ��i�����j�Ɖ�]�� FixedUpdate �ŏ���
        Move();
        ApplyRotation();
    }

    private void Update()
    {
        // ���͂� Update �Ŏ擾
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // ���K�����ꂽ���̓x�N�^�[
        inputMove = new Vector3(h, 0f, v).normalized;

        // isRunning �̔���i�㉺���E�E�O�シ�ׂđΉ��j
        bool newRunning = inputMove.magnitude > inputThreshold;

        // animator �����݂���ꍇ�̂݁A�ω�������� SetBool ���Ă�
        if (animator != null)
        {
            bool current = animator.GetBool("isRunning");
            if (current != newRunning)
            {
                animator.SetBool("isRunning", newRunning);
            }
        }

        // ���͂��������� lastMoveDir ���X�V
        if (inputMove.magnitude > inputThreshold)
        {
            lastMoveDir = inputMove;
        }

        // �N�[���^�C������
        if (CTCount > 0.0f)
        {
            CTCount -= Time.deltaTime;
            if (CTCount < 0f) CTCount = 0f;
        }
        else
        {
            // �X�y�[�X�����ň�x�����ߊl
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Capture();
            }
        }

        //Debug.Log($"Input Check -> h: {h:F2}, v: {v:F2}, running: {newRunning}");
    }

    private void Move()
    {
        // ���̓x�N�g���𑬓x�ɕϊ�
        Vector3 moveVelocity = inputMove * speed;

        // Rigidbody �̑��x�ɒ��ڑ��
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    private void ApplyRotation()
    {
        // ��~���ł� lastMoveDir �������i�Ō�̕������ێ��j
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
            Debug.Log("�ł��߂��G��ߊl���܂����I");
        }
        else
        {
            Debug.Log("�ߊl�͈͓��ɓG�͂��܂���ł����B");
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
            Debug.Log("�ǂɂԂ������I");
        }
    }
}
