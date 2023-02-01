using UnityEngine;

public class TurnCharacterImageInfo : MonoBehaviour
{
    [SerializeField]
    private int _uniqueId;

    public int UniqueId
    {
        set => _uniqueId = value;
        get => _uniqueId;
    }
}
