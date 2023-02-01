using System.Collections.Generic;
using UnityEngine;
using BlockModeStatus;

public class MapManager : MonoBehaviour
{
    /// <summary>
    /// ブロック生成時に使用するブロック(prefab)
    /// </summary>
    [SerializeField]
    private GameObject _prefabBlockFlower;

    /// <summary>
    /// ブロック生成時に使用するブロック(移動不可)
    /// </summary>
    [SerializeField]
    private GameObject _prefabNotMoveBlock;

    /// <summary>
    /// ブロックの座標情報を2次元配列で保持
    /// </summary>
    private MapBlock[,] _mapBlocks;

    public static MapManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // 2次元配列初期化
        InitializeMapSize();
        
        // ブロックを生成
        CreateMap();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 内部の2次元配列を初期化
    /// </summary>
    private void InitializeMapSize()
    {
        // [横：縦]
        _mapBlocks = new MapBlock[MapInfo.MAP_WIDTH, MapInfo.MAP_HEIGHT];
    }

    /// <summary>
    /// (-<see cref="DefineMapClass.MAP_WIDTH"/>, -<see cref="DefineMapClass.MAP_HEIGHT"/>)地点から
    /// (<see cref="DefineMapClass.MAP_WIDTH"/>, <see cref="DefineMapClass.MAP_HEIGHT"/>)までのマップブロックを自動生成する
    /// </summary>
    private void CreateMap()
    {
        for (var i = 0; i < MapInfo.MAP_WIDTH; i++)
        {
            for (var j = 0; j < MapInfo.MAP_HEIGHT; j++)
            {
                var pos = new Vector3(-(MapInfo.MAP_WIDTH / 2) + i, 0.0f, -(MapInfo.MAP_HEIGHT / 2) + j);

                var gameObj = Instantiate((Random.Range(0.0f, 100.0f) > MapInfo.RATE_NOT_MOVE_BLOCK ? _prefabNotMoveBlock : _prefabBlockFlower), pos, Quaternion.identity);

                // 生成したブロック情報を保持
                // ブロック自身が持つ情報を2次元配列で管理している(blockには例えば(-1,-1)といった位置情報を持ったブロックが配列で管理される)
                _mapBlocks[i, j] = gameObj.GetComponent<MapBlock>();

                // 配置されたブロックの座標を設定する
                _mapBlocks[i, j].SetBlockPosition((int)pos.x, (int)pos.z);

                // ブロック生成時はブロック状のキャラクター存在フラグを初期化
                _mapBlocks[i, j].SetExistCharacterFlg(false);


            }
        }
    }
    //private void CreateMap()
    //{
    //    var blockPos = Vector3.zero;
    //    var pos = Vector3.zero;
    //    GameObject gameObj;
    //    MapBlock block;

    //    // (-x,-z)地点から(x，z)までマップを作成する
    //    blockPos.x = -(DefineMapClass.MAP_WIDTH / 2);
    //    blockPos.z = -(DefineMapClass.MAP_HEIGHT / 2);

    //    for (int i = 0; i < DefineMapClass.MAP_WIDTH; i++)
    //    {
    //        for (int j = 0; j < DefineMapClass.MAP_HEIGHT; j++)
    //        {
    //            pos = blockPos;
    //            pos.x += i;
    //            pos.z += j;

    //            var rate = Random.Range(0.0f, 100.0f);

    //            if (rate > DefineMapClass.RATE_NOT_MOVE_BLOCK)
    //            {
    //                gameObj = Instantiate(_prefabNotMoveBlock, Vector3.zero, Quaternion.identity);
    //            }
    //            else
    //            {
    //                gameObj = Instantiate(_prefabBlockFlower, Vector3.zero, Quaternion.identity);
    //            }

    //            gameObj.transform.position = pos;

    //            block = gameObj.GetComponent<MapBlock>();

    //            // 生成したブロック情報を保持
    //            // ブロック自身が持つ情報を2次元配列で管理している(blockには例えば(-1,-1)といった位置情報を持ったブロックが配列で管理される)
    //            _mapBlocks[i, j] = block;

    //            // 配置されたブロックの座標を設定する
    //            _mapBlocks[i, j].SetBlockPosition((int)pos.x, (int)pos.z);

    //            // ブロック生成時はブロック状のキャラクター存在フラグを初期化
    //            _mapBlocks[i, j].SetExistCharacterFlg(false);


