using UnityEngine;
using UnityEngine.UI;

public class Footsteps : MonoBehaviour
{
    [Header("距離閾値")]
    [SerializeField] private float captureRange = 3f;     //メーター3の数値
    [SerializeField] private float meter2Threshold = 5f;  //メーター2の数値
    [SerializeField] private float meter1Threshold = 10f; //メーター1の数値

    [Header("プレイヤー")]
    [SerializeField] private Transform playerTransform; //Inspectorで割当て

    [Header("円の画像")]
    [SerializeField] private GameObject circle1; //内(メーター1の円)
    [SerializeField] private GameObject circle2; //中(メーター2の円)
    [SerializeField] private GameObject circle3; //外(メーター3の円)

    void Start()
    {
        //プレイヤーが未設定なら簡易フォールバック
        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        //初期表示はオフ
        UpdateUI(0);
    }

    void Update()
    {
        //敵をタグで検索
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float minDist = float.MaxValue;
        bool foundMoving = false;

        //最も近い移動中の敵を探索
        foreach (var e in enemies)
        {
            var npc = e.GetComponent<NPC>();
            if (npc == null) continue;
            if (!npc.move) continue; //moveがtrueの敵だけ(動いている敵)

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
    /// 円の表示切り替え
    /// </summary>
    /// <param name="meter">現在のメーターの値</param>
    private void UpdateUI(int meter)
    {
        if (circle1 != null) circle1.SetActive(meter >= 1);
        if (circle2 != null) circle2.SetActive(meter >= 2);
        if (circle3 != null) circle3.SetActive(meter >= 3);
    }
}