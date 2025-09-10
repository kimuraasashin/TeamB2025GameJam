using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// �Q�[���̏�Ԃ̗�
    /// </summary>
    enum Game
    {
        GamePlay,
        GameClear,
        GameOver,
    }

    [SerializeField]
    private const int treasureQuantity = 2; //�t�B�[���h�ɑ��݂����̐�(�Œ萔)
    private int stolenCount = 0;            //����ꂽ��̃J�E���g
    private bool isGameEnd = false;         //�Q�[�����I��������
    private Game gameState = Game.GamePlay; //���݂̃Q�[���̏��
    public Image fadePanel;                 //�Â�����p�̉摜

    private void Start()
    {
        // �ŏ��͊��S�ɕs�����ɂ��Ă���
        SetAlpha(1f);

        // 1�b�����ē����Ƀt�F�[�h�C��
        StartCoroutine(FadeTo(0f, 0.5f));
    }

    private void Update()
    {
        //�G���^�O�Ō���
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //�G�����݂��Ȃ���΁A�Q�[���N���A
        if(enemies.Length == 0)
        {
            ChangeState(Game.GameClear);
        }

        //����ꂽ��̐����t�B�[���h�ɑ��݂����̐��Ɠ����ɂȂ��(�S�Ă̕�𓐂�ꂽ�Ȃ�)�A�Q�[���I�[�o�[
        if(stolenCount == treasureQuantity)
        {
            ChangeState(Game.GameOver);
        }

        //�Q�[���̏�Ԃ��v���C���łȂ����
        if (gameState != Game.GamePlay)
        {
            //�����Q�[�����I�����Ă��Ȃ����
            if(!isGameEnd)
            {
                //0.7 �܂Ńt�F�[�h�A�E�g���A�Q�[�����I���ɐݒ�
                StartCoroutine(FadeTo(0.7f, 0.5f));
                isGameEnd = true;
            }

            //�Q�[���N���A���Q�[���I�[�o�[�ŉ��o��ύX
            switch (gameState)
            {
                case Game.GameClear:
                    //�Q�[���N���A�\��
                    break;
                case Game.GameOver:
                    //�Q�[���I�[�o�[�\��
                    break;
            }
        }
    }

    /// <summary>
    /// �Q�[���̏�Ԃ�ύX
    /// </summary>
    /// <param name="game"></param>
    void ChangeState(Game game)
    {
        gameState = game;
    }

    /// <summary>
    /// ��𓐂�ꂽ(�J�E���g�𑝉�)
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
        // �K�v�Ȃ�
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