    //        }
    //    }
    //}


    /// <summary>
    /// 指定先のブロック上に配置可能か判断
    /// </summary>
    /// <param name="blockPosX"></param>
    /// <param name="blockPosZ"></param>
    /// <returns>true 使用可能 false 使用不可</returns>
    public bool IsSettingOnBlock(int blockPosX, int blockPosZ)
    {
        // 使用状態を設定するための2次元配列のidxを取得する
        FindBaseIndex(blockPosX, blockPosZ, out var baseIndexX, out var baseIndexZ);

        // 既に指定先のブロックが使用済みの場合
        return _mapBlocks[baseIndexX, baseIndexZ].CheckMovableBlock();
    }

    /// <summary>
    /// 任意のマップブロックの使用状態を設定する
    /// </summary>
    /// <param name="blockPosX"></param>
    /// <param name="blockPosZ"></param>
    /// <param name="flg"></param>
    public void SetUsedMapBlock(int blockPosX, int blockPosZ, bool isUsedBlock)
    {
        Debug.Log("SetUsedMapBlock " + " X : " + blockPosX + " Z : " + blockPosZ);

        // 使用状態を設定するための2次元配列のidxを取得する
        FindBaseIndex(blockPosX, blockPosZ, out var baseIndexX, out var baseIndexZ);

        // 使用状態を設定(キャラの有無)
        _mapBlocks[baseIndexX, baseIndexZ].SetExistCharacterFlg(isUsedBlock);
    }

    /// <summary>
    /// 全てのブロック状の強調ブロックを非表示にする
    /// </summary>
    public void ClearSelectBlockMode()
    {
        for (int i = 0; i < MapInfo.MAP_WIDTH; i++)
        {
            for (int j = 0; j < MapInfo.MAP_HEIGHT; j++)
            {
                _mapBlocks[i, j].EmphasizeBlock(SelectBlockMode.OFF);
            }
        }
    }


    /// <summary>
    /// キャラクターの座標情報と一致する2次元配列のidxを取得する
    /// キャラクター座標から2次元配列のIndex変換
    /// </summary>
    /// <param name="charaPosX">キャラのx座標</param>
    /// <param name="charaPosZ">キャラのz座標</param>
    /// <param name="baseIndexX">キャラの座標情報と一致した配列のidxX</param>
    /// <param name="baseIndexZ">キャラの座標情報と一致した配列のidxZ</param>
    private void FindBaseIndex(int charaPosX, int charaPosZ, out int baseIndexX, out int baseIndexZ)
    {
        baseIndexX = -1;
        baseIndexZ = -1;

        for (int i = 0; i < MapInfo.MAP_WIDTH; i++)
        {
            for (int j = 0; j < MapInfo.MAP_HEIGHT; j++)
            {
                // 2次元配列で保持しているブロックの座標とキャラクターの座標が一致しているときの
                // 配列のインデックスを取得する
                if ((_mapBlocks[i, j].BlockPosX == charaPosX) &&
                    (_mapBlocks[i, j].BlockPosZ == charaPosZ))
                {
                    baseIndexX = i;
                    baseIndexZ = j;
                    break;
                }
            }

            // インデックスを取得しているならループから抜ける
            if ((baseIndexX >= 0) || (baseIndexZ >= 0))
            {
                break;
            }
        }
    }

