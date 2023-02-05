namespace BlockModeStatus
{
    /// <summary>
    /// 選択したブロックの状態
    /// </summary>
    public enum SelectBlockMode
    {
        OFF = 0,
        SELECT_BLOCK,
        SELECT_MOVE,
        SELECT_ATTACK
    }
}

public static class MapInfo
{
    // マップのサイズ
    public const int MAP_WIDTH = 8;
    public const int MAP_HEIGHT = 8;

    // 移動不可ブロックの生成率
    public const float RATE_NOT_MOVE_BLOCK = 80.0f;
}

