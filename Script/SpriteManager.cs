using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
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
    private Sprite _thumbSprite;
    [SerializeField]
    private Sprite _earthSprite;

    public static SpriteManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

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

    public Sprite ThumbSprite
    {
        get
        {
            return _thumbSprite;
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
