using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusManager : MonoBehaviour
{
    [SerializeField]
    private Image _dispimage;
    [SerializeField]
    private Image _typeImage;
    [SerializeField]
    private Text _characterName;
    [SerializeField]
    private Text _hpText;
    [SerializeField]
    private Slider _hpSlider;
    [SerializeField]
    private Text _atkText;
    [SerializeField]
    private Text _defText;
    [SerializeField]
    private Text _moveText;

    private SpriteManager _spriteManager;

    public static StatusManager instance;

    private void Awake()
    {
        if (instance = null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Instance StatusManager");
        _spriteManager = SpriteManager.instance; // GameObject.Find("SpriteManager").GetComponent<SpriteManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ステータスウィンドウ情報を設定
    /// </summary>
    /// <param name="character"></param>
    public void SetStatusInfo(Character character)
    {
        // キャラクター画像を設定
        SetCharacterImage(character);

        // キャラクターの属性画像を設定
        SetCharacterTypeImage(character);

        // キャラクターのHP状態設定
        SetCharacterHpSlider(character);

        // ステータスウィンドウで使用するテキストを設定する
        SetStatusText(character);
    }

    /// <summary>
    /// キャラクター画像を設定する
    /// </summary>
    /// <param name="character"></param>
    private void SetCharacterImage(Character character)
    {
        //Debug.Log("SetCharacterImage " + character.GetCharactrerData.CharacterName);

        //foreach (Sprite sprite in _spriteManager.CharacterImageList)
        //{
        //    if (sprite.name.Equals(character.GetCharactrerData.CharacterName))
        //    {
        //        _dispimage.sprite = sprite;
        //        break;
        //    }
        //}

        _dispimage.sprite = character.GetCharactrerData.CharaImage;
    }

    /// <summary>
    /// キャラクター毎に対応した属性画像を設定する
    /// </summary>
    /// <param name="character"></param>
    private void SetCharacterTypeImage(Character character)
    {
        switch(character.GetCharactrerData.Type)
        {
            case (int)DefineCharacterClass.Attribute.FIRE:
                _typeImage.sprite = _spriteManager.FireSprite;
                break;
            case (int)DefineCharacterClass.Attribute.WATER:
                _typeImage.sprite = _spriteManager.WaterSprite;
                break;
            case (int)DefineCharacterClass.Attribute.WIND:
                _typeImage.sprite = _spriteManager.WindSprite;
                break;
            case (int)DefineCharacterClass.Attribute.THUMBDER:
                _typeImage.sprite = _spriteManager.ThumbSprite;
                break;
            case (int)DefineCharacterClass.Attribute.EARTH:
                _typeImage.sprite = _spriteManager.EarthSprite;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// キャラクターのHPゲージを設定
    /// </summary>
    /// <param name="character"></param>
    private void SetCharacterHpSlider(Character character)
    {
        // ダメージ前の現在HPが最大HPに対して、どれぐらいの割合か計算
        float currentAmount = (float)character.CurrentHp / character.GetCharactrerData.MaxHP;

        // スライダーは0~1の範囲
        _hpSlider.value = currentAmount;
    }

    /// <summary>
    /// ステータスウィンドウにテキストを設定
    /// </summary>
    /// <param name="character"></param>
    private void SetStatusText(Character character)
    {

        Debug.Log("SetStatusText");

        // キャラクター名設定
        _characterName.text = character.GetCharactrerData.CharacterName;

        Debug.Log("SetStatusText : " + character.CurrentHp);

        // キャラクターHP設定
        _hpText.text = character.CurrentHp + " / " + character.GetCharactrerData.MaxHP.ToString();

        // キャラクター攻撃力設定
        _atkText.text = character.GetCharactrerData.Atk.ToString();
        // キャラクター防御力設定
        _defText.text = character.GetCharactrerData.Def.ToString();
        // キャラクター移動力設定
        _moveText.text = character.GetCharactrerData.Move.ToString();
    }

    /// <summary>
    /// HPゲージを変動
    /// </summary>
    /// <param name="targetCharacter"></param>
    /// <param name="damage"></param>
    public void ChangeHpSlider(Character targetCharacter, int damage)
    {
        // 残りHPの計算
        var targetHp = targetCharacter.CurrentHp - damage;
        // 0に収める
        targetHp = Mathf.Clamp(targetHp, 0, targetCharacter.GetCharactrerData.MaxHP);

        // ダメージ前の現在HPが最大HPに対して、どれぐらいの割合か計算
        float currentAmount = (float)targetCharacter.CurrentHp / targetCharacter.GetCharactrerData.MaxHP;

        // ダメージ後の現在HPが最大HPに対して、どれぐらいの割合か計算
        float damageAfterAmount = (float)targetHp / targetCharacter.GetCharactrerData.MaxHP;
        
        // 第1引数はgetter
        // 第2引数はSetter
        // 第3引数は変化先の目標値
        // 第4引数はアニメーション時間
        // OnUpdateで目標値まで徐々に値を変化させる
        DOTween.To(() => currentAmount, (n) => currentAmount = n, damageAfterAmount, 1.0f)
            .OnUpdate(() =>
            {
                // スライダーは0~1の範囲
                _hpSlider.value = currentAmount;
            });

        // キャラクターの残りHPを設定する
        targetCharacter.CurrentHp = targetHp;
        _hpText.text = targetCharacter.CurrentHp + " / " + targetCharacter.GetCharactrerData.MaxHP.ToString();
    }
}
