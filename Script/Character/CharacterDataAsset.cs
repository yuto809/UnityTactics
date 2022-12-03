using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObjectを生成
/// </summary>
[CreateAssetMenu(menuName = "MyScriptable/Create CharacterData")]
public class CharacterDataAsset : ScriptableObject
{
    public List<CharacterData> _characterDataList = new List<CharacterData>();
}

/// <summary>
/// エディターからクラス情報を設定する
/// </summary>
[System.Serializable]
public class CharacterData
{
    public enum Attribute
    {
        FIRE = 0,
        WATER,
        WIND,
        THUBMDER,
        EARTH
    }

    [SerializeField]
    private string _characterName;

    [SerializeField]
    private int _characterId;

    [SerializeField]
    private int _characterLevel;

    [SerializeField]
    private int _maxHp;

    [SerializeField]
    private int _atk;

    [SerializeField]
    private int _def;

    [SerializeField]
    private int _move;

    [SerializeField]
    private int _quick;

    [SerializeField]
    private Attribute _attribute;

    [SerializeField]
    private bool _enemyFlg;

    [SerializeField]
    private Sprite _charaImage;

    public string CharacterName
    {
        get
        {
            return _characterName;
        }
    }

    public int CharacterID
    {
        get
        {
            return _characterId;
        }
    }

    public int CharacterLevel
    {
        get
        {
            return _characterLevel;
        }
    }

    public int MaxHP
    {
        get
        {
            return _maxHp;
        }
    }

    public int Atk
    {
        get
        {
            return _atk;
        }
    }

    public int Def
    {
        get
        {
            return _def;
        }
    }

    public int Move
    {
        get
        {
            return _move;
        }
    }

    public int Quick
    {
        get
        {
            return _quick;
        }
    }

    public int Type
    {
        get
        {
            return (int)_attribute;
        }
    }

    public bool IsEnemy
    {
        get
        {
            return _enemyFlg;
        }
    }

    public Sprite CharaImage
    {
        get
        {
            return _charaImage;
        }
    }

}
