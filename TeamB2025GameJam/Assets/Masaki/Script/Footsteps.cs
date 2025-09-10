using UnityEngine;
using UnityEngine.UI;

public class Footsteps : MonoBehaviour
{
    [Header("����臒l")]
    [SerializeField] private float captureRange = 3f;     //���[�^�[3�̐��l
    [SerializeField] private float meter2Threshold = 5f;  //���[�^�[2�̐��l
    [SerializeField] private float meter1Threshold = 10f; //���[�^�[1�̐��l

    [Header("�v���C���[")]
    [SerializeField] private Transform playerTransform; //Inspector�Ŋ�����

    [Header("�~�̉摜")]
    [SerializeField] private GameObject circle1; //��(���[�^�[1�̉~)
    [SerializeField] private GameObject circle2; //��(���[�^�[2�̉~)
    [SerializeField] private GameObject circle3; //�O(���[�^�[3�̉~)

    void Start()
    {
        //�v���C���[�����ݒ�Ȃ�ȈՃt�H�[���o�b�N
        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        //�����\���̓I�t
        UpdateUI(0);
    }

    void Update()
    {
        //�G���^�O�Ō���
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float minDist = float.MaxValue;
        bool foundMoving = false;

        //�ł��߂��ړ����̓G��T��
        foreach (var e in enemies)
        {
            var npc = e.GetComponent<NPC>();
            if (npc == null) continue;
            if (!npc.move) continue; //move��true�̓G����(�����Ă���G)

            float d = Vector3.Distance(playerTransform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                foundMoving = true;
            }
        }

        int meter = 0;
        if (foundMoving)
        {
            if (minDist <= captureRange) meter = 3;
            else if (minDist <= meter2Threshold) meter = 2;
            else if (minDist <= meter1Threshold) meter = 1;
            else meter = 0;
        }
        else
        {
            meter = 0;
        }

        UpdateUI(meter);
    }

    /// <summary>
    /// �~�̕\���؂�ւ�
    /// </summary>
    /// <param name="meter">���݂̃��[�^�[�̒l</param>
    private void UpdateUI(int meter)
    {
        if (circle1 != null) circle1.SetActive(meter >= 1);
        if (circle2 != null) circle2.SetActive(meter >= 2);
        if (circle3 != null) circle3.SetActive(meter >= 3);
    }
}