using UnityEngine;

public class CameraAngle : MonoBehaviour
{
    /// <summary>
    /// カメラの回転状態を管理
    /// </summary>
    public enum RotateStatus
    {
        LEFT_ROTATE  = -1,
        NOT_ROTATE   = 0,
        RIGHT_ROTATE = 1
    }

    /// <summary>
    /// カメラの回転状態
    /// </summary>
    private RotateStatus _rotateStatus;

    /// <summary>
    /// カメラの回転状態を返す
    /// </summary>
    public RotateStatus GetRotateStatus
    {
        get
        {
            return _rotateStatus;
        }
    }

    private void Start()
    {
        _rotateStatus = RotateStatus.NOT_ROTATE;
    }

    /// <summary>
    /// 右方向回転
    /// </summary>
    public void RotateLeft()
    {
        _rotateStatus = RotateStatus.LEFT_ROTATE;
    }

    /// <summary>
    /// 左方向回転
    /// </summary>
    public void RotateRight()
    {
        _rotateStatus = RotateStatus.RIGHT_ROTATE;
    }

    /// <summary>
    /// 回転停止
    /// </summary>
    public void CancelRotate()
    {
        _rotateStatus = RotateStatus.NOT_ROTATE;
    }
}
