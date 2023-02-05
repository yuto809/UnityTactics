using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    /* 操作キャラクターごとにカメラを切り替える */
    // TurnManagerで操作したい
    // 生成キャラの任意Z軸離れた個所にカメラ移動
    // 操作キャラクターの移動に合わせて、カメラも移動させる

    private const int PriorityVirtualCamera = 10;

    [SerializeField]
    private float _rotateSpeed = 20.0f;

    [SerializeField]
    private CinemachineVirtualCamera _changeCamera1;

    [SerializeField]
    private CinemachineVirtualCamera _changeCamera2;

    private TurnManager _turnManager;

    private CinemachineVirtualCamera _activeVirtualCamera;

    /// <summary>
    /// true : VirtualCamera1 / false : VirtualCamera2
    /// </summary>
    private bool _toggle;

    public static CameraManager Instance;

    /// <summary>
    /// カメラの回転状態
    /// </summary>
    private CameraAngle _cameraAngle;

    public CinemachineVirtualCamera ActiveVirtualCamera
    {
        get
        {
            return _activeVirtualCamera;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _turnManager = TurnManager.Instance;

        // バーチャルカメラの優先順位設定(大きい方が優先度が高い)
        _changeCamera1.Priority = PriorityVirtualCamera;
        _changeCamera2.Priority = PriorityVirtualCamera - 1;

        _changeCamera1.enabled = true;
        _activeVirtualCamera = _changeCamera1;
        _toggle = true;
        _changeCamera2.enabled = false;

        _cameraAngle = GetComponent<CameraAngle>();
    }

    void Update()
    {
        // カメラの回転状態を取得して、カメラを回転させる
        RotateCamera((int)_cameraAngle.GetRotateStatus);
    }

    /// <summary>
    /// カメラを回転させる
    /// </summary>
    private void RotateCamera(int rotDirection)
    {
        Character character = null;

        if (rotDirection == (int)CameraAngle.RotateStatus.NOT_ROTATE)
        {
            return;
        }

        // 操作キャラ取得
        character = _turnManager.ActiveTurnCharacter;

        float speed = rotDirection * _rotateSpeed * Time.deltaTime;

        // RotateAround(円運動の中心,軸,速度)
        // 操作キャラクター中心に回す
        ActiveVirtualCamera.transform.RotateAround(character.transform.position, character.transform.up, speed);
    }

    /// <summary>
    /// 追従対象のカメラを切り替える
    /// </summary>
    /// <param name="character"></param>
    /// <param name="nextCharacter"></param>
    public void ChangeActiveVirtualCamera(Character character)
    {
        // VirtualCamera1を使用している場合
        if (_toggle)
        {
            _toggle = false;

            _changeCamera1.enabled = false;
            _changeCamera2.enabled = true;
            _activeVirtualCamera = _changeCamera2;
            _changeCamera2.Follow = character.transform; // 追従対象
        }
        else
        {
            _toggle = true;

            _changeCamera2.enabled = false;
            _changeCamera1.enabled = true;
            _activeVirtualCamera = _changeCamera1;
            _changeCamera1.Follow = character.transform; // 追従対象

        }
    }

    /// <summary>
    /// 攻撃タイミングでカメラを揺らす
    /// </summary>
    public void ShakeCamera()
    {
        var source = GetComponent<Cinemachine.CinemachineImpulseSource>();
        source.GenerateImpulse();
    }
}
