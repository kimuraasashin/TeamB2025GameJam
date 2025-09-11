using System.Collections;
using UnityEngine;
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
    private int stolenCount = 0;            //盗られた宝のカウント
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

    private void Start()
    {
        // 最初は完全に不透明にしておく
        SetAlpha(1f);

        // 1秒かけて透明にフェードイン
        StartCoroutine(FadeTo(0f, 0.5f));
    }

    private void Update()
    {
        //敵をタグで検索
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //敵が存在しなければ、ゲームクリア
        if(enemies.Length == 0)
        {
            ChangeState(Game.GameClear);
        }

        //盗られた宝の数がフィールドに存在する宝の数と同じになれば(全ての宝を盗られたなら)、ゲームオーバー
        if(stolenCount == treasureQuantity)
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

    public void SetAlpha(float alpha)
    {
        if (fadePanel == null) return;
        alpha = Mathf.Clamp01(alpha);
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
        // 必要なら
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
}
