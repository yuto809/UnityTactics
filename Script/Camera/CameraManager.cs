using UnityEngine;
using DG.Tweening;
using Cinemachine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    /* 操作キャラクターごとにカメラを切り替える */
    // TurnManagerで操作したい
    // 生成キャラの任意Z軸離れた個所にカメラ移動
    // 操作キャラクターの移動に合わせて、カメラも移動させる

    private const int PRIORITY_VIRTUAL_CAMERA = 10;

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private Camera _characterCamera;

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

    public static CameraManager instance;

    public CinemachineVirtualCamera ActiveVirtualCamera
    {
        get
        {
            return _activeVirtualCamera;
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
        _turnManager = TurnManager.instance;

        // バーチャルカメラの優先順位設定(大きい方が優先度が高い)
        _changeCamera1.Priority = PRIORITY_VIRTUAL_CAMERA;
        _changeCamera2.Priority = PRIORITY_VIRTUAL_CAMERA - 1;

        //_mainCamera.transform.position = new Vector3(0, DefineCameraClass.CAMERA_POS_Y, 0);
        //_mainCamera.transform.rotation = Quaternion.Euler(DefineCameraClass.CAMERA_ROTATE_X, 0.0f, 0.0f);
        //_mainCamera.gameObject.SetActive(true);
        //_characterCamera.gameObject.SetActive(false);


        //_changeCamera1.Follow = _turnManager.ActiveTurnCharacter.transform; // 追従対象
        //_changeCamera1.LookAt = lookAt; // 照準合わせ対象
        //_changeCamera2.Follow = follow; // 追従対象
        //_changeCamera2.LookAt = lookAt; // 照準合わせ対象

        _changeCamera1.enabled = true;
        _activeVirtualCamera = _changeCamera1;
        _toggle = true;
        _changeCamera2.enabled = false;

    }

    void Update()
    {

    }

    /// <summary>
    /// カメラ位置を操作キャラクターに合わせる
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posZ"></param>
    public void SetCameraPosition(float posX, float posZ)
    {
        _mainCamera.transform.position = new Vector3(posX, DefineCameraClass.CAMERA_POS_Y, (posZ - DefineCameraClass.FIRST_CAMERA_POS_Z));
        _mainCamera.transform.rotation = Quaternion.Euler(DefineCameraClass.CAMERA_ROTATE_X, 0.0f, 0.0f);
        Debug.Log("SetCameraPosition : " + _mainCamera.transform.position);
    }


    public void ChangeActiveVirtualCamera(Character character, Character nextCharacter)
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
    /// キャラ移動時にカメラも移動させる
    /// </summary>
    /// <param name="dist"></param>
    public void MoveCameraPosition(Vector3 dist)
    {
        Debug.Log("キャラの移動先の座標" + dist);

        // 指定位置に、0.5秒かけて移動する
        // SetEaseでどのぐらいの緩急をつけるのか設定
        _mainCamera.transform.DOMove(dist, 0.5f).SetEase(Ease.Linear).SetRelative();

    }

    /// <summary>
    /// 攻撃タイミングでカメラを揺らす
    /// </summary>
    public void ShakeCamera()
    {
        var source = GetComponent<Cinemachine.CinemachineImpulseSource>();
        source.GenerateImpulse();
        //_mainCamera.transform.DOShakePosition(1.0f, 0.5f, 5, 90.0f, false, true);
    }
}
