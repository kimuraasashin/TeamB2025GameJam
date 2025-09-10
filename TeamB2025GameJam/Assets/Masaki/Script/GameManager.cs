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
                    break;
                case Game.GameOver:
                    //ゲームオーバー表示
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
}
