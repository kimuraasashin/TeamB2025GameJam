using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�ړ�")]
    public float speed = 5f; //�ړ����x
    private Rigidbody rb;    //���W�b�h�{�f�B


    [Header("�ߊl")]
    public float captureRange = 5f; //�ߊl�͈�
    public float captureCT = 3f;    //�ߊl����̃N�[���^�C��
    public float CTCount = 0f;      //�o�ߎ���

    void Start()
    {
        //���W�b�h�{�f�B���擾
        rb = GetComponent<Rigidbody>();

        // Rigidbody����]���Ȃ��悤�ɌŒ�i�L�������]����Ȃ��悤�ɂ���j
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        //�ړ�����
        Move();
    }

    private void Update()
    {
        //�N�[���^�C�����I����Ă��Ȃ���΁A�N�[���^�C��������
        if(CTCount >= 0.0f)
        {
            CTCount -= Time.deltaTime;
        }
        //�N�[���^�C�����I����Ă���΁A�X�y�[�X�L�[�ŕߊl
        else
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Capture();
            }
        }
    }

    /// <summary>
    /// �V���v���Ȉړ�(FixedUpdate�ŌĂяo��)
    /// </summary>
    private void Move()
    {
        // A,D�L�[�ō��E�ړ��AW,S�L�[�őO��ړ�
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Debug.Log($"h={h}, v={v}");

        // �}�b�v��i���[���h���W�j�̈ړ��x�N�g��
        Vector3 move = new Vector3(h, 0, v).normalized * speed;

        // Rigidbody���g���Ĉʒu���X�V�i�Փ˔��肠��j
        rb.MovePosition(transform.position + move * Time.fixedDeltaTime);
    }

    /// <summary>
    /// �͈͓��̋`����ߊl(�X�y�[�X�L�[�œ���)
    /// </summary>
    private void Capture()
    {
        // �͈͓���Collider���擾
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, captureRange);

        Collider nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hitColliders)
        {
            // "Enemy"�^�O�̂����I�u�W�F�N�g���Ώ�
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);

                // ���߂��Ώۂ�����������X�V
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestTarget = hit;
                }
            }
        }

        // ��ԋ߂��Ώۂ�����������ߊl����
        if (nearestTarget != null)
        {
            //�߂܂����`�����폜(����������������A�`����OnCaptured�ɕύX)
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
}