    /// <summary>
    /// キャラクターのmoveぶん、移動範囲を取得する
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public List<MapBlock> SearchMoveAreaByCharacter(Character character)
    {
        // キャラクターの移動可能ブロックリスト
        List<MapBlock> moveAllowList = new List<MapBlock>();
        int moveCnt = 0;

        int baseIndexX;
        int baseIndexZ;

        FindBaseIndex(character.PositionX, character.PositionZ, out baseIndexX, out baseIndexZ);

        // ベース位置からキャラクターが持つ移動力分だけ移動可能エリアを探す
        // 上方向
        for (int z = baseIndexZ; z < MapInfo.MAP_HEIGHT; z++)
        {
            // 移動力分調査したら終了
            if (moveCnt > character.GetCharacterData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexZ != z)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[baseIndexX, z].IsExistCharacter) && (!_mapBlocks[baseIndexX,z].IsMoveable))
                {
                    Debug.Log(_mapBlocks[baseIndexX, z]);
                    moveAllowList.Add(_mapBlocks[baseIndexX, z]);
                }
                else
                {
                    break;
                }
            }

            moveCnt++;
        }

        moveCnt = 0;

        // 右方向
        for (int x = baseIndexX; x < MapInfo.MAP_WIDTH; x++)
        {
            // 移動力分調査したら終了
            if (moveCnt > character.GetCharacterData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexX != x)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[x, baseIndexZ].IsExistCharacter) && (!_mapBlocks[x, baseIndexZ].IsMoveable))
                {
                    moveAllowList.Add(_mapBlocks[x, baseIndexZ]);
                }
                else
                {
                    break;
                }
            }

            moveCnt++;
        }

        moveCnt = 0;

        // 下方向
        for (int z = baseIndexZ; z >= 0; z--)
        {
            // 移動力分調査したら終了
            if (moveCnt > character.GetCharacterData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexZ != z)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[baseIndexX, z].IsExistCharacter) && (!_mapBlocks[baseIndexX, z].IsMoveable))
                {
                    moveAllowList.Add(_mapBlocks[baseIndexX, z]);
                }
                else
                {
                    break;
                }
            }


            moveCnt++;
        }

        moveCnt = 0;

        // 左方向
        for (int x = baseIndexX; x >= 0; x--)
        {
            // 移動力分調査したら終了
            if (moveCnt > character.GetCharacterData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexX != x)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[x, baseIndexZ].IsExistCharacter) && (!_mapBlocks[x, baseIndexZ].IsMoveable))
                {
                    moveAllowList.Add(_mapBlocks[x, baseIndexZ]);
                }
                else
                {
                    break;
                }
            }

            moveCnt++;
        }

        return moveAllowList;
    }


    // キャラクターの攻撃範囲を取得する
    public List<MapBlock> SearchAttackAreaByCharacter(int characterPosX, int characterPosZ)
    {
        List<MapBlock> atkAllowList = new List<MapBlock>();

        int baseIndexX;
        int baseIndexZ;

        FindBaseIndex(characterPosX, characterPosZ, out baseIndexX, out baseIndexZ);

        Debug.Log("baseIndexX : " + baseIndexX + " baseIndexZ : " + baseIndexZ);

        // ベース位置から周囲1マスを攻撃範囲とする　(仮)
        // 範囲外でないか確認する
        // 上
        if ((baseIndexZ + 1) < MapInfo.MAP_HEIGHT)
        {
            atkAllowList.Add(_mapBlocks[baseIndexX, baseIndexZ + 1]);
        }
        //// 斜め右上
        //if (((baseIndexX + 1) < DefineMapClass.MAP_WIDTH) &&
        //    ((baseIndexZ + 1) < DefineMapClass.MAP_HEIGHT))
        //{
        //    atkAllowList.Add(_mapBlocks[baseIndexX + 1, baseIndexZ + 1]);
        //}
        // 右
        if ((baseIndexX + 1) < MapInfo.MAP_WIDTH)
        {
            atkAllowList.Add(_mapBlocks[baseIndexX + 1, baseIndexZ]);
        }
        //// 斜め右下
        //if (((baseIndexX + 1) < DefineMapClass.MAP_WIDTH) &&
        //    ((baseIndexZ - 1) >= 0))
        //{
        //    atkAllowList.Add(_mapBlocks[baseIndexX + 1, baseIndexZ - 1]);
        //}
        // 下
        if ((baseIndexZ - 1) >= 0)
        {
            atkAllowList.Add(_mapBlocks[baseIndexX, baseIndexZ - 1]);
        }
        //// 斜め左下
        //if (((baseIndexX - 1) >= 0) &&
        //    ((baseIndexZ - 1) >= 0))
        //{
        //    atkAllowList.Add(_mapBlocks[baseIndexX - 1, baseIndexZ - 1]);
        //}
        // 左
        if ((baseIndexX - 1) >= 0)
        {
            atkAllowList.Add(_mapBlocks[baseIndexX - 1, baseIndexZ]);
        }
        //// 斜め左上
        //if (((baseIndexX - 1) >= 0) &&
        //    ((baseIndexZ + 1) < DefineMapClass.MAP_HEIGHT))
        //{
        //    atkAllowList.Add(_mapBlocks[baseIndexX - 1, baseIndexZ + 1]);
        //}

        // 指定キャラクターが攻撃可能なブロックをリストで返す
        return atkAllowList;
    }
}
