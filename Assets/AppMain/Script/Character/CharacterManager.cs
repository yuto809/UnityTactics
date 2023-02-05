using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeletedCharacter
{
    public string CharaName;
    public int UniqueId;
}

/// <summary>
/// キャラクターの生成
/// キャラクター生成時のパラメータ設定
/// 破壊されたキャラクター名の保持
/// </summary>
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
    //private Dictionary<int, Character> _characterDictionary;
    private int _uniqueCharacterId;

    //private MapManager _mapManager;
    //private TurnManager _turnManager;
    //private EffectManager _effectManager;

    public static CharacterManager Instance;

    // 消失したキャラクター名リスト
    private List<string> _deleteCharacterNameList;

    // 消失したキャラクター名リスト
    private List<DeletedCharacter> _deletedCharacterList;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    private void Start()
    {
        _characterList = new List<Character>();
        //_mapManager = MapManager.instance;
        //_turnManager = TurnManager.instance;
        //_effectManager = EffectManager.Instance;
        _deleteCharacterNameList = new List<string>();
        _deletedCharacterList = new List<DeletedCharacter>();

        //_characterDictionary = new Dictionary<int, Character>();
        _uniqueCharacterId = 1;


        // ステージ情報に合わせてキャラクターを生成
        // ★味方用、敵用に分ける
        CreateCharacterInfo();
    }

    public List<DeletedCharacter> DeletedCharacterDataList
    {
        get
        {
            return _deletedCharacterList;
        }
    }

    /// <summary>
    /// キャラクター生成を行う(キャラクター生成、パラメータ設定)
    /// </summary>
    private void CreateCharacterInfo()
    {
        int randomPosX = 0;
        int randomPosZ = 0;

        const int mapWidthX = MapInfo.MAP_WIDTH / 2;
        const int mapHeightZ = MapInfo.MAP_HEIGHT / 2;

        StageData baseStageData = null;

        // 現在のステージ情報を取得
        baseStageData = GetStageInfo();

        //////////////////ステージによって敵情報を変えることを考えていたけど一旦忘れる/////////////////

        // 味方キャラクターを生成
        CreateCharacter(baseStageData.PlayerCount, true);

        // 敵キャラクターを生成
        CreateCharacter(baseStageData.EnemyCount, false);

        // ゲーム上に存在するキャラクターごとに情報を設定する
        //SetParameter();

        // キャラクターを再配置する
        foreach (Character chara in _characterList)
        {
            bool isSuccess = false;

            // キャラクターの初期位置をランダム定義
            if (chara.GetCharacterData.IsEnemy)
            {
                Debug.Log("Set Enemy Character Position");

                // キャラクター配置が決定するまで繰り返す
                while (!isSuccess)
                {
                    // 敵キャラの場合は、ステージ上半分を初期位置にする
                    randomPosX = Random.Range(-mapWidthX, mapWidthX);
                    randomPosZ = Random.Range(1, mapHeightZ);

                    // キャラクターの配置が可能かチェック
                    if (MapManager.Instance.IsSettingOnBlock(randomPosX, randomPosZ))
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
                    if (MapManager.Instance.IsSettingOnBlock(randomPosX, randomPosZ))
                    {
                        isSuccess = true;
                    }
                }
            }

            // 初期位置設定
            chara.SetInitPosition(randomPosX, randomPosZ);

            // 配置先のマップブロック情報を更新する
            MapManager.Instance.SetUsedMapBlock(chara.PositionX, chara.PositionZ, true);

            Debug.Log("キャラクターの配置場所：" + chara.PositionX + "  " + chara.PositionZ);
        }

        // ゲーム開始時に存在するキャラクターリストを保持する
        TurnManager.Instance.SetCharacterTurnList = _characterList;
        TurnManager.Instance.TurnDispInit();
    }

    /// <summary>
    /// 現在のステージ情報を取得する
    /// </summary>
    private StageData GetStageInfo()
    {
        // ★上位画面から渡されるデータを比較する必要がある
        // 仮としてLEVEL2にする
        StageData.StageLevelInfo inputStageLevel = StageData.StageLevelInfo.STAGE_LEVEL_2;
        StageData baseStageData = null;

        foreach (StageData stageData in _stageInfo._stageDataList)
        {
            // ★仮レベル2
            if (stageData.StageLevel == inputStageLevel)
            {
                baseStageData = stageData;
            }
        }

        if (baseStageData == null)
        {
            // ステージ情報が見つからなかった場合はステージレベル1の情報を設定する
            baseStageData.SetDefaultStageData();
        }

        return baseStageData;
    }


    /// <summary>
    /// 味方キャラクター、敵キャラクターを生成する
    /// </summary>
    /// <param name="availableCharacter">生成数</param>
    /// <param name="isCharacterType"> true : 味方 / false : 敵</param>
    private void CreateCharacter(int availableCharacter, bool isCharacterType)
    {
        //Character character = null;
        var choice = 0;

        // ★一旦、ゴブリン(黄)を味方キャラとする 0
        // ★敵キャラはゴブリン(緑)に固定する     1 
        // 取得したステージ情報に基づいて味方キャラクターを生成
        for (int i = 0; i < availableCharacter; i++)
        {
            // 敵キャラ仮決め
            if (!isCharacterType)
            {
                choice = Random.Range(0, 2);
            }
            // 出撃キャラ仮決め
            else
            {
                // min以上、max未満
                choice = Random.Range(2, 4);
            }

            GameObject gameObj = Instantiate(_prefabCharacter[choice],
                          new Vector3(0, 0.5f, 0),
                          Quaternion.identity);

            // インスタンス作成したオブジェクトからCharacterコンポーネントを取得
            var character = gameObj.GetComponent<Character>();

            // 敵キャラなら180度回転
            if (!isCharacterType)
            {
                character.transform.rotation = Quaternion.Euler(0, 180f, 0);
            }

            if (gameObj != null)
            {
                Debug.Log("Set Param");
                // AssetDataで管理されたキャラクターのパラメータ情報を設定する
                character.GetCharacterData = _characterDataAsset.GetCharacterDataById(character.GetCharaId);
                // キャラクターの現在HPを保持
                character.CurrentHp = character.GetCharacterData.MaxHP;
                Debug.Log("現在HPの設定　：" + character.CurrentHp);
                // キャラクターID(オブジェクトに割り振っているID)は、ScriptableObjectで管理しているキャラクターIDに対応する
                // ユニークIDは、使用するキャラクターが被ることがあるため、独自のIDを振りなおして管理する
                character.UniqueId = _uniqueCharacterId;
                _uniqueCharacterId++;

                // オブジェクトにアタッチしたキャラクター情報を保持する
                _characterList.Add(character);
            }
        }
    }

    /// <summary>
    /// AssetDataで管理されたキャラクターのパラメータ情報を設定する
    /// </summary>
    //private void SetParameter()
    //{
    //    /*
    //     * キャラクターID(オブジェクトに割り振っているID)は、ScriptableObjectで管理しているキャラクターIDに対応する
    //     * ユニークIDは、使用するキャラクターが被ることがあるため、独自のIDを振りなおして管理する
    //     */


    //    // 1始まりにする
    //    var uniqueId = 1;

    //    // AssetDataに設定された各キャラクター情報を走査
    //    // AssetだからIDは必ず連番
    //    foreach (CharacterData charaData in _characterDataAsset._characterDataList)
    //    {
    //        // ゲーム上に存在するキャラクター情報(Prefab)を走査
    //        // ランダム生成されたリストだからIDは被っている可能性がある
    //        foreach (Character chara in _characterList)
    //        {
    //            // 同じIDが見つけて、パラメータを保持(アタッチされたキャラクターごとに持つようになる)
    //            if (charaData.CharacterID == chara.GetCharaId)
    //            {
    //                Debug.Log("Set Param");
    //                // AssetDataを保持
    //                chara.GetCharacterData = charaData;
    //                // キャラクターの現在HPを保持
    //                chara.CurrentHp = charaData.MaxHP;
    //                Debug.Log("現在HPの設定　：" + chara.CurrentHp);


    //                //_characterNameUniqueList.Add(charaData.CharacterName, uniqueId);


    //                // キャラクター毎にユニークIDを割り振る
    //                chara.UniqueId = uniqueId;
    //                uniqueId++;
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 選択したブロックを基にキャラクターを見つける
    /// </summary>
    /// <param name="mapBlock"></param>
    /// <returns></returns>
    public Character SearchCharacterByPosition(MapBlock mapBlock)
    {
        return SearchCharacterByPosition(mapBlock.BlockPosX, mapBlock.BlockPosZ);
    }

    /// <summary>
    /// 選択したブロックの座標と同じ座標のキャラクターを見つける
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posZ"></param>
    /// <returns></returns>
    public Character SearchCharacterByPosition(int posX, int posZ)
    {
        //    Debug.Log(_characterList.Count);
        //    Debug.Log("x : " + posX + " z : " + posZ);
        foreach (Character chara in _characterList)
        {
            if (chara.PositionX == posX && chara.PositionZ == posZ)
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
        DeletedCharacter data = new DeletedCharacter();

        data.CharaName = name;
        data.UniqueId = id;

        _deletedCharacterList.Add(data);
    }

    /// <summary>
    /// 死亡時にキャラクターが持つ情報を削除する
    /// </summary>
    /// <param name="character"></param>
    public void DeleteCharacterInfo(Character character)
    {

        // 現在の場所を移動可能にする
        MapManager.Instance.SetUsedMapBlock(character.PositionX, character.PositionZ, false);

    }



    public void ShowDeadEffect(Vector3 position, GameObject character)
    {
        EffectManager.Instance.ShowCharacterEffect(EffectData.EFFECT.Die, position);


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
