using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



//�����g�p�t�@�C��






public class SelectCharacterImage : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> _srcCharacterImage;

    [SerializeField]
    private Image _dispimage;

    private void Start()
    {
        //_srcCharacterImage = new List<Sprite>();
        _dispimage = GetComponent<Image>();
    }


    // �w�肵�����[�h�Ńu���b�N����������
    // collider�����Editer���Ŗ����ɂ��Ă���
    public void SetCharacterImage(Character chara)
    {

        //var charaSprite = Resources.Load<Sprite>(DefineCharacterClass.CharacterImageFilePath + chara.GetCharactrerData.CharacterName);
        //Sprite charaSprite;

        //#if _DEBUG
        //        Debug.Log(DefineCharacterClass.CharacterImageFilePath + chara.GetCharactrerData.CharacterName);
        //#endif

        Debug.Log("SetCharacterImage " + chara.GetCharactrerData.CharacterName);
        Debug.Log(_srcCharacterImage.Count);
        foreach (Sprite sprite in _srcCharacterImage)
        {
            Debug.Log(sprite.name);
            if (sprite.name.Equals(chara.GetCharactrerData.CharacterName))
            {
                _dispimage.sprite = sprite;
                break;
            }
        }

#if _DEBUG
        Debug.Log(_dispimage.sprite);
#endif
        //switch (chara.GetCharactrerData.CharacterName)
        //{
        //    case DefineCharacterClass.GOBLINE:
        //        _dispimage.sprite = charaSprite;
        //        break;
        //    default:
        //        break;

        //}
    }
}
