using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Scene-Masaki"; //遷移先シーン名
    [SerializeField] private Image fadePanel;           //フェード用画像(黒)
    [SerializeField] private float fadeDuration = 1.0f; //フェードにかける時間
    private bool isFading = false; //フェードしているか

    private void Start()
    {
        // 最初は完全に透明にする
        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;
    }

    void Update()
    {
        // クリックされたかつ、まだフェードしていなければフェード開始
        if (Input.GetMouseButtonDown(0) && !isFading)
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        //フラグ管理
        isFading = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // 0 → 1 に補間
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            Color c = fadePanel.color;
            c.a = alpha;
            fadePanel.color = c;

            yield return null; // 1フレーム待つ
        }

        //シーン遷移
        SceneManager.LoadScene(nextSceneName);
    }
}
