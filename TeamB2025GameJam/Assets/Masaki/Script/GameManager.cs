using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField]
    private const int enemyQuantity = 1;    //�t�B�[���h�ɑ��݂���`���̐�(�Œ萔)
    private int stolenCount = 0;            //����ꂽ��̃J�E���g
    private int enemyCount = 0;             //�߂܂����`���̃J�E���g
    private bool isEnemyGoal = false;       //�`�����S�[��������
    private bool isGameEnd = false;         //�Q�[�����I��������
    private Game gameState = Game.GamePlay; //���݂̃Q�[���̏��
    public Image fadePanel;                 //�Â�����p�̉摜
    public Image gameClear;                 //�Q�[���N���A�̃��S
    public Image gameOver;                  //�Q�[���I�[�o�[�̃��S

    //���o�p�p�����[�^
    [SerializeField] private float clearScaleDuration = 0.8f;
    [SerializeField] private float overMoveDuration = 0.8f;
    [SerializeField] private float gameOverTargetY = 0f; //�ŏI�I�Ɉړ�������Y���W

    //���o�Ǘ��p�t���O
    private bool didShowClearAnimation = false;
    private bool didShowOverAnimation = false;

    //�Q�[���J�n�ɕK�v�ȗv�f
    [SerializeField] private Transform fieldTransform = null;  //�t�B�[���h�isafe positions���q�Ɏ��I�u�W�F�N�g�j
    [SerializeField] private GameObject playerPrefab = null;   //�v���C���[�̃v���n�u
    [SerializeField] private GameObject enemyPrefab = null;    //�G�̃v���n�u

    [SerializeField] private Transform treasurePos = null;     //��̔z�u�ʒu
    [SerializeField] private GameObject treasurePrefab = null; //��̃v���n�u

    [SerializeField] private Transform goalTranceform = null;  //�S�[���̈ʒu

    //��x�����������邽�߂̃K�[�h
    private bool didSpawnInitial = false;

    private void Start()
    {
        // �ŏ��͊��S�ɕs�����ɂ��Ă���
        SetAlpha(1f);

        // 1�b�����ē����Ƀt�F�[�h�C��
        StartCoroutine(FadeTo(0f, 0.5f));

        // �t�B�[���h�̎q���烉���_����2�I��Ńv���C���[�ƓG��z�u���A����z�u
        if (!didSpawnInitial)
        {
            SpawnPlayerAndEnemyAtRandomChildren();
            SpawnTreasure();
            didSpawnInitial = true;
        }
    }

    private void Update()
    {
        //�G���^�O�Ō���
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //�߂܂����`���̐����`���̑����ɂȂ�΁A�Q�[���N���A
        if(enemyCount == enemyQuantity)
        {
            ChangeState(Game.GameClear);
        }

        //����ꂽ��̐����t�B�[���h�ɑ��݂����̐��Ɠ����ɂȂ�(�S�Ă̕�𓐂�ꂽ)���A�G���S�[�������Ȃ�A�Q�[���I�[�o�[
        if(stolenCount == treasureQuantity && isEnemyGoal)
        {
            ChangeState(Game.GameOver);
        }

        if(Input.GetKeyDown(KeyCode.V))
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
                    if (!didShowClearAnimation)
                    {
                        StartCoroutine(PlayGameClear());
                        didShowClearAnimation = true;
                    }
                    break;
                case Game.GameOver:
                    //�Q�[���I�[�o�[�\��
                    if (!didShowOverAnimation)
                    {
                        StartCoroutine(PlayGameOver());
                        didShowOverAnimation = true;
                    }
                    break;
            }

            //���N���b�N�Ń^�C�g���ɑJ��
            if(Input.GetMouseButton(0))
            {
                SceneManager.LoadScene("Title");
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

    /// <summary>
    /// �G���S�[������
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
    /// �t�F�[�h���o
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
    /// �Q�[���N���A���o �摜�̃X�P�[����1�܂ŏ㏸
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayGameClear()
    {
        if (gameClear == null) yield break;

        RectTransform rt = gameClear.rectTransform;

        // �\�����Ă����i��\���ɂ��Ă���ꍇ�ɔ����āj
        gameClear.gameObject.SetActive(true);

        Vector3 startScale = rt.localScale;
        Vector3 targetScale = Vector3.one; // �X�P�[��1�ɂ���

        float elapsed = 0f;
        // duration��0�ȉ��Ȃ瑦�ݒ�
        if (clearScaleDuration <= 0f)
        {
            rt.localScale = targetScale;
            yield break;
        }

        while (elapsed < clearScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / clearScaleDuration);
            // �ɂ₩�ȃC�[�W���O
            float ease = Mathf.SmoothStep(0f, 1f, t);
            rt.localScale = Vector3.Lerp(startScale, targetScale, ease);
            yield return null;
        }
        rt.localScale = targetScale;
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o �摜��y���W��0�܂ňړ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayGameOver()
    {
        if (gameOver == null) yield break;

        RectTransform rt = gameOver.rectTransform;

        // �\�����Ă����i��\���ɂ��Ă���ꍇ�ɔ����āj
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
    /// fieldTransform �̎q�̂��������_����2�I��ŁA1�Ƀv���C���[�A����1�ɓG�𐶐�
    /// </summary>
    private void SpawnPlayerAndEnemyAtRandomChildren()
    {
        int childCount = fieldTransform.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("Field Transform has no children to use as spawn points.");
            return;
        }

        // childCount �� 1 �̏ꍇ�͓����ʒu�ɗ����u���i���邢�͕ʓ���ɂ������ꍇ�͂����𒲐��j
        int idx1 = Random.Range(0, childCount);
        int idx2 = idx1;
        if (childCount > 1)
        {
            // idx2 �� idx1 �Ɣ��Ȃ��悤�ɑI��
            do
            {
                idx2 = Random.Range(0, childCount);
            } while (idx2 == idx1);
        }

        Transform t1 = fieldTransform.GetChild(idx1);
        Transform t2 = fieldTransform.GetChild(idx2);

        // �v���C���[�ƓG�𐶐��i�ʒu�Ɖ�]�͎q�� Transform �ɍ��킹��j
        Instantiate(playerPrefab, t1.position, t1.rotation);
        GameObject enemy = Instantiate(enemyPrefab, t2.position, t2.rotation);
        
        
        NPC npc = enemy.GetComponent<NPC>();
        npc.goal = goalTranceform;
    }

    /// <summary>
    /// ��𐶐�
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
