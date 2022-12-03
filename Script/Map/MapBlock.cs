using UnityEngine;

/*
 * MapBlock.csはブロック自身に関わる処理を行うスクリプト
 * マップの初期化や作成はMapManager.cs側で行う
 */


public class MapBlock : MonoBehaviour
{
    /// <summary>
    /// 選択しているブロックの子オブジェクトを操作する変数
    /// </summary>
    private GameObject _selectBlock;
    /// <summary>
    /// 生成されたブロックのx座標
    /// </summary>
    private int _blockPosX;
    /// <summary>
    /// 生成されたブロックのz座標
    /// </summary>
    private int _blockPosZ;
    /// <summary>
    /// キャラクター存在フラグ
    /// </summary>
    private bool _isExistCharaFlg;

    [SerializeField]
    private Material _selectAreaMaterial;
    [SerializeField]
    private Material _moveAreaMaterial;
    [SerializeField]
    private Material _atkAreaMaterial;
    [SerializeField]
    private bool _isMoveFlg;

    /// <summary>
    /// ブロックの座標xを返す
    /// </summary>
    public int BlockPosX
    {
        get
        {
            return _blockPosX;
        }
    }

    /// <summary>
    /// ブロックの座標zを返す
    /// </summary>
    public int BlockPosZ
    {
        get
        {
            return _blockPosZ;
        }
    }

    /// <summary>
    /// 移動可能なブロックかどうか判断フラグを返す
    /// </summary>
    public bool IsMoveFlg
    {
        get
        {
            return _isMoveFlg;
        }
    }

    /// <summary>
    /// 既にブロック状にキャラクターが存在しているかどうかのフラグ
    /// </summary>
    public bool IsExistCharacter
    {
        get
        {
            return _isExistCharaFlg;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 子オブジェクト(強調ブロック)を非表示にしておく
        _selectBlock = transform.GetChild(0).gameObject;

        // MapBlock.csは全てのブロックに適用されるため全て非表示
        _selectBlock.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 配置されたブロックの座標を設定する
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SetBlockPosition(int x, int z)
    {
        _blockPosX = x;
        _blockPosZ = z;

        //Debug.Log("x座標：" + _blockPosX + " z座標：" + _blockPosZ);
    }

    // 指定したモードでブロックを強調する
    // collider判定はEditer側で無効にしている
    public void EnphasizeBlock(DefineMapClass.SelectBlockMode blockMode)
    {
        switch(blockMode)
        {
            case DefineMapClass.SelectBlockMode.OFF:
                _selectBlock.SetActive(false);
                break;

            case DefineMapClass.SelectBlockMode.SELECT_BLOCK:
                _selectBlock.GetComponent<Renderer>().material = _selectAreaMaterial;
                _selectBlock.SetActive(true);
                break;
            case DefineMapClass.SelectBlockMode.SELECT_MOVE:
                _selectBlock.GetComponent<Renderer>().material = _moveAreaMaterial;
                _selectBlock.SetActive(true);
                break;
            case DefineMapClass.SelectBlockMode.SELECT_ATTACK:
                _selectBlock.GetComponent<Renderer>().material = _atkAreaMaterial;
                _selectBlock.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// ブロック上にキャラクターが存在するかどうかを設定する
    /// </summary>
    /// <param name="flg"></param>
    public void SetExistCharacterFlg(bool flg)
    {
        _isExistCharaFlg = flg;
    }

    /// <summary>
    /// 未使用ブロックか判断
    /// </summary>
    /// <returns>true : 未使用 false : 使用済み</returns>
    public bool CheckUsedBlock()
    {
        // ★ブロックが使用済みとしたい場合はここで管理
        if (_isExistCharaFlg || _isMoveFlg)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}
