using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo : MonoBehaviour
{
    public List<StageData> _stageDataList = new List<StageData>();
}


[System.Serializable]
public class StageData
{
    public enum StageLevelInfo
    {
        STAGE_LEVEL_1 = 0,
        STAGE_LEVEL_2,
        STAGE_LEVEL_3
    }

    /// <summary>
    /// 敵キャラクター数
    /// </summary>
    [SerializeField]
    private int _enemyCount;
    /// <summary>
    /// プレイヤー数
    /// </summary>
    [SerializeField]
    private int _playerCount;
    /// <summary>
    /// ステージ上に配置可能な敵キャラクターレベル
    /// </summary>
    [SerializeField]
    private int _enemyLevel;
    /// <summary>
    /// 自動生成するためのステージレベル
    /// </summary>
    [SerializeField]
    private StageLevelInfo _stageLevel;
    
    public int EnemyCount
    {
        get
        {
            return _enemyCount;
        }
    }

    public int PlayerCount
    {
        get
        {
            return _playerCount;
        }
    }

    public int EnemyLevel
    {
        get
        {
            return _enemyLevel;
        }
    }

    public StageLevelInfo StageLevel
    {
        get
        {
            return _stageLevel;
        }
    }
}
