using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScrollViewButton : MonoBehaviour
{
    private GameManager _gameManager;
    private int _uniqueId;

    private void Start()
    {
        _gameManager = GameManager.Instance;        
    }

    /// <summary>
    /// スクロールビュー上のボタンで選択したキャラクターのステータスを表示する
    /// </summary>
    public void ClickShowCharacterStatus()
    {
        // 親オブジェクトで保持しているユニークIDを設定
        _uniqueId = GetComponentInParent<TurnCharacterImageInfo>().UniqueId;

        _gameManager.ShowScrollViewCharacterStatus(_uniqueId);

    }
}
