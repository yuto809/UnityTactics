using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    /// <summary>
    /// Turn情報に使用するImage(prefab)
    /// </summary>
    [SerializeField]
    private GameObject _turnCharacterImage;

    [SerializeField]
    private GameObject _scrollviewContent;

    // Sort前のターンリスト(origin)
    private List<Character> _characterList;

    private List<Character> _turnList;

    private CharacterManager _characterManager;
    private SpriteManager _spriteManager;
    private CameraManager _cameraManager;


    // 現在操作が可能なキャラクターを保持する
    private Character _activeTurnCharacter;

    private Transform _parentContent;

    /// <summary>
    /// 見かけ上のターンリストUIを更新するかどうかのフラグ
    /// </summary>
    private bool _changeTurnListUI;

    /// <summary>
    /// True : player / False : Enemy
    /// </summary>
    private bool _turnFlg;

    public static TurnManager instance;

    public Character ActiveTurnCharacter
    {
        get
        {
            return _activeTurnCharacter;
        }
    }

    // ゲーム開始時に存在するキャラクター情報を保持する
    public List<Character> SetCharacterTurnList
    {
        set
        {
            _characterList = value;
        }
    }

    /// <summary>
    /// プレイヤー、敵どちらがターン開始したかどうかのフラグ
    /// </summary>
    public bool TurnFlg
    {
        get
        {
            return _turnFlg;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        _cameraManager = CameraManager.instance;
        _characterManager = CharacterManager.instance;
        _spriteManager = SpriteManager.instance;
        // スクロールバー(画像リストを保持する親オブジェクト)
        _parentContent = GameObject.Find("Content").transform;
        _characterList = new List<Character>();
        _turnList = new List<Character>();

        _changeTurnListUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Turn情報を表示するための初期化(CharacterManagerからコール)
    /// </summary>
    public void TurnDispInit()
    {
        // 各キャラクターの「素早さ」を降順に並び変える
        var turnList = _characterList.OrderByDescending(d => d.GetCharactrerData.Quick);

        // ゲーム上のすべてのキャラクターを走査
        foreach (Character character in turnList)
        {
            // ソート後のターンリストを保持
            _turnList.Add(character);

            // 事前に登録しているキャラクター画像を走査
            foreach (Sprite sprite in _spriteManager.CharacterImageList)
            {
                if (sprite.name.Equals(character.GetCharactrerData.CharacterName))
                {
                    // scrollviewのcontentにprefabを追加する
                    _turnCharacterImage.GetComponent<Image>().sprite = sprite;

                    // 画像を設定するオブジェクトにユニークID情報を載せる
                    _turnCharacterImage.GetComponent<TurnCharacterImageInfo>().UniqueId = character.UniqueId;


                    GameObject item = Instantiate(_turnCharacterImage, _scrollviewContent.transform);
                    break;
                }
            }
        }

        // 操作可能キャラクターを保持
        _activeTurnCharacter = _turnList[0];

        _activeTurnCharacter.ChangeActiveOperator(true);

        // 開始時のターンフラグ(プレイヤー、敵どちらが最初にターン開始なのか)設定
        SetTurnFlg();

        Debug.Log("End TurnDispInit");
    }

    /// <summary>
    /// 次のターン遷移時の処理
    /// </summary>
    public void NextTurn()
    {
        // HP0となったキャラはターンリストから削除する
        DeleteCharaFromTurnList();

        // 内部のターンリスト情報を更新
        ChangeTurnListInfo(0);

        _changeTurnListUI = true;
        // 見かけ上のターンリスト情報を更新
        //ChangeTurnListUI();
    }

    /// <summary>
    /// 開始時のターンフラグ(プレイヤー、敵どちらが最初にターン開始なのか)設定
    /// </summary>
    private void SetTurnFlg()
    {
        // 操作キャラが敵ならfalseとする
        if (_activeTurnCharacter.GetCharactrerData.IsEnemy)
        {
            _turnFlg = false;
        }
        else
        {
            _turnFlg = true;
        }
    }

    /// <summary>
    /// HP0となったキャラはターンリストから削除する
    /// </summary>
    private void DeleteCharaFromTurnList()
    {
        // Destroyされたキャラクター名が存在する場合
        //if(_characterManager.DeleteCharacterNameList.Count > 0)
        //{

        //    Debug.Log("HP0ターンリスト数：" + _characterManager.DeleteCharacterNameList.Count);
        //}
        if (_characterManager.DeleteCharacterDataList.Count > 0)
        {

            Debug.Log("HP0ターンリスト数：" + _characterManager.DeleteCharacterDataList.Count);
        }
        else
        {
            return;
        }

        // 現在のターンリスト情報内にHPが0になってしまったキャラクターがいればリスト内から削除
        _turnList.RemoveAll(s => s.CurrentHp == 0);

        // 消すキャラのユニークIDで、childにアクセスして消す
        // スクロールバー内の子オブジェクトを取得(画像リスト)
        foreach (Transform child in _parentContent)
        {
            foreach (DeleteCharacterData data in _characterManager.DeleteCharacterDataList)
            {
                Debug.Log("子要素キャラクター名：" + child.GetComponent<Image>().sprite + "削除キャラクター名：" + data.charaName);
                // キャラクターの名前と使用している画像名が一致したらターンリスト内から削除する
                // オブジェクト名じゃなくて画像ファイル名で比較
                if (child.GetComponent<Image>().sprite.name.Equals(data.charaName) && (data.uniqueId == child.GetComponent<TurnCharacterImageInfo>().UniqueId))
                {
                    Debug.Log("ターンリストから削除するキャラクター名：" + data.charaName);
                    GameObject.Destroy(child.gameObject);
                }

            }
        }



        //// スクロールバー内の子オブジェクトを取得(画像リスト)
        //foreach (Transform child in _parentContent)
        //{
        //    foreach (string charaName in _characterManager.DeleteCharacterNameList)
        //    {
        //        Debug.Log("子要素キャラクター名：" + child.GetComponent<Image>().sprite + "削除キャラクター名：" + charaName);
        //        // キャラクターの名前と使用している画像名が一致したらターンリスト内から削除する
        //        // オブジェクト名じゃなくて画像ファイル名で比較
        //        if (child.GetComponent<Image>().sprite.name.Equals(charaName))
        //        {
        //            Debug.Log("ターンリストから削除するキャラクター名：" + charaName);
        //            GameObject.Destroy(child.gameObject);
        //        }

        //    }
        //}
    }

    /// <summary>
    /// 内部のターンリスト情報を更新
    /// </summary>
    /// <param name="idx"></param>
    private void ChangeTurnListInfo(int idx)
    {
        Debug.Log("ChangeTurnListInfo");

        // 先頭情報保持
        Character tmp = _turnList[idx];

        // 内部情報を入れ替える
        for (int i = 0; i < _turnList.Count; i++)
        {
            if (((i + 1) % _turnList.Count) == 0)
            {
                _turnList[i] = tmp;
            }
            else
            {
                _turnList[i] = _turnList[i + 1];
            }
        }

        // ターン切り替え前のキャラを対象
        //_activeTurnCharacter.ChangeActiveOperator();

        // 操作可能キャラクターを保持
        _activeTurnCharacter = _turnList[0];
        Debug.Log("現在行動中のキャラクターのHP：" + _activeTurnCharacter.CurrentHp);

        //_activeTurnCharacter.ChangeActiveOperator();

        SetTurnFlg();

        // カメラ位置を変更する
        _cameraManager.ChangeActiveVirtualCamera(_turnList[0], _turnList[1]);
        //_cameraManager.SetCameraPosition(_activeTurnCharacter.transform.position.x, _activeTurnCharacter.transform.position.z);
        Debug.Log("ChangeTurnListInfo 座標：" + _activeTurnCharacter.transform.position);
    }

    /// <summary>
    /// 見かけ上のターンリスト情報を更新
    /// </summary>
    /// <param name="idx"></param>
    public void ChangeTurnListUI()
    {
        if (_changeTurnListUI)
        {
            // ターンリスト画像の入れ替え
            for (int i = 0; i < _parentContent.childCount; i++)
            {
                if (((i + 1) % _parentContent.childCount) == 0)
                {
                    _parentContent.GetChild(0).SetSiblingIndex(0);
                }
                else
                {
                    _parentContent.GetChild(i + 1).SetSiblingIndex(i);
                }
            }

            _changeTurnListUI = false;
        }
    }
}
