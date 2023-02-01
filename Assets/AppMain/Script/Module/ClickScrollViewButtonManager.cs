using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScrollViewButtonManager : MonoBehaviour
{
    private GameManager _gameManager;
    private int _uniqueId;

    void Start()
    {
        _gameManager = GameManager.instance;        
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
