using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * ・キャラクターの生成
 * ・キャラクター生成時のパラメータ設定
 * ・破壊されたキャラクター名の保持
 */

public class DeleteCharacterData
{
    public string charaName;
    public int uniqueId;
}

public class CharacterManager : MonoBehaviour
{
    /// <summary>
    /// キャラクター情報(ScriptableObject)
    /// </summary>
    [SerializeField]
    private CharacterDataAsset _characterDataAsset;

    [SerializeField]
    private StageInfo _stageInfo;

    [SerializeField]
    public List<GameObject> _prefabCharacter;

    private List<Character> _characterList;

    private MapManager _mapManager;
    private TurnManager _turnManager;
    private EffectManager _effectManager;

    public static CharacterManager instance;

    // 消失したキャラクター名リスト
    private List<string> _deleteCharacterNameList;

    // 消失したキャラクター名リスト
    private List<DeleteCharacterData> _deleteCharacterInfoList;



    private Dictionary<string,int> _characterNameUniqueList;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    void Start()
    {
        _characterList = new List<Character>();
        _mapManager = MapManager.instance;
        _turnManager = TurnManager.instance;
        _effectManager = EffectManager.instance;
        _deleteCharacterNameList = new List<string>();
        _characterNameUniqueList = new Dictionary<string, int>();
        _deleteCharacterInfoList = new List<DeleteCharacterData>();

        // ステージ情報に合わせてキャラクターを生成
        // ★味方用、敵用に分ける
        CreateCharacterInfo();
    }

    void Update()
    {

    }

    ///// <summary>
    ///// 消失キャラ名を取得
    ///// </summary>
    //public List<string> DeleteCharacterNameList
    //{
    //    get
    //    {
    //        return _deleteCharacterNameList;
    //    }
    //}


    public List<DeleteCharacterData> DeleteCharacterDataList
    {
        get
        {
            return _deleteCharacterInfoList;
        }
    }



    /// <summary>
    /// キャラクター生成を行う(キャラクター生成、パラメータ設定)
    /// </summary>
    private void CreateCharacterInfo()
    {
        Character character = null;
        int randomPosX = 0;
        int randomPosZ = 0;

        // ★上位画面から渡されるデータを比較する必要がある
        // 仮としてLEVEL2にする
        StageData.StageLevelInfo inputStageLevel = StageData.StageLevelInfo.STAGE_LEVEL_2;
        StageData baseStageData = null;

        var mapWidthX = DefineMapClass.MAP_WIDTH / 2;
        var mapHeightZ = DefineMapClass.MAP_HEIGHT / 2;


        foreach (StageData stageData in _stageInfo._stageDataList)
        {
            // ★仮レベル2
            if (stageData.StageLevel == inputStageLevel)
            {
                baseStageData = stageData;
            }
        }
        //////////////////ステージによって敵情報を変えることを考えていたけど一旦忘れる/////////////////

        // 味方キャラクターを生成
        CreateCharacter(baseStageData.PlayerCount, true);

        // 敵キャラクターを生成
        CreateCharacter(baseStageData.EnemyCount, false);

        // ゲーム上に存在するキャラクターごとに情報を設定する
        SetParameter();

        // キャラクターを再配置する
        foreach (Character chara in _characterList)
        {
            bool isSuccess = false;

            // キャラクターの初期位置をランダム定義
            if (chara.GetCharactrerData.IsEnemy)
            {
                Debug.Log("Set Enemy Character Position");

                // キャラクター配置が決定するまで繰り返す
                while (!isSuccess)
                {
                    // 敵キャラの場合は、ステージ上半分を初期位置にする
                    randomPosX = Random.Range(-mapWidthX, mapWidthX);
                    randomPosZ = Random.Range(1, mapHeightZ);

                    // キャラクターの配置が可能かチェック
                    if (_mapManager.CheckSettingOnBlock(randomPosX, randomPosZ))
                    {
                        isSuccess = true;
                    }
                }
            }
            else
            {
                Debug.Log("Set My Character Position");

                // キャラクター配置が決定するまで繰り返す
                while (!isSuccess)
                {
                    // 味方キャラの場合は、ステージ下半分を初期位置にする
                    randomPosX = Random.Range(-mapWidthX, mapWidthX);
                    randomPosZ = Random.Range(-mapHeightZ, -1);

                    // キャラクターの配置が可能かチェック
                    if (_mapManager.CheckSettingOnBlock(randomPosX, randomPosZ))
                    {
                        isSuccess = true;
                    }
                }
            }

            // 初期位置設定
            chara.SetInitPosition(randomPosX, randomPosZ);

            // 配置先のマップブロック情報を更新する
            _mapManager.SetUsedMapBlock(chara.GetCharaPosX, chara.GetCharaPosZ, true);

            Debug.Log("キャラクターの配置場所：" + chara.GetCharaPosX + "  " + chara.GetCharaPosZ);
        }

        // ゲーム開始時に存在するキャラクターリストを保持する
        _turnManager.SetCharacterTurnList = _characterList;
        _turnManager.TurnDispInit();
    }

