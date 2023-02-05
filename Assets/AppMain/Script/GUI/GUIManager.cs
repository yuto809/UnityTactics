using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform _commandPanel;

    [SerializeField]
    private RectTransform _statusPanel;

    [SerializeField]
    private Button _moveButton;

    [SerializeField]
    private Button _attackButton;

    [SerializeField]
    private GameObject _battleStartPanel;

    [SerializeField]
    private List<TextMeshProUGUI> _battleStartMessages;

    private GameManager _gameManager;

    /// <summary>
    /// コマンドメニューが表示中かどうかを示す変数(True:表示中)
    /// </summary>
    private bool _isCommandWindowVisible;

    /// <summary>
    /// キャラクターステータス画面が表示中かどうかを示す変数(True:表示中)
    /// </summary>
    private bool _isStatusWindowVisible;

    private int _currentMessageIndex;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _isStatusWindowVisible = false;
        _isCommandWindowVisible = false;
        _currentMessageIndex = -1;
    }

    /// <summary>
    /// 画面上から「移動」を選択したときの処理
    /// </summary>
    public void ShowCharacterMoveArea()
    {
        _gameManager.ShowMoveArea();
        HideStatusWindow();
    }

    /// <summary>
    /// 画面上から、「攻撃」を選択したときの処理
    /// </summary>
    public void ShowCharacterAttackArea()
    {
        _gameManager.ShowAttackArea();
        HideStatusWindow();
    }

    /// <summary>
    /// 画面上から、「待機」を選択したときの処理
    /// </summary>
    public void EndCharacterTurn()
    {
        // 操作キャラクター情報を初期化する
        _gameManager.ClearSelectCharacter();
        HideCommandMenu();
        _gameManager.ClearCommandMenuFlg();
    }

    /// <summary>
    /// コマンドメニューを開く(移動・攻撃・待機メニュー)
    /// </summary>
    public void SlideCommandMenu()
    {
        Debug.Log("SlideCommandMenu : " + _isCommandWindowVisible);

        if (!_isCommandWindowVisible)
        {
            _commandPanel.DOLocalMoveX(400f, 0.5f);
            //_commandPanelAnim.SetTrigger("SlideIn");
            _isCommandWindowVisible = true;
        }

        // 既に実行したアクションがあるならボタンを非活性にする
        ChangeButtonEnable();
    }

    /// <summary>
    /// キャラクターのステータスウィンドウを開く
    /// </summary>
    public void SlideStatusWindow()
    {
        Debug.Log("SlideStatusWindow : " + _isStatusWindowVisible);

        if (!_isStatusWindowVisible)
        {
            _statusPanel.DOLocalMoveX(-320f, 0.5f);
            //_statusPanelAnim.SetTrigger("SlideIn");
            _isStatusWindowVisible = true;
        }
    }

    /// <summary>
    /// コマンドメニューを閉じる
    /// </summary>
    public void HideCommandMenu()
    {
        _gameManager.ClearSelectCharacterFlg();

        if (_isCommandWindowVisible)
        {
            _commandPanel.DOLocalMoveX(700f, 0.5f);
            //_commandPanelAnim.SetTrigger("SlideOut");
            _isCommandWindowVisible = false;

        }

        if (_isStatusWindowVisible)
        {
            _statusPanel.DOLocalMoveX(-800f, 0.5f);
            //_statusPanelAnim.SetTrigger("SlideOut");
            _isStatusWindowVisible = false;
        }

    }

    /// <summary>
    /// ステータスウィンドウを閉じる
    /// </summary>
    private void HideStatusWindow()
    {
        Debug.Log("HideStatusWindow");

        if (_isStatusWindowVisible)
        {
            _statusPanel.DOLocalMoveX(-800f, 0.5f);
            //_statusPanelAnim.SetTrigger("SlideOut");
            _isStatusWindowVisible = false;
        }
    }

    /// <summary>
    /// 被ダメージ時に画面を揺らす
    /// </summary>
    public void ShakeStatusWindow()
    {
        Debug.Log("Shake");
        _statusPanel.DOShakePosition(2.0f);
        _statusPanel.DOShakePosition(1.0f, 30.0f,5,90.0f,false,true);
    }

    /// <summary>
    /// 移動/攻撃/待機ボタンの状態管理
    /// </summary>
    private void ChangeButtonEnable()
    {
        if (_gameManager.GetEndMoveFlg)
        {
            _moveButton.enabled = false;
            _moveButton.image.color = Color.gray;
        }
        else
        {
            _moveButton.enabled = true;
            _moveButton.image.color = Color.white;
        }
        if (_gameManager.GetEndAttackFlg)
        {
            _attackButton.enabled = false;
            _attackButton.image.color = Color.gray;
        }
        else
        {
            _attackButton.enabled = true;
            _attackButton.image.color = Color.white;
        }
    }


    private void SetEnablePanel()
    {
        _battleStartPanel.SetActive(false);
    }



    /// <summary>
    /// 戦闘開始メッセージ(パネル)を表示する
    /// </summary>
    public void ShowBattleStart()
    {
        _battleStartPanel.GetComponent<Image>().DOFade(0.5f, 3.0f);


        ShowBattleStartMessage();
    }

    /// <summary>
    /// アタッチされたメッセージを戦闘開始メッセージとして表示する
    /// </summary>
    private void ShowBattleStartMessage()
    {
        _currentMessageIndex++;

        if (_currentMessageIndex >= _battleStartMessages.Count)
        {
            _currentMessageIndex = -1;
            _battleStartPanel.GetComponent<Image>().DOFade(0f, 3.0f).OnComplete(SetEnablePanel);

            return;
        }

        TextMeshProUGUI textMeshPro = _battleStartMessages[_currentMessageIndex].GetComponent<TextMeshProUGUI>();

        TextMeshInitialize(textMeshPro);
        TextAnimationStart(textMeshPro, 3.0f);
    }

    private void TextMeshInitialize(TextMeshProUGUI text)
    {
        text.DOFade(0, 0);
        text.characterSpacing = -50;
    }

    /// <summary>
    /// 戦闘開始メッセージを徐々に平げながら表示する
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    private void TextAnimationStart(TextMeshProUGUI text, float duration)
    {
        // 文字間隔を開ける
        DOTween.To(
                    () => text.characterSpacing, 
                    value => text.characterSpacing = value, 
                    10, 
                    duration
                   ).SetEase(Ease.OutQuart);

        // フェード
        DOTween.Sequence()
            .Append(text.DOFade(1, duration / 4))
            .AppendInterval(duration / 2)
            .Append(text.DOFade(0, duration / 4))
            .OnStepComplete(ShowBattleStartMessage);
    }

}
