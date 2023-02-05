using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCharacterImageInfo : MonoBehaviour
{
    [SerializeField]
    private int _uniqueId;

    public int UniqueId
    {
        set
        {
            _uniqueId = value;
        }
        get
        {
            return _uniqueId;
        }
    }


}
