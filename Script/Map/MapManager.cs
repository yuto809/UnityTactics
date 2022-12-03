using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    /// <summary>
    /// ブロック生成時に使用するブロック(prefab)
    /// </summary>
    [SerializeField]
    private GameObject _blockFlowerGlass;

    /// <summary>
    /// ブロック生成時に使用するブロック(移動不可)
    /// </summary>
    [SerializeField]
    private GameObject _notMoveBlock;

    /// <summary>
    /// ブロックの座標情報を2次元配列で保持
    /// </summary>
    private MapBlock[,] _mapBlocks;

    public static MapManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
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
        _mapBlocks = new MapBlock[DefineMapClass.MAP_WIDTH, DefineMapClass.MAP_HEIGHT];
    }

    /// <summary>
    /// ブロックを自動生成
    /// </summary>
    private void CreateMap()
    {
        Vector3 blockPos = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject gameObj;
        MapBlock block;

        // (-x,-z)地点から(x，z)までマップを作成する
        blockPos.x = -(DefineMapClass.MAP_WIDTH / 2);
        blockPos.z = -(DefineMapClass.MAP_HEIGHT / 2);

        for (int i = 0; i < DefineMapClass.MAP_WIDTH; i++)
        {
            for (int j = 0; j < DefineMapClass.MAP_HEIGHT; j++)
            {
                pos = blockPos;
                pos.x += i;
                pos.z += j;

                var rate = Random.Range(0.0f, 100.0f);

                if (rate > DefineMapClass.RATE_NOT_MOVE_BLOCK)
                {
                    gameObj = Instantiate(_notMoveBlock, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    gameObj = Instantiate(_blockFlowerGlass, Vector3.zero, Quaternion.identity);
                }

                gameObj.transform.position = pos;

                block = gameObj.GetComponent<MapBlock>();

                // 生成したブロック情報を保持
                // ブロック自身が持つ情報を2次元配列で管理している(blockには例えば(-1,-1)といった位置情報を持ったブロックが配列で管理される)
                _mapBlocks[i, j] = block;

                // 配置されたブロックの座標を設定する
                _mapBlocks[i, j].SetBlockPosition((int)pos.x, (int)pos.z);

                // ブロック生成時はブロック状のキャラクター存在フラグを初期化
                _mapBlocks[i, j].SetExistCharacterFlg(false);


            }
        }
    }

    /// <summary>
    /// 指定先のブロック上に配置可能か判断
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>true 使用可能 false 使用不可</returns>
    public bool CheckSettingOnBlock(int blockPosX, int blockPosZ)
    {
        int baseIndexX = 0;
        int baseIndexZ = 0;

        // 使用状態を設定するための2次元配列のidxを取得する
        FindBaseIndex(blockPosX, blockPosZ, out baseIndexX, out baseIndexZ);

        // 既に指定先のブロックが使用済みの場合
        if (!_mapBlocks[baseIndexX, baseIndexZ].CheckUsedBlock())
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 任意のマップブロックの使用状態を設定する
    /// </summary>
    /// <param name="blockPosX"></param>
    /// <param name="blockPosZ"></param>
    /// <param name="flg"></param>
    public void SetUsedMapBlock(int blockPosX, int blockPosZ, bool flg)
    {
        int baseIndexX = 0;
        int baseIndexZ = 0;

        Debug.Log("SetUsedMapBlock " + " X : " + blockPosX + " Z : " + blockPosZ);

        // 使用状態を設定するための2次元配列のidxを取得する
        FindBaseIndex(blockPosX, blockPosZ, out baseIndexX, out baseIndexZ);

        // 使用状態を設定(キャラの有無)
        _mapBlocks[baseIndexX, baseIndexZ].SetExistCharacterFlg(flg);
    }

    /// <summary>
    /// 全てのブロック状の強調ブロックを非表示にする
    /// </summary>
    public void ClearSelectBlockMode()
    {
        for (int i = 0; i < DefineMapClass.MAP_WIDTH; i++)
        {
            for (int j = 0; j < DefineMapClass.MAP_HEIGHT; j++)
            {
                _mapBlocks[i, j].EnphasizeBlock(DefineMapClass.SelectBlockMode.OFF);
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

        for (int i = 0; i < DefineMapClass.MAP_WIDTH; i++)
        {
            for (int j = 0; j < DefineMapClass.MAP_HEIGHT; j++)
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

    //未使用
    /// <summary>
    /// キャラクターの周囲1マスを取得する
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public List<MapBlock> SearchAroundAreaByCharacter(Character character)
    {
        // 周囲マスを管理するブロックリスト
        List<MapBlock> aroundAreaList = new List<MapBlock>();

        int baseIndexX;
        int baseIndexZ;

        // キャラクターの座標情報と一致する2次元配列のidxを取得する
        FindBaseIndex(character.GetCharaPosX, character.GetCharaPosZ, out baseIndexX, out baseIndexZ);

        // ベース位置から周囲1マスを取得する
        // 上
        if ((baseIndexZ + 1) < DefineMapClass.MAP_HEIGHT)
        {
            aroundAreaList.Add(_mapBlocks[baseIndexX, baseIndexZ + 1]);
        }
        //// 斜め右上
        //if (((baseIndexX + 1) < DefineMapClass.MAP_WIDTH) &&
        //    ((baseIndexZ + 1) < DefineMapClass.MAP_HEIGHT))
        //{
        //    aroundAreaList.Add(_mapBlocks[baseIndexX + 1, baseIndexZ + 1]);
        //}
        // 右
        if ((baseIndexX + 1) < DefineMapClass.MAP_WIDTH)
        {
            aroundAreaList.Add(_mapBlocks[baseIndexX + 1, baseIndexZ]);
        }
        //// 斜め右下
        //if (((baseIndexX + 1) < DefineMapClass.MAP_WIDTH) &&
        //    ((baseIndexZ - 1) >= 0))
        //{
        //    aroundAreaList.Add(_mapBlocks[baseIndexX + 1, baseIndexZ - 1]);
        //}
        // 下
        if ((baseIndexZ - 1) >= 0)
        {
            aroundAreaList.Add(_mapBlocks[baseIndexX, baseIndexZ - 1]);
        }
        //// 斜め左下
        //if (((baseIndexX - 1) >= 0) &&
        //    ((baseIndexZ - 1) >= 0))
        //{
        //    aroundAreaList.Add(_mapBlocks[baseIndexX - 1, baseIndexZ - 1]);
        //}
        // 左
        if ((baseIndexX - 1) >= 0)
        {
            aroundAreaList.Add(_mapBlocks[baseIndexX - 1, baseIndexZ]);
        }
        //// 斜め左上
        //if (((baseIndexX - 1) >= 0) &&
        //    ((baseIndexZ + 1) < DefineMapClass.MAP_HEIGHT))
        //{
        //    aroundAreaList.Add(_mapBlocks[baseIndexX - 1, baseIndexZ + 1]);
        //}

        return null;
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

        FindBaseIndex(character.GetCharaPosX, character.GetCharaPosZ, out baseIndexX, out baseIndexZ);

        // ベース位置からキャラクターが持つ移動力分だけ移動可能エリアを探す
        // 上方向
        for (int z = baseIndexZ; z < DefineMapClass.MAP_HEIGHT; z++)
        {
            // 移動力分調査したら終了
            if (moveCnt > character.GetCharactrerData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexZ != z)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[baseIndexX, z].IsExistCharacter) && (!_mapBlocks[baseIndexX,z].IsMoveFlg))
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
        for (int x = baseIndexX; x < DefineMapClass.MAP_WIDTH; x++)
        {
            // 移動力分調査したら終了
            if (moveCnt > character.GetCharactrerData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexX != x)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[x, baseIndexZ].IsExistCharacter) && (!_mapBlocks[x, baseIndexZ].IsMoveFlg))
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
            if (moveCnt > character.GetCharactrerData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexZ != z)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[baseIndexX, z].IsExistCharacter) && (!_mapBlocks[baseIndexX, z].IsMoveFlg))
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
            if (moveCnt > character.GetCharactrerData.Move)
            {
                break;
            }

            // キャラクターの現在地は移動先に含めない
            if (baseIndexX != x)
            {
                // 移動先にキャラが存在していない、移動不可でない
                if ((!_mapBlocks[x, baseIndexZ].IsExistCharacter) && (!_mapBlocks[x, baseIndexZ].IsMoveFlg))
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
    public List<MapBlock> SearchAttackAreaByCharacter(Character character, int characterPosX, int characterPosZ)
    {
        List<MapBlock> atkAllowList = new List<MapBlock>();

        int baseIndexX;
        int baseIndexZ;

        FindBaseIndex(characterPosX, characterPosZ, out baseIndexX, out baseIndexZ);

        Debug.Log("baseIndexX : " + baseIndexX + " baseIndexZ : " + baseIndexZ);

        // ベース位置から周囲1マスを攻撃範囲とする　(仮)
        // 範囲外でないか確認する
        // 上
        if ((baseIndexZ + 1) < DefineMapClass.MAP_HEIGHT)
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
        if ((baseIndexX + 1) < DefineMapClass.MAP_WIDTH)
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
