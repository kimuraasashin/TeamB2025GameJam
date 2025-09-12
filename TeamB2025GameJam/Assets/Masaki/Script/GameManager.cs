using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ゲームの状態の列挙
    /// </summary>
    enum Game
    {
        GamePlay,
        GameClear,
        GameOver,
    }

    [SerializeField]
    private const int treasureQuantity = 2; //フィールドに存在する宝の数(固定数)
    [SerializeField]
    private const int enemyQuantity = 1;    //フィールドに存在する義賊の数(固定数)
    private int stolenCount = 0;            //盗られた宝のカウント
    private int enemyCount = 0;             //捕まえた義賊のカウント
    private bool isEnemyGoal = false;       //義賊がゴールしたか
    private bool isGameEnd = false;         //ゲームが終了したか
    private Game gameState = Game.GamePlay; //現在のゲームの状態
    public Image fadePanel;                 //暗くする用の画像
    public Image gameClear;                 //ゲームクリアのロゴ
    public Image gameOver;                  //ゲームオーバーのロゴ

    //演出用パラメータ
    [SerializeField] private float clearScaleDuration = 0.8f;
    [SerializeField] private float overMoveDuration = 0.8f;
    [SerializeField] private float gameOverTargetY = 0f; //最終的に移動したいY座標

    //演出管理用フラグ
    private bool didShowClearAnimation = false;
    private bool didShowOverAnimation = false;

    //ゲーム開始に必要な要素
    [SerializeField] private Transform fieldTransform = null;  //フィールド（safe positionsを子に持つオブジェクト）
    [SerializeField] private GameObject playerPrefab = null;   //プレイヤーのプレハブ
    [SerializeField] private GameObject enemyPrefab = null;    //敵のプレハブ

    [SerializeField] private Transform treasurePos = null;     //宝の配置位置
    [SerializeField] private GameObject treasurePrefab = null; //宝のプレハブ

    [SerializeField] private Transform goalTranceform = null;  //ゴールの位置

    //一度だけ生成するためのガード
    private bool didSpawnInitial = false;

    private void Start()
    {
        // 最初は完全に不透明にしておく
        SetAlpha(1f);

        // 1秒かけて透明にフェードイン
        StartCoroutine(FadeTo(0f, 0.5f));

        // フィールドの子からランダムに2つ選んでプレイヤーと敵を配置し、宝も配置
        if (!didSpawnInitial)
        {
            SpawnPlayerAndEnemyAtRandomChildren();
            SpawnTreasure();
            didSpawnInitial = true;
        }
    }

    private void Update()
    {
        //敵をタグで検索
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //捕まえた義賊の数が義賊の総数になれば、ゲームクリア
        if(enemyCount == enemyQuantity)
        {
            ChangeState(Game.GameClear);
        }

        //盗られた宝の数がフィールドに存在する宝の数と同じになる(全ての宝を盗られた)かつ、敵がゴールしたなら、ゲームオーバー
        if(stolenCount == treasureQuantity && isEnemyGoal)
        {
            ChangeState(Game.GameOver);
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            ChangeState(Game.GameOver);
        }

        //ゲームの状態がプレイ中でなければ
        if (gameState != Game.GamePlay)
        {
            //もしゲームが終了していなければ
            if(!isGameEnd)
            {
                //0.7 までフェードアウトし、ゲームを終了に設定
                StartCoroutine(FadeTo(0.7f, 0.5f));
                isGameEnd = true;
            }

            //ゲームクリアかゲームオーバーで演出を変更
            switch (gameState)
            {
                case Game.GameClear:
                    //ゲームクリア表示
                    if (!didShowClearAnimation)
                    {
                        StartCoroutine(PlayGameClear());
                        didShowClearAnimation = true;
                    }
                    break;
                case Game.GameOver:
                    //ゲームオーバー表示
                    if (!didShowOverAnimation)
                    {
                        StartCoroutine(PlayGameOver());
                        didShowOverAnimation = true;
                    }
                    break;
            }

            //左クリックでタイトルに遷移
            if(Input.GetMouseButton(0))
            {
                SceneManager.LoadScene("Title");
            }
        }
    }

    /// <summary>
    /// ゲームの状態を変更
    /// </summary>
    /// <param name="game"></param>
    void ChangeState(Game game)
    {
        gameState = game;
    }

    /// <summary>
    /// 宝を盗られた(カウントを増加)
    /// </summary>
    public void Stolen()
    {
        stolenCount++;
    }

    /// <summary>
    /// 敵がゴールした
    /// </summary>
    public void EnemyGoal()
    {
        isEnemyGoal = true;
    }

    public void EnemyCapture()
    {
        enemyCount++;
    }

    public void SetAlpha(float alpha)
    {
        if (fadePanel == null) return;
        alpha = Mathf.Clamp01(alpha);
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
        fadePanel.raycastTarget = (alpha > 0.01f);
    }

    /// <summary>
    /// フェード演出
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadePanel == null) yield break;
        targetAlpha = Mathf.Clamp01(targetAlpha);

        float startAlpha = fadePanel.color.a;
        if (duration <= 0f)
        {
            SetAlpha(targetAlpha);
            yield break;
        }

        float elapsed = 0f;
        Color c = fadePanel.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadePanel.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        fadePanel.color = c;
    }

    /// <summary>
    /// ゲームクリア演出 画像のスケールを1まで上昇
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayGameClear()
    {
        if (gameClear == null) yield break;

        RectTransform rt = gameClear.rectTransform;

        // 表示しておく（非表示にしている場合に備えて）
        gameClear.gameObject.SetActive(true);

        Vector3 startScale = rt.localScale;
        Vector3 targetScale = Vector3.one; // スケール1にする

        float elapsed = 0f;
        // durationが0以下なら即設定
        if (clearScaleDuration <= 0f)
        {
            rt.localScale = targetScale;
            yield break;
        }

        while (elapsed < clearScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / clearScaleDuration);
            // 緩やかなイージング
            float ease = Mathf.SmoothStep(0f, 1f, t);
            rt.localScale = Vector3.Lerp(startScale, targetScale, ease);
            yield return null;
        }
        rt.localScale = targetScale;
    }

    /// <summary>
    /// ゲームオーバー演出 画像のy座標を0まで移動
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayGameOver()
    {
        if (gameOver == null) yield break;

        RectTransform rt = gameOver.rectTransform;

        // 表示しておく（非表示にしている場合に備えて）
        gameOver.gameObject.SetActive(true);

        Vector2 startPos = rt.anchoredPosition;
        float startY = startPos.y;
        float targetY = gameOverTargetY;

        float elapsed = 0f;
        if (overMoveDuration <= 0f)
        {
            startPos.y = targetY;
            rt.anchoredPosition = startPos;
            yield break;
        }

        while (elapsed < overMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / overMoveDuration);
            float ease = Mathf.SmoothStep(0f, 1f, t);
            float y = Mathf.Lerp(startY, targetY, ease);
            Vector2 p = rt.anchoredPosition;
            p.y = y;
            rt.anchoredPosition = p;
            yield return null;
        }

        Vector2 final = rt.anchoredPosition;
        final.y = targetY;
        rt.anchoredPosition = final;
    }

    /// <summary>
    /// fieldTransform の子のうちランダムに2つ選んで、1つにプレイヤー、もう1つに敵を生成
    /// </summary>
    private void SpawnPlayerAndEnemyAtRandomChildren()
    {
        int childCount = fieldTransform.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("Field Transform has no children to use as spawn points.");
            return;
        }

        // childCount が 1 の場合は同じ位置に両方置く（あるいは別動作にしたい場合はここを調整）
        int idx1 = Random.Range(0, childCount);
        int idx2 = idx1;
        if (childCount > 1)
        {
            // idx2 が idx1 と被らないように選択
            do
            {
                idx2 = Random.Range(0, childCount);
            } while (idx2 == idx1);
        }

        Transform t1 = fieldTransform.GetChild(idx1);
        Transform t2 = fieldTransform.GetChild(idx2);

        // プレイヤーと敵を生成（位置と回転は子の Transform に合わせる）
        Instantiate(playerPrefab, t1.position, t1.rotation);
        GameObject enemy = Instantiate(enemyPrefab, t2.position, t2.rotation);
        
        
        NPC npc = enemy.GetComponent<NPC>();
        npc.goal = goalTranceform;
    }

    /// <summary>
    /// 宝を生成
    /// </summary>
    private void SpawnTreasure()
    {
        int childCount = treasurePos.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("TreasurePos has no children to use as spawn points.");
            return;
        }

        int idx1 = Random.Range(0, childCount);
        int idx2 = idx1;
        if (childCount > 1)
        {
            do
            {
                idx2 = Random.Range(0, childCount);
            } while (idx2 == idx1);
        }

        Transform t1 = treasurePos.GetChild(idx1);
        Transform t2 = treasurePos.GetChild(idx2);

        Instantiate(treasurePrefab, t1.position, t1.rotation);
        Instantiate(treasurePrefab, t2.position, t2.rotation);
    }
}
