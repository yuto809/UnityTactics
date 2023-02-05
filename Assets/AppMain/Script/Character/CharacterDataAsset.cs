using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのステータスパラメータのテーブルを生成
/// 詳細はCharacterData参照
/// </summary>
[CreateAssetMenu(menuName = "MyScriptable/Create CharacterData")]
public class CharacterDataAsset : ScriptableObject
{
    public List<CharacterData> _characterDataList = new ();

    /// <summary>
    /// キャラクターIDに紐づくマスターデータを返す
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CharacterData GetCharacterDataById(int id)
    {
        // IDは1originのため、indexは-1
        return _characterDataList[id - 1];
    }
}

/// <summary>
/// エディターからクラス情報を設定する
/// </summary>
[System.Serializable]
public class CharacterData
{
    /// <summary>
    /// 属性
    /// </summary>
    public enum Attribute
    {
        Fire = 0,
        Water,
        Wind,
        Thunder,
        Earth
    }

    /// <summary>
    /// キャラクター名
    /// </summary>
    [SerializeField]
    private string _characterName;

    /// <summary>
    /// キャラクターID(重複あり)
    /// </summary>
    [SerializeField]
    private int _characterId;

    /// <summary>
    /// キャラクターレベル
    /// </summary>
    [SerializeField]
    private int _characterLevel;

    /// <summary>
    /// キャラクターの最大体力値
    /// </summary>
    [SerializeField]
    private int _maxHp;

    /// <summary>
    /// キャラクターの攻撃力
    /// </summary>
    [SerializeField]
    private int _atk;

    /// <summary>
    /// キャラクターの防御力
    /// </summary>
    [SerializeField]
    private int _def;

    /// <summary>
    /// キャラクターの移動力
    /// </summary>
    [SerializeField]
    private int _move;

    /// <summary>
    /// キャラクターの素早さ
    /// </summary>
    [SerializeField]
    private int _quick;

    /// <summary>
    /// キャラクターの属性
    /// </summary>
    [SerializeField]
    private Attribute _attribute;

    /// <summary>
    /// キャラクターが敵かどうか
    /// </summary>
    [SerializeField]
    private bool _enemyFlg;

    /// <summary>
    /// キャラクター画像
    /// </summary>
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
