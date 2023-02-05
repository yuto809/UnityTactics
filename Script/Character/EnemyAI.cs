using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
    private GameManager _gameManager;
    private MapManager _mapManager;
    private CharacterManager _characterManager;
    private Character _selectingCharacter;

    // 次処理実行開始するまでの待ち時間
    private const float WAIT_ACTION_TIME = 1.0f;

    /// <summary>
    /// 敵行動中フラグ
    /// </summary>
    private bool _enemyActionFlg;

    /// <summary>
    /// 敵が行動中かどうかを取得する
    /// </summary>
    public bool EnemyActionFlg
    {
        get
        {
            return _enemyActionFlg;
        }
    }

    private void Start()
    {
        _gameManager = GameManager.instance;
        _mapManager = MapManager.instance;
        _characterManager = CharacterManager.instance;
    }

    /// <summary>
    /// 敵行動開始
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public IEnumerator EnemyAction(Character enemy)
    {
        // 行動済みフラグ
        bool actionFlg = false;
        // 周囲リスト
        List<MapBlock> aroundArea = new List<MapBlock>();
        // 移動可能範囲リスト
        List<MapBlock> moveAllowArea = new List<MapBlock>();
        // 攻撃可能範囲リスト
        List<MapBlock> atkAllowArea = new List<MapBlock>();
        // 現在の行動開始キャラクターを更新
        _selectingCharacter = enemy;

        // 敵行動中フラグON
        _enemyActionFlg = true;

        // 攻撃対象キャラクターのステータスウィンドウを表示する
        _gameManager.SelectPatternUI(GameManager.PatternUI.PATTERN_STATUS_UI, _selectingCharacter);



        // ★★★★★★★★★★　処理を見やすくするために分割する ★★★★★★★★★★★★

        // 1 自身の周囲にプレイヤーが存在するか確認
        //   →　周囲にプレイヤーがいれば攻撃する
        // 2 自身の周囲にプレイヤーがいなければ、自身の移動可能範囲を検索して移動

        // 3 移動後に、自身の周囲にプレイヤーが存在するか確認
        //   →　周囲にプレイヤーがいれば攻撃する



        //****** START 周囲1マスを検索して敵を見つけたら攻撃する *************

        // 自身周囲1マス取得
        aroundArea = _mapManager.SearchAttackAreaByCharacter(enemy, enemy.GetCharaPosX, enemy.GetCharaPosZ);

        // 移動可能範囲ごとに攻撃範囲を検索
        foreach (MapBlock atkBlock in aroundArea)
        {
            // 指定したブロック状にの座標を引数にして、同じ座標にキャラクターが居るか確認
            var targetCharacter = _characterManager.SearchCharacterByPosition(atkBlock.BlockPosX, atkBlock.BlockPosZ);

            // 取得した情報が空以外、かつ自分自身でない、かつ敵キャラクター(敵フラグなし)の場合
            if ((targetCharacter != null) &&
                (targetCharacter != _selectingCharacter) &&
                (!targetCharacter.GetCharactrerData.IsEnemy))
            {
                Debug.Log("敵キャラクターの行動を開始します：" + enemy.GetCharactrerData.CharacterName +
                            " 周囲1マスの攻撃対象キャラクター：" + targetCharacter.GetCharactrerData.CharacterName);

                DOVirtual.DelayedCall(1.0f, () =>
                {
                    // 指定キャラクターが対象キャラクターへ攻撃を行う
                    _gameManager.Attack(targetCharacter, enemy);
                });

                // debug
                Debug.Log(targetCharacter.GetCharaPosX + " : " + targetCharacter.GetCharaPosZ);
                Debug.Log(enemy.GetCharaPosX + " : " + enemy.GetCharaPosZ);
                //UnityEditor.EditorApplication.isPaused = true;

                // 行動済みフラグ更新
                actionFlg = true;
                break;
            }
        }

        yield return new WaitForSeconds(WAIT_ACTION_TIME);
        //****** END 周囲1マスを検索して敵を見つけたら攻撃する *************


        //****** START 移動範囲検索→攻撃範囲検索→移動して攻撃 *************

        // 指定キャラの移動範囲を取得する
        // 取得範囲は縦横
        moveAllowArea = _mapManager.SearchMoveAreaByCharacter(enemy);

        // 移動可能範囲内を検索
        foreach (MapBlock moveBlock in moveAllowArea)
        {
            // 行動し終えたらループを抜ける
            if (actionFlg)
            {
                break;
            }

            // 移動先に対する攻撃範囲を取得
            // arg1 : 移動を行うキャラクター
            // arg2,3 : 移動先のマップブロック
            atkAllowArea = _mapManager.SearchAttackAreaByCharacter(enemy, moveBlock.BlockPosX, moveBlock.BlockPosZ);

            Debug.Log("攻撃範囲取得： " + moveBlock.BlockPosX + " : " + moveBlock.BlockPosZ);

            // 移動可能範囲ごとに攻撃範囲を検索
            foreach (MapBlock atkBlock in atkAllowArea)
            {
                // 指定したブロック状にの座標を引数にして、同じ座標にキャラクターが居るか確認
                var targetCharacter = _characterManager.SearchCharacterByPosition(atkBlock.BlockPosX, atkBlock.BlockPosZ);

                // 取得した情報が空以外、かつ自分自身でない、かつ敵キャラクター(敵フラグなし)の場合
                if ((targetCharacter != null) &&
                    (targetCharacter != _selectingCharacter) &&
                    (!targetCharacter.GetCharactrerData.IsEnemy))
                {
                    Debug.Log("敵キャラクターの行動を開始します：" + enemy.GetCharactrerData.CharacterName +
                                " 攻撃対象キャラクター：" + targetCharacter.GetCharactrerData.CharacterName);

                    // 移動前のブロック上キャラ存在フラグを初期化
                    _mapManager.SetUsedMapBlock(enemy.GetCharaPosX, enemy.GetCharaPosZ, false);

                    // キャラクター移動と停止　0.5s
                    enemy.MovePosition(moveBlock.BlockPosX, moveBlock.BlockPosZ);

                    // 移動先のブロックを使用済みにする
                    // ★★これもUniRxでキャラクター側の座標が変わるタイミングで変更したい
                    // これだと敵キャラ側にも同じ処理が必要

                    // 移動先のブロック上キャラ存在フラグを設定
                    _mapManager.SetUsedMapBlock(moveBlock.BlockPosX, moveBlock.BlockPosZ, true);

                    yield return new WaitForSeconds(1.0f);

                    // 指定キャラクターが対象キャラクターへ攻撃を行う
                    _gameManager.Attack(targetCharacter, enemy);

                    // debug
                    Debug.Log(moveBlock.BlockPosX + " : " + moveBlock.BlockPosZ);
                    Debug.Log(targetCharacter.GetCharaPosX + " : " + targetCharacter.GetCharaPosZ);
                    Debug.Log(enemy.GetCharaPosX + " : " + enemy.GetCharaPosZ);
                    //UnityEditor.EditorApplication.isPaused = true;

                    // 行動済みフラグ更新
                    actionFlg = true;
                    break;
                }
            }
        }

        yield return new WaitForSeconds(WAIT_ACTION_TIME);
        //****** END 移動範囲検索→攻撃範囲検索→移動して攻撃 *************

        // 対象が見つからず何も行動していない場合
        if (!actionFlg)
        {
            var randIndex = Random.Range(0, moveAllowArea.Count);
            // キャラクター移動
            enemy.MovePosition(moveAllowArea[randIndex].BlockPosX, moveAllowArea[randIndex].BlockPosZ);
        }

        // 操作キャラクターを初期化
        _gameManager.ClearSelectCharacter();

        // 選択状態クリア
        _mapManager.ClearSelectBlockMode();


        yield return new WaitForSeconds(WAIT_ACTION_TIME);

        // ステータスウィンドウとコマンドメニューを非表示
        _gameManager.SelectPatternUI(GameManager.PatternUI.PATTERN_HIDE_COMMAND_STATUS_UI);

        // 敵行動終了
        _enemyActionFlg = false;

    }
}

