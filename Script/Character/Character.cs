using UnityEngine;
using DG.Tweening;
using UniRx;

public class Character : MonoBehaviour
{
    private Animator _animator;

    /// <summary>
    /// ScriptableObjectからキャラクター情報を取得
    /// </summary>
    [SerializeField]
    private CharacterDataAsset _characterDataAsset;

    /// <summary>
    /// キャラクターの初期位置X
    /// </summary>
    [SerializeField]
    private int _initPosX;
    /// <summary>
    /// キャラクターの初期位置Z
    /// </summary>
    [SerializeField]
    private int _initPosZ;
    /// <summary>
    /// キャラクターのID(Image参照で使用する)
    /// </summary>
    [SerializeField]
    private int _characterId;
    /// <summary>
    /// キャラクターのユニークID
    /// </summary>
    [SerializeField]
    private int _uniqueId;

    /// <summary>
    /// キャラクターの衝突判定1
    /// </summary>
    [SerializeField]
    private Collider _weaponCollider_L;

    /// <summary>
    /// キャラクターの衝突判定2
    /// </summary>
    [SerializeField]
    private Collider _weaponCollider_R;

    private int _charaPosX;
    private int _charaPosZ;
    private CharacterData _orgCharacterData;

    // イベント
    private ReactiveProperty<int> _currentHp = new ReactiveProperty<int>();

    private CharacterManager _characterManager;

    // キャラクターの移動後の座標
    private Vector3 _moveEndPosition = Vector3.zero;

    private bool _activeoOperate;


    /// <summary>
    /// キャラクターの現在のx座標
    /// </summary>
    public int GetCharaPosX
    {
        get
        {
            return _charaPosX;
        }
    }

    /// <summary>
    /// キャラクターの現在のz座標
    /// </summary>
    public int GetCharaPosZ
    {
        get
        {
            return _charaPosZ;
        }
    }

    /// <summary>
    /// キャラクターのID(モンスター種ごとに変わる)
    /// </summary>
    public int GetCharaId
    {
        get
        {
            return _characterId;
        }
    }

    /// <summary>
    /// CharacterManagerから取得したキャラクター情報(基本情報)
    /// </summary>
    public CharacterData GetCharactrerData
    {
        set
        {
            _orgCharacterData = value;
        }
        get
        {
            return _orgCharacterData;
        }
    }

    /// <summary>
    /// キャラクターの現在の体力
    /// </summary>
    public int CurrentHp
    {
        set
        {
            _currentHp.Value = value;
        }
        get
        {
            return _currentHp.Value;
        }
    }

    // UniRx使用する際は、以下の記載方法
    //public int CurrentHp { get => _currentHp.Value; set => _currentHp.Value = value; }

    /// <summary>
    /// キャラクターのユニークID(重複なし)
    /// </summary>
    public int UniqueId
    {
        set
        {
            _uniqueId = value;
        }
        get
        {
            return _uniqueId;
        }
    }

    public bool ActiveOperator
    {
        get
        {
            return _activeoOperate;
        }

    }



    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterManager = CharacterManager.instance;
        _activeoOperate = false;

