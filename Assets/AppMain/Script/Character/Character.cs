using UnityEngine;
using DG.Tweening;
using UniRx;

public class Character : MonoBehaviour
{
    private Animator _animator;

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

    private CharacterData _orgCharacterData;

    // イベント
    private ReactiveProperty<int> _currentHp = new();

    private CharacterManager _characterManager;

    private bool _activeOperate;


    /// <summary>
    /// キャラクターの現在のx座標
    /// </summary>
    public int PositionX { get; private set; }

    /// <summary>
    /// キャラクターの現在のz座標
    /// </summary>
    public int PositionZ { get; private set; }

    /// <summary>
    /// キャラクターのID(モンスター種ごとに変わる)
    /// </summary>
    public int GetCharaId => _characterId;

    /// <summary>
    /// CharacterManagerから取得したキャラクター情報(基本情報)
    /// </summary>
    public CharacterData GetCharacterData
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
            return _activeOperate;
        }

    }

    private void Awake()
    {
        // イベント通知したら止めるようにする
        _currentHp.AddTo(this);
    }


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterManager = CharacterManager.Instance;
        _activeOperate = false;

        // 現在HPが0になったら消滅
        // 処理順の関係で、デフォルト値設定
        //_currentHp.Value = 999;
        Debug.Log("Character.cs : CurrentHP");
        _currentHp.Where(currentHp => currentHp == 0)
                  .Subscribe(_ =>
                  {
                      // 自分の名前＋ユニークID
                      _characterManager.SetDeleteCharacterName(GetCharacterData.CharacterName);
                      _characterManager.SetDeleteCharacterInfo(GetCharacterData.CharacterName, UniqueId);

                      Debug.Log("Character Died" + GetCharacterData.CharacterName);

                      _characterManager.DeleteCharacterInfo(this);

                      _characterManager.ShowDeadEffect(this.transform.position, this.gameObject);

                      DeadMotion();
                      //Destroy(this.gameObject);
                  });
    }

    public void ChangeActiveOperator(bool ope)
    {
        _activeOperate = ope;
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
        PositionX = posX;
        PositionZ = posZ;

        transform.position = new Vector3(PositionX, 0.5f, PositionZ);
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
        vector3.x = distPosX - PositionX;
        vector3.z = distPosZ - PositionZ;

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
        DOVirtual.DelayedCall(0.5f, StopMove);

        // キャラクターの位置情報を更新
        PositionX = distPosX;
        PositionZ = distPosZ;
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
        vector3.x = distPosX - PositionX;
        vector3.z = distPosZ - PositionZ;

        // 移動先の方向に向きを回転(移動先のベクトルが必要)
        Quaternion look = Quaternion.LookRotation(vector3);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 1.0f);
    }

    /// <summary>
    /// 移動停止
    /// </summary>
    private void StopMove()
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
}
