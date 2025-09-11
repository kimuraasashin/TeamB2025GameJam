using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Scene-Masaki"; //�J�ڐ�V�[����
    [SerializeField] private Image fadePanel;           //�t�F�[�h�p�摜(��)
    [SerializeField] private float fadeDuration = 1.0f; //�t�F�[�h�ɂ����鎞��
    private bool isFading = false; //�t�F�[�h���Ă��邩

    private void Start()
    {
        // �ŏ��͊��S�ɓ����ɂ���
        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;
    }

    void Update()
    {
        // �N���b�N���ꂽ���A�܂��t�F�[�h���Ă��Ȃ���΃t�F�[�h�J�n
        if (Input.GetMouseButtonDown(0) && !isFading)
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        //�t���O�Ǘ�
        isFading = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // 0 �� 1 �ɕ��
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            Color c = fadePanel.color;
            c.a = alpha;
            fadePanel.color = c;

            yield return null; // 1�t���[���҂�
        }

        //�V�[���J��
        SceneManager.LoadScene(nextSceneName);
    }
}
