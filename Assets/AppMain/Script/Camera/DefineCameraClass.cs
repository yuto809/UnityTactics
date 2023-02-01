using UnityEngine;

public class DefineCameraClass : MonoBehaviour
{
    /// <summary>
    /// メインカメラの高さを固定
    /// </summary>
    public const float CAMERA_POS_Y = 3.5f;

    /// <summary>
    /// メインカメラの角度Xを固定(カメラ切り替え時の初回値)
    /// </summary>
    public const float CAMERA_ROTATE_X = 45.0f;

    /// <summary>
    /// 操作kキャラクターから、距離をおいてメインカメラを設定するための固定値
    /// </summary>
    public const float FIRST_CAMERA_POS_Z = 2.0f;//4.5f;
}
