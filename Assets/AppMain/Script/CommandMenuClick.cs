using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMenuClick : MonoBehaviour
{

    [SerializeField]
    private GUIManager _guiManager;

    private void Start()
    {

    }

    public void ClickMoveButton()
    {
        _guiManager.ShowCharacterMoveArea();
    }

    public void ClickAttackButton()
    {
        _guiManager.ShowCharacterAttackArea();
    }
    
    public void ClickWaitButton()
    {
        _guiManager.EndCharacterTurn();
    }
}
