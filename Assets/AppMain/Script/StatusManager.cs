using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DefineCharacterType;

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
    [SerializeField]
    private SpriteHolder _spriteHolder;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Instance StatusManager");
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

        _dispimage.sprite = character.GetCharacterData.CharaImage;
    }

    /// <summary>
    /// キャラクター毎に対応した属性画像を設定する
    /// </summary>
    /// <param name="character"></param>
    private void SetCharacterTypeImage(Character character)
    {
        switch (character.GetCharacterData.Type)
        {
            case (int)Attribute.FIRE:
                _typeImage.sprite = _spriteHolder.FireSprite;
                break;
            case (int)Attribute.WATER:
                _typeImage.sprite = _spriteHolder.WaterSprite;
                break;
            case (int)Attribute.WIND:
                _typeImage.sprite = _spriteHolder.WindSprite;
                break;
            case (int)Attribute.THUMBDER:
                _typeImage.sprite = _spriteHolder.ThunderbSprite;
                break;
            case (int)Attribute.EARTH:
                _typeImage.sprite = _spriteHolder.EarthSprite;
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
        float currentAmount = (float)character.CurrentHp / character.GetCharacterData.MaxHP;

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
        _characterName.text = character.GetCharacterData.CharacterName;

        Debug.Log("SetStatusText : " + character.CurrentHp);

        // キャラクターHP設定
        _hpText.text = character.CurrentHp + " / " + character.GetCharacterData.MaxHP;

        // キャラクター攻撃力設定
        _atkText.text = character.GetCharacterData.Atk.ToString();
        // キャラクター防御力設定
        _defText.text = character.GetCharacterData.Def.ToString();
        // キャラクター移動力設定
        _moveText.text = character.GetCharacterData.Move.ToString();
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
        targetHp = Mathf.Clamp(targetHp, 0, targetCharacter.GetCharacterData.MaxHP);

        // ダメージ前の現在HPが最大HPに対して、どれぐらいの割合か計算
        float currentAmount = (float)targetCharacter.CurrentHp / targetCharacter.GetCharacterData.MaxHP;

        // ダメージ後の現在HPが最大HPに対して、どれぐらいの割合か計算
        float damageAfterAmount = (float)targetHp / targetCharacter.GetCharacterData.MaxHP;
        
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
        _hpText.text = targetCharacter.CurrentHp + " / " + targetCharacter.GetCharacterData.MaxHP;
    }
}