    /// <summary>
    /// 味方キャラクター、敵キャラクターを生成する
    /// </summary>
    /// <param name="charaCnt"></param>
    /// <param name="friendFlg"></param>
    private void CreateCharacter(int charaCnt, bool friendFlg)
    {
        Character character = null;
        var choise = 0;

        // ★一旦、ゴブリン(黄)を味方キャラとする 0
        // ★敵キャラはゴブリン(緑)に固定する     1 
        // 取得したステージ情報に基づいて味方キャラクターを生成
        for (int i = 0; i < charaCnt; i++)
        {
            // 敵キャラ仮決め
            if (!friendFlg)
            {
                choise = Random.Range(0, 2);
            }
            // 出撃キャラ仮決め
            // true : Friemd / false : Enemy
            else
            {
                // min以上、max未満
                choise = Random.Range(2, 4);
            }

            GameObject gameObj = Instantiate(_prefabCharacter[choise],
                          new Vector3(0, 0.5f, 0),
                          Quaternion.identity);

            // インスタンス作成したオブジェクトからCharacterコンポーネントを取得
            character = gameObj.GetComponent<Character>();

            // 敵キャラなら180度回転
            if (!friendFlg)
            {
                character.transform.rotation = Quaternion.Euler(0, 180f, 0);
            }

            if (gameObj != null)
            {
                // オブジェクトにアタッチしたキャラクター情報を保持する
                _characterList.Add(character);
            }
        }
    }

    /// <summary>
    /// AssetDataで管理されたキャラクターのパラメータ情報を設定する
    /// </summary>
    public void SetParameter()
    {
        /*
         * キャラクターID(オブジェクトに割り振っているID)は、ScriptableObjectで管理しているキャラクターIDに対応する
         * ユニークIDは、使用するキャラクターが被ることがあるため、独自のIDを振りなおして管理する
         */


        // 1始まりにする
        var uniqueId = 1;

        // AssetDataに設定された各キャラクター情報を走査
        foreach (CharacterData charaData in _characterDataAsset._characterDataList)
        {
            // ゲーム上に存在するキャラクター情報(Prefab)を走査
            foreach (Character chara in _characterList)
            {
                // 同じIDが見つけて、パラメータを保持(アタッチされたキャラクターごとに持つようになる)
                if (charaData.CharacterID == chara.GetCharaId)
                {
                    Debug.Log("Set Param");
                    // AssetDataを保持
                    chara.GetCharactrerData = charaData;
                    // キャラクターの現在HPを保持
                    chara.CurrentHp = charaData.MaxHP;
                    Debug.Log("現在HPの設定　：" + chara.CurrentHp);


                    //_characterNameUniqueList.Add(charaData.CharacterName, uniqueId);


                    // キャラクター毎にユニークIDを割り振る
                    chara.UniqueId = uniqueId;
                    uniqueId++;
                }
            }
        }
    }

    // 選択したブロックの座標と同じ座標のキャラクターを見つける
    public Character SearchCharacterByPosition(int posX, int posZ)
    {
        //    Debug.Log(_characterList.Count);
        //    Debug.Log("x : " + posX + " z : " + posZ);
        foreach (Character chara in _characterList)
        {
            if ((chara.GetCharaPosX == posX) && (chara.GetCharaPosZ == posZ))
            {
                return chara;
            }
        }

        return null;
    }

    /// <summary>
    /// キャラクターが破壊(HPが0)されたキャラクター名を保持(ターンリストの削除に使用する)
    /// </summary>
    /// <param name="name"></param>
    public void SetDeleteCharacterName(string name)
    {
        _deleteCharacterNameList.Add(name);
    }


    public void SetDeleteCharacterInfo(string name, int id)
    {
        DeleteCharacterData data = new DeleteCharacterData();

        data.charaName = name;
        data.uniqueId = id;

        _deleteCharacterInfoList.Add(data);
    }

    /// <summary>
    /// 死亡時にキャラクターが持つ情報を削除する
    /// </summary>
    /// <param name="character"></param>
    public void DeleteCharacterInfo(Character character)
    {

        // 現在の場所を移動可能にする
        _mapManager.SetUsedMapBlock(character.GetCharaPosX, character.GetCharaPosZ, false);

    }



    public void ShowDeadEffect(Vector3 position, GameObject character)
    {
        _effectManager.ShowCharacterEffect(EffectData.EFFECT.Die, position);


        StartCoroutine(DestroyCharacter(3.0f, character));
    }

    IEnumerator DestroyCharacter(float waitTime, GameObject obj)
    {
        // 指定時間待ち
        yield return new WaitForSeconds(waitTime);

        Destroy(obj);
    }

    public Character SearchUniqueIdCharacter(int uniqueId)
    {
        // ゲーム上に存在するキャラクター情報(Prefab)を走査
        foreach (Character chara in _characterList)
        {
            // ユニークIDキャラクターを返す
            if (uniqueId == chara.UniqueId)
            {
                return chara;
            }

        }

        return null;
    }

    


}
