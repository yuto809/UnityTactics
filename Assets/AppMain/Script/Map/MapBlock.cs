
using UnityEngine;
using BlockModeStatus;

/// <summary>
/// ブロック自身に関わる処理を行うスクリプト(マップの初期化や作成はMapManager.cs側で行う)
/// </summary>
public class MapBlock : MonoBehaviour
{
    /// <summary>
    /// 選択しているブロックの子オブジェクトを操作する変数
    /// </summary>
    private GameObject _selectBlock;

    private Renderer _modeBlockRenderer;

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

    /// <summary>
    /// true:移動不可
    /// </summary>
    [SerializeField]
    private bool _isMovable;

    /// <summary>
    /// ブロックの座標xを返す
    /// </summary>
    public int BlockPosX => _blockPosX;

    /// <summary>
    /// ブロックの座標zを返す
    /// </summary>
    public int BlockPosZ => _blockPosZ;

    /// <summary>
    /// 移動可能なブロックかどうか判断フラグを返す
    /// </summary>
    public bool IsMoveable => _isMovable;

    /// <summary>
    /// 既にブロック状にキャラクターが存在しているかどうかのフラグ
    /// </summary>
    public bool IsExistCharacter => _isExistCharaFlg;

    // Start is called before the first frame update
    private void Start()
    {
        // 子オブジェクト(強調ブロック)を非表示にしておく
        _selectBlock = transform.GetChild(0).gameObject;

        // マップブロックを指定のモードに変更するためのコンポーネントを取得
        _modeBlockRenderer = _selectBlock.GetComponent<Renderer>();

        // MapBlock.csは全てのブロックに適用されるため全て非表示
        _selectBlock.SetActive(false);
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
    // collider判定はEditor側で無効にしている
    public void EmphasizeBlock(SelectBlockMode blockMode)
    {
        switch(blockMode)
        {
            case SelectBlockMode.OFF:
                _selectBlock.SetActive(false);
                break;

            case SelectBlockMode.SELECT_BLOCK:
                _modeBlockRenderer.material = _selectAreaMaterial;
                _selectBlock.SetActive(true);
                break;
            case SelectBlockMode.SELECT_MOVE:
                _modeBlockRenderer.material = _moveAreaMaterial;
                //_selectBlock.GetComponent<Renderer>().material = _moveAreaMaterial;
                _selectBlock.SetActive(true);
                break;
            case SelectBlockMode.SELECT_ATTACK:
                _modeBlockRenderer.material = _atkAreaMaterial;
                //_selectBlock.GetComponent<Renderer>().material = _atkAreaMaterial;
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
    /// キャラクターが移動可能なブロックか確認
    /// </summary>
    /// <returns>true : 移動可能 false : 移動不可</returns>
    public bool CheckMovableBlock()
    {
        // ★ブロックが使用済みとしたい場合はここで管理
        return !_isExistCharaFlg && !_isMovable;
    }

}
