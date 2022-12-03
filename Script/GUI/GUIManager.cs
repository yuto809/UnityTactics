using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public enum PANEL
    {
        PANEL_BATTLE_START
    }

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

    private bool _slideCommandPanelFlg;
    private bool _slideStatusWindowFlg;

    private Animator _statusPanelAnim;
    private Animator _commandPanelAnim;

    public static GUIManager instance;

    private int _currentMessageIndex;

    private void Awake()
    {
        if (instance = null)
        {
            instance = this;
        }
    }

    void Start()
    {
        _gameManager = GameManager.instance;
        _statusPanelAnim = _statusPanel.GetComponent<Animator>();
        _commandPanelAnim = _commandPanel.GetComponent<Animator>();
        _slideStatusWindowFlg = false;
        _slideCommandPanelFlg = false;
        _currentMessageIndex = -1;
    }
    
    void Update()
    {
        
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

        // 移動または攻撃が実行済みなら初期化する
        // 何も行動せずに待機を選択した場合も同様
        if ((_gameManager.GetEndAttackFlg || _gameManager.GetEndMoveFlg) ||
            (!_gameManager.GetEndAttackFlg && !_gameManager.GetEndMoveFlg))
        {
            _gameManager.ClearCommandMenuFlg();
        }
    }

    /// <summary>
    /// コマンドメニューを開く(移動・攻撃・待機メニュー)
    /// </summary>
    public void SlideCommandMenu()
    {
        Debug.Log("SlideCommandMenu : " + _slideCommandPanelFlg);

        if (!_slideCommandPanelFlg)
        {
            _commandPanel.DOLocalMoveX(400f, 0.5f);
            //_commandPanelAnim.SetTrigger("SlideIn");
            _slideCommandPanelFlg = true;
        }

        // 既に実行したアクションがあるならボタンを非活性にする
        ChangeButtonEnable();
    }

    /// <summary>
    /// キャラクターのステータスウィンドウを開く
    /// </summary>
    public void SlideStatusWindow()
    {
        Debug.Log("SlideStatusWindow : " + _slideStatusWindowFlg);

        if (!_slideStatusWindowFlg)
        {
            _statusPanel.DOLocalMoveX(-320f, 0.5f);
            //_statusPanelAnim.SetTrigger("SlideIn");
            _slideStatusWindowFlg = true;
        }
    }

    /// <summary>
    /// コマンドメニューを閉じる
    /// </summary>
    public void HideCommandMenu()
    {
        _gameManager.ClearSelectCharacterFlg();

        if (_slideCommandPanelFlg)
        {
            _commandPanel.DOLocalMoveX(700f, 0.5f);
            //_commandPanelAnim.SetTrigger("SlideOut");
            _slideCommandPanelFlg = false;

        }

        if (_slideStatusWindowFlg)
        {
            _statusPanel.DOLocalMoveX(-800f, 0.5f);
            //_statusPanelAnim.SetTrigger("SlideOut");
            _slideStatusWindowFlg = false;
        }

    }

    /// <summary>
    /// ステータスウィンドウを閉じる
    /// </summary>
    public void HideStatusWindow()
    {
        Debug.Log("HideStatusWindow");

        if (_slideStatusWindowFlg)
        {
            _statusPanel.DOLocalMoveX(-800f, 0.5f);
            //_statusPanelAnim.SetTrigger("SlideOut");
            _slideStatusWindowFlg = false;
        }
    }

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




    public void ShowBattleStart()
    {
        _battleStartPanel.GetComponent<Image>().DOFade(0.5f, 3.0f);


        ShowBattleStartMessage();
    }

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
