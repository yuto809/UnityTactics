using System.Collections.Generic;
using UnityEngine;

public class SpriteHolder : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _srcCharacterImage;
    [SerializeField]
    private Sprite _fireSprite;
    [SerializeField]
    private Sprite _waterSprite;
    [SerializeField]
    private Sprite _windSprite;
    [SerializeField]
    private Sprite _thunderSprite;
    [SerializeField]
    private Sprite _earthSprite;

    private void Start()
    {
        //Debug.Log("SpriteManager Start");
        //_srcCharacterImage = new List<Sprite>();
    }

    public List<Sprite> CharacterImageList
    {
        get
        {
            return _srcCharacterImage;
        }
    }

    public Sprite FireSprite
    {
        get
        {
            return _fireSprite;
        }
    }

    public Sprite WaterSprite
    {
        get
        {
            return _waterSprite;
        }
    }

    public Sprite WindSprite
    {
        get
        {
            return _windSprite;
        }
    }

    public Sprite ThunderbSprite
    {
        get
        {
            return _thunderSprite;
        }
    }
    public Sprite EarthSprite
    {
        get
        {
            return _earthSprite;
        }
    }
}
