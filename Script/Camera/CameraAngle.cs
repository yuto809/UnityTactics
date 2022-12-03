using UnityEngine;
using Cinemachine;

public class CameraAngle : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 20.0f;

    private bool _rotateFlg;
    private bool _rotateLeft;
    private TurnManager _turnManager;
    private Character _activeCharacter;
    private CameraManager _cameraManager;

    void Start()
    {
        _rotateFlg = false;
        _rotateLeft = false;
        _turnManager = TurnManager.instance;
        _cameraManager = CameraManager.instance;
    }

    void Update()
    {
        if (_rotateFlg)
        {
            _activeCharacter = _turnManager.ActiveTurnCharacter;

            float speed = _rotateSpeed * Time.deltaTime;

            if (_rotateLeft)
            {
                speed *= -1.0f;
            }

            // RotateAround(円運動の中心,軸,速度)
            // 操作キャラクター中心に回す
            //transform.RotateAround(_activeCharacter.transform.position,  _activeCharacter.transform.up, speed);
            _cameraManager.ActiveVirtualCamera.transform.RotateAround(_activeCharacter.transform.position, _activeCharacter.transform.up, speed);
        }
    }

    /// <summary>
    /// 右方向回転
    /// </summary>
    public void RotateLeft()
    {
        _rotateFlg = true;
    }

    /// <summary>
    /// 左方向回転
    /// </summary>
    public void RotateRight()
    {
        _rotateFlg = true;
        _rotateLeft = true;
    }

    /// <summary>
    /// 回転停止
    /// </summary>
    public void CancelRotate()
    {
        _rotateFlg = false;
        _rotateLeft = false;
    }






}