        // 現在HPが0になったら消滅
        // 処理順の関係で、デフォルト値設定
        //_currentHp.Value = 999;
        Debug.Log("Character.cs : CurrentHP");
        _currentHp.Where(currentHp => currentHp == 0)
                  .Subscribe(_ =>
                  {
                      // 自分の名前＋ユニークID
                      _characterManager.SetDeleteCharacterName(GetCharactrerData.CharacterName);
                      _characterManager.SetDeleteCharacterInfo(GetCharactrerData.CharacterName, UniqueId);

                      Debug.Log("Character Died" + GetCharactrerData.CharacterName);

                      _characterManager.DeleteCharacterInfo(this);

                      _characterManager.ShowDeadEffect(this.transform.position, this.gameObject);

                      DeadMotion();
                      //Destroy(this.gameObject);
                  });
    }

    void Update()
    {

        //Debug.Log(transform.position);


    }

    public void ChangeActiveOperator(bool ope)
    {
        _activeoOperate = ope;
        //if (!_activeoOperate)
        //{
        //    Debug.Log("ChangeActiveOperator True");
        //    _activeoOperate = ope;
        //}
        //else
        //{
        //    Debug.Log("ChangeActiveOperator False");
        //    _activeoOperate = false;
        //}
    }

    /// <summary>
    /// キャラクターの初期位置を設定
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posZ"></param>
    public void SetInitPosition(int posX, int posZ)
    {
        _charaPosX = posX;
        _charaPosZ = posZ;

        transform.position = new Vector3(_charaPosX, 0.5f, _charaPosZ);
    }


    /// <summary>
    /// キャラクターを指定先まで移動させる
    /// </summary>
    /// <param name="distPosX"></param>
    /// <param name="distPosZ"></param>
    public void MovePosition(int distPosX, int distPosZ)
    {
        // 移動先の向きを計算(ベクトル)
        Vector3 vector3 = Vector3.zero;
        vector3.x = distPosX - GetCharaPosX;
        vector3.z = distPosZ - GetCharaPosZ;

        _moveEndPosition = vector3;

        // カメラ移動
        //_cameraManager.MoveCameraPosition(vector3);

        // 移動先の方向に向きを回転(移動先のベクトルが必要)
        Quaternion look = Quaternion.LookRotation(vector3);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 1.0f);

        // 指定位置に、0.5秒かけて移動する
        // SetEaseでどのぐらいの緩急をつけるのか設定
        transform.DOMove(vector3, 0.5f).SetEase(Ease.Linear).SetRelative();

        _animator.SetFloat("SetMove", 2.0f); // vector3.magnitude);

        // 指定位置まで仮に0.5秒かけて移動するので、その時間で停止させる
        DOVirtual.DelayedCall(0.5f, () =>
        {
            StopMove();
        });

        // キャラクターの位置情報を更新
        _charaPosX = distPosX;
        _charaPosZ = distPosZ;
    }

    /// <summary>
    /// 移動先の方向を向かせる
    /// </summary>
    /// <param name="distPosX"></param>
    /// <param name="distPosZ"></param>
    public void TurnToTarget(int distPosX, int distPosZ)
    {
        // 移動先の向きを計算(ベクトル)
        Vector3 vector3 = Vector3.zero;
        vector3.x = distPosX - GetCharaPosX;
        vector3.z = distPosZ - GetCharaPosZ;

        // 移動先の方向に向きを回転(移動先のベクトルが必要)
        Quaternion look = Quaternion.LookRotation(vector3);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 1.0f);
    }

    /// <summary>
    /// 移動停止
    /// </summary>
    public void StopMove()
    {
        Debug.Log("Move Stop");
        _animator.SetFloat("SetMove", 0f);
    }

    /// <summary>
    /// 攻撃モーション開始
    /// </summary>
    public void AttackMotion()
    {
        //ModifyPosition();
        _animator.SetTrigger("Attack");
    }

    /// <summary>
    /// 死亡モーション開始
    /// </summary>
    private void DeadMotion()
    {
        _animator.SetTrigger("Dead");
    }


    /// <summary>
    /// 攻撃モーション終了時に衝突判定を隠す(AnimationClipから設定)
    /// </summary>
    public void HideWeaponColider()
    {
        if (_weaponCollider_L != null)
        {
            _weaponCollider_L.enabled = false;
        }

        if (_weaponCollider_R != null)
        {
            _weaponCollider_R.enabled = false;
        }

        //ModifyPosition();
    }

    /// <summary>
    /// 攻撃モーション時に衝突判定を表示(AnimationClipから設定)
    /// </summary>
    public void ShowWeaponColider()
    {
        Debug.Log("ShowWeaponColider");
        if (_weaponCollider_L != null)
        {
            _weaponCollider_L.enabled = true;
        }

        if (_weaponCollider_R != null)
        {
            _weaponCollider_R.enabled = true;
        }
    }

    public void ModifyPosition()
    {
        Debug.Log("ModifyPosition");
        transform.position = new Vector3(_charaPosX, 0.5f, _charaPosZ);
    }


}
