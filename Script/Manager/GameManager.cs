using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ターン状態
    /// </summary>
    public enum TurnPhase
    {
        PHASE_MY_TURN = 0,       // プレイヤーターン 
        PHASE_MOVE_TURN,
        PHASE_COMMAND_TURN,
        PHASE_ATK_TURN,
        PHASE_WAIT,              // フェーズ遷移待ち状態
        PHASE_ENEMY_TURN,        // 敵ターン
        PHASE_ENEMY_ACT_TURN,    // 敵行動中状態
        PHASE_NEXT_TURN,         // 次フェーズ設定状態
        PHASE_GAME_START 　　　　// ゲーム開始時状態
    }
    /// <summary>
    ///  画面上に表示するUIパターン
    /// </summary>
    public enum PatternUI
    {
        PATTERN_COMMAND_STATUS_UI = 0,
        PATTERN_STATUS_UI,
        PATTERN_STATUS_DAMAGE_UI,
        PATTERN_HIDE_COMMAND_STATUS_UI,
        PATTERN_STATUS_UI_FROM_SCROLL
    }


    private MapManager _mapManager;
    private CharacterManager _characterManager;
    private GUIManager _guiManager;
    private StatusManager _statusManager;
    private TurnManager _turnManager;
    private CameraManager _cameraManager;

    private List<MapBlock> _moveAllowArea;
    private List<MapBlock> _atkAllowArea;
    private TurnPhase _currentPhase;
    private TurnPhase _nextPhase;
    private Character _selectingCharacter;
    private MapBlock _selectingMapBlock;

    // キャラ選択状態のフラグ
    private bool _selectingCharaFlg;

    private bool _endMoveFlg;
    private bool _endAttackFlg;

    // 敵ターン開始フラグ
    private bool _enemyTurnFlg;

    // シングルトン
    public static GameManager instance;

    // 今実行中のコルーチン
    private Coroutine _currentCoroutine;

    // 次フェーズ遷移待ち状態を維持する時間
    private const float PHASE_WAIT_TIME = 1.0f;

    /// <summary>
    /// 敵AI操作情報
    /// </summary>
    [SerializeField]
    private EnemyAI _enemyAI;

    /// <summary>
    /// 移動終了フラグ
    /// </summary>
    public bool GetEndMoveFlg
    {
        get
        {
            return _endMoveFlg;
        }
    }

    /// <summary>
    /// 攻撃終了フラグ
    /// </summary>
    public bool GetEndAttackFlg
    {
        get
        {
            return _endAttackFlg;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    void Start()
    {
        _cameraManager = CameraManager.instance;
        _mapManager = MapManager.instance;
        _characterManager = CharacterManager.instance;
        _turnManager = TurnManager.instance;

        // instanceを参照するとnullを参照してしまうためFindで検索で一旦対処
        _guiManager = GameObject.Find("GUIManager").GetComponent<GUIManager>(); // GUIManager.instance;
        _statusManager = GameObject.Find("StatusManager").GetComponent<StatusManager>();// StatusManager.instance;
 

        _moveAllowArea = new List<MapBlock>();
        _atkAllowArea = new List<MapBlock>();

        // 初期値としてターン状態はゲーム開始状態に設定
        _currentPhase = TurnPhase.PHASE_GAME_START;
        _nextPhase = TurnPhase.PHASE_NEXT_TURN;

        _enemyAI = GetComponent<EnemyAI>();
    }

    void Update()
    {
        // キャラクター選択状態でない場合
        if ((!_selectingCharaFlg))// && (!_enemyTurnFlg))
        {
            // 常にマップを選択できるようにする
            SelectBlock();
        }

        // 敵ターンはスムーズにまわすために
        // 毎回見る
        if (_turnManager.ActiveTurnCharacter.GetCharactrerData.IsEnemy && _enemyTurnFlg)
        {
            //Debug.Log("敵ターン中");
            switch (_currentPhase)
            {
                // 次のフェーズ遷移待ち
                case TurnPhase.PHASE_WAIT:
                    WaitForCorutine();
                    break;

                // 現在のキャラクターのターンを終了し、次キャラクターのターンを開始
                case TurnPhase.PHASE_NEXT_TURN:

                    // キャラクターターンを次に回す
                    _turnManager.NextTurn();

                    // 次プレイヤーへ
                    NextPlayerPhase();

                    break;

                // 敵行動選択状態
                case TurnPhase.PHASE_ENEMY_TURN:

                    // 敵が行動中でない場合
                    if (!_enemyAI.EnemyActionFlg)
                    {
                        Debug.Log("敵：行動開始");

                        // 敵行動処理
                        _currentCoroutine = StartCoroutine(_enemyAI.EnemyAction(_turnManager.ActiveTurnCharacter));

                        // 敵行動中状態に設定
                        ChangePhase(TurnPhase.PHASE_ENEMY_ACT_TURN);
                    }
                    break;

                // 敵行動中状態はスキップ
                case TurnPhase.PHASE_ENEMY_ACT_TURN:

                    //Debug.Log("敵行動中");
                    // 敵コルーチン処理が終了した場合
                    if (!_enemyAI.EnemyActionFlg)
                    {
                        _currentCoroutine = null;
                        ChangePhase(TurnPhase.PHASE_NEXT_TURN);
                       
                    }

                    break;

                default:
                    break;
            }

        }
        else
        {
            MapBlock mapBlock = null;

            switch (_currentPhase)
            {
                // 次のフェーズ遷移待ち
                case TurnPhase.PHASE_WAIT:

                    WaitForCorutine();
                    break;

                // ゲーム開始状態
                case TurnPhase.PHASE_GAME_START:

                    // ステージ1周させて、タイトルいれる

                    _guiManager.ShowBattleStart();


                    _cameraManager.ChangeActiveVirtualCamera(_turnManager.ActiveTurnCharacter, null);
                    //_cameraManager.SetCameraPosition(_turnManager.ActiveTurnCharacter.GetCharaPosX, _turnManager.ActiveTurnCharacter.GetCharaPosZ);

                    // 始めに行動可能なキャラクターのステータスを表示する
                    _statusManager.SetStatusInfo(_turnManager.ActiveTurnCharacter);
                    _guiManager.SlideStatusWindow();

                    // 自分のターンが終了していないなら、継続
                    if (_turnManager.TurnFlg)
                    {
                        // フェーズ遷移待ちに変更
                        SetPhaseWait(TurnPhase.PHASE_MY_TURN);
                    }
                    else
                    {
                        SetPhaseWait(TurnPhase.PHASE_ENEMY_TURN);
                    }

                    break;


                // 行動選択状態
                case TurnPhase.PHASE_MY_TURN:

                    // クリック処理が正常の場合に処理を実行する
                    if (IsPlayerOperation())
                    {
                        // GUI表示
                        ShowCharacterMenu();
                    }

                    break;

                // 画面上から「移動」選択した際に実行される
                // ここに来る時点ですでに移動可能範囲は保持され表示されている状態
                case TurnPhase.PHASE_MOVE_TURN:

                    // クリック処理が正常の場合に処理を実行する
                    if (IsPlayerOperation())
                    {
                        // 任意のブロック情報を取得
                        mapBlock = GetSelectMapBlock();

                        // 任意のブロックへキャラを移動
                        MoveCharacter(mapBlock);
                    }

                    break;

                // 画面上から「攻撃」選択した際に実行される
                // ここに来る時点ですでに攻撃可能範囲は保持され表示されている状態
                case TurnPhase.PHASE_ATK_TURN:

                    // クリック処理が正常の場合に処理を実行する
                    if (IsPlayerOperation())
                    {
                        // 任意のブロック情報を取得
                        mapBlock = GetSelectMapBlock();

                        // 任意のブロック状のキャラへ攻撃
                        AttackCharacter(mapBlock);
                    }

                    break;

                // 現在のキャラクターのターンを終了し、次キャラクターのターンを開始
                case TurnPhase.PHASE_NEXT_TURN:

                    // 次のキャラクターにターンを回す
                    _turnManager.NextTurn();

                    // 次プレイヤーへ
                    NextPlayerPhase();

                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// クリック時の判定処理
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerOperation()
    {
        //Debug.Log("IsPlayerOperation : " + _turnManager.ActiveTurnCharacter.ActiveOperator);


        // ★操作権を与えたい
        // 左クリック、かつUI上でない、かつ敵ターンでない場合、かつ操作権がある
        if (Input.GetMouseButtonDown(0) && 
            !EventSystem.current.IsPointerOverGameObject() && 
            !_enemyTurnFlg )
            //&&
            //_turnManager.ActiveTurnCharacter.ActiveOperator)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// フェーズ遷移待ち時のコルーチン処理
    /// </summary>
    private void WaitForCorutine()
    {
        // 現在実行中のコルーチン処理がない場合は実行
        if (_currentCoroutine == null)
        {
            _currentCoroutine = StartCoroutine(ChangePhase(PHASE_WAIT_TIME, _nextPhase));
        }
    }

    /// <summary>
    /// 画面上から任意のブロックを選択し、選択したブロック情報を返す
    /// </summary>
    /// <returns></returns>
    private MapBlock GetSelectMapBlock()
    {
        GameObject obj = null;
        MapBlock mapBlock = null;
        RaycastHit hitInfo;

        // メインカメラから映るスクリーン上のx、y座標をrayに変換して保持
        // mousePositionが左上を0基準で、x、y座標を表す
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //// rayを飛ばして、何かにぶつかったら
        //if (Physics.Raycast(ray, out hitInfo))
        //{
        //    // 任意のブロックオブジェクトを取得
        //    obj = hitInfo.collider.gameObject;
        //    mapBlock = obj.GetComponent<MapBlock>();
        //}

        // rayを飛ばしてすべてのオブジェクト取得
        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            // 任意のブロックオブジェクトを取得
            obj = hit.collider.gameObject;
            mapBlock = obj.GetComponent<MapBlock>();

            // mapBlock情報を取得出来たら抜ける
            if (mapBlock != null)
            {
                break;
            }
        }

        return mapBlock;
    }

    // キャラクター指定していたらスキップ
    // 指定したマップブロック情報を設定する
    /// <summary>
    /// ブロック選択時に強調する
    /// </summary>
    private void SelectBlock()
    {
        MapBlock mapBlock = null;

        mapBlock = GetSelectMapBlock();

        if (mapBlock != null)
        {
            _selectingMapBlock = mapBlock;

            // 前回の強調状態は消したいため、マップ状態を初期化
            _mapManager.ClearSelectBlockMode();

            // 選択したブロックを強調
            mapBlock.EnphasizeBlock(DefineMapClass.SelectBlockMode.SELECT_BLOCK);

            //// ブロックの選択状態を更新
            //SetBlockStatus(mapBlock);
        }
    }

    /// <summary>
    /// 移動・攻撃・待機のメニューを表示する
    /// </summary>
    private void ShowCharacterMenu()
    {
        Character character = null;

        // 選択したマップブロックが取得できている場合
        if (_selectingMapBlock != null)
        {
            // 指定したブロックの座標を引数にして、同じ座標にキャラクターが居るか確認
            character = _characterManager.SearchCharacterByPosition(_selectingMapBlock.BlockPosX, _selectingMapBlock.BlockPosZ);

        }

        // 指定したブロック状にキャラクターが存在している場合
        if (character != null)
        {
            // 移動・攻撃・待機メニューを表示
            // ステータスウィンドウを表示する
            SelectPatternUI(PatternUI.PATTERN_COMMAND_STATUS_UI, character);

            // キャラクターが指定されている状態
            _selectingCharaFlg = true;
        }


        if (character == null)
        {
            SelectPatternUI(PatternUI.PATTERN_HIDE_COMMAND_STATUS_UI);
        }
    }

    /// <summary>
    /// 画面から「移動」選択時 : 指定キャラクターの移動範囲を表示する
    /// </summary>
    public void ShowMoveArea()
    {
        Character character = null;

        // 選択したマップブロックが取得できている場合
        if (_selectingMapBlock != null)
        {
            // 指定したブロックの座標を引数にして、同じ座標にキャラクターが居るか確認
            character = _characterManager.SearchCharacterByPosition(_selectingMapBlock.BlockPosX, _selectingMapBlock.BlockPosZ);
        }

        // 指定したブロック状にキャラクターが存在している場合
        if (character != null)
        {
            // 選択状態クリア
            _mapManager.ClearSelectBlockMode();

            // 選択キャラ情報を保持
            _selectingCharacter = character;

            //Debug.Log("キャラ名：" + character.GetCharactrerData.CharacterName);
            //Debug.Log("キャラのx座標：" + character.GetCharaPosX + "　キャラのz座標：" + character.GetCharaPosZ);

            // 指定キャラの移動範囲を取得する
            // 取得範囲は縦横
            _moveAllowArea = _mapManager.SearchMoveAreaByCharacter(character);

            foreach (MapBlock moveAreaBlock in _moveAllowArea)
            {
                // 選択したブロックを強調
                moveAreaBlock.EnphasizeBlock(DefineMapClass.SelectBlockMode.SELECT_MOVE);
            }

            // フェーズ切り替え
            //StartCoroutine(ChangePhase(2.0f, TurnPhase.PHASE_MOVE_TURN));
            ChangePhase(TurnPhase.PHASE_MOVE_TURN);
        }
        else
        {
            ClearSelectCharacter();
        }

       
    }

    // 指定マスへキャラクターを移動させる
    private void MoveCharacter(MapBlock mapBlock)
    {
        Debug.Log("MoveCharacter");

        // 移動の最終確認をする(GUI)
        // ★★★

        Debug.Log(mapBlock);

        // Containsで、リスト内に指定のデータが含まれているか確認する
        // 指定のデータ＝選択した移動先のブロック
        if (_moveAllowArea.Contains(mapBlock))
        {
            // 移動前のブロック上キャラ存在フラグを初期化
            _mapManager.SetUsedMapBlock(_selectingCharacter.GetCharaPosX, _selectingCharacter.GetCharaPosZ, false);

            // キャラクター移動
            _selectingCharacter.MovePosition(mapBlock.BlockPosX, mapBlock.BlockPosZ);

            // 移動先のブロックを使用済みにする
            // ★★これもUniRxでキャラクター側の座標が変わるタイミングで変更したい
            // これだと敵キャラ側にも同じ処理が必要

            // 移動先のブロック上キャラ存在フラグを設定
            _mapManager.SetUsedMapBlock(mapBlock.BlockPosX, mapBlock.BlockPosZ, true);

            // リスト初期化
            _moveAllowArea.Clear();

            // 選択状態クリア
            _mapManager.ClearSelectBlockMode();

            // 「移動」は実施済みにする
            _endMoveFlg = true;
        }
        else
        {

            Debug.Log("no mapblock");

        }

        /// 行動選択ウィンドウを隠す
        SelectPatternUI(PatternUI.PATTERN_HIDE_COMMAND_STATUS_UI);

        // フェーズ切り替え
        NextAction();

        //_selectingCharacter.ModifyPosition();
    }

    // 指定キャラクターの攻撃範囲を表示させる
    public void ShowAttackArea()
    {
        Character character = null;

        // 選択したマップブロックが取得できている場合
        if (_selectingMapBlock != null)
        {
            character = _characterManager.SearchCharacterByPosition(_selectingMapBlock.BlockPosX, _selectingMapBlock.BlockPosZ);
        }

        Debug.Log("ShowAttackArea" + "X座標：" + character.GetCharaPosX + " Z座標：" + character.GetCharaPosZ);

        // 指定したブロック状にキャラクターが存在している場合
        if (character != null)
        {
            // 選択状態クリア
            _mapManager.ClearSelectBlockMode();

            // 選択キャラ情報を保持
            _selectingCharacter = character;

            _atkAllowArea = _mapManager.SearchAttackAreaByCharacter(_selectingCharacter, 
                                                                    _selectingCharacter.GetCharaPosX,
                                                                    _selectingCharacter.GetCharaPosZ);

            foreach (MapBlock atkAreaBlock in _atkAllowArea)
            {
                // 選択したブロックを強調
                atkAreaBlock.EnphasizeBlock(DefineMapClass.SelectBlockMode.SELECT_ATTACK);
            }

            // フェーズ切り替え
            _currentCoroutine = StartCoroutine(ChangePhase(PHASE_WAIT_TIME, TurnPhase.PHASE_ATK_TURN));

            //SetPhaseWait(TurnPhase.PHASE_ATK_TURN);
            ChangePhase(TurnPhase.PHASE_ATK_TURN);

        }
        else
        {
            ClearSelectCharacter();
        }
    }

    /// <summary>
    /// 指定したブロック状のキャラを攻撃する
    /// </summary>
    /// <param name="mapBlock"></param>
    private void AttackCharacter(MapBlock mapBlock)
    {
        //Debug.Log("AttackCharacter");

        Debug.Log(mapBlock);

        // Containsで、リスト内に指定のデータが含まれているか確認する
        // 指定のデータ＝画面上から選択したブロック
        if (_atkAllowArea.Contains(mapBlock))
        {
            // 指定したブロック状にの座標を引数にして、同じ座標にキャラクターが居るか確認
            var targetCharacter = _characterManager.SearchCharacterByPosition(mapBlock.BlockPosX, mapBlock.BlockPosZ);

            if (targetCharacter != null)
            {
                Debug.Log("攻撃対象キャラクター名：" + targetCharacter.GetCharactrerData.CharacterName);

                // 指定キャラクターが対象キャラクターへ攻撃を行う
                Attack(targetCharacter, _selectingCharacter);

                // 攻撃終了フラグ設定
                _endAttackFlg = true;
            }

            // リスト初期化
            _atkAllowArea.Clear();

            // 選択状態クリア
            _mapManager.ClearSelectBlockMode();
        }

        SelectPatternUI(PatternUI.PATTERN_HIDE_COMMAND_STATUS_UI);

        NextAction();
    }


    /// <summary>
    /// 指定キャラクターを攻撃
    /// </summary>
    /// <param name="target"></param>
    /// <param name="selectingCharacter"></param>
    public void Attack(Character target, Character selectingCharacter)
    {
        // 攻撃対象となるキャラのコライダーを有効にする
        target.gameObject.GetComponent<CapsuleCollider>().enabled = true;

        // 攻撃相手の方向を向く
        selectingCharacter.TurnToTarget(target.GetCharaPosX, target.GetCharaPosZ);

        // 攻撃モーション
        // キャラクターごとにモーションを分ける？？
        selectingCharacter.AttackMotion();
    }


    /// <summary>
    /// 次のフェーズ設定を行う(指定時間あり)
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="phase"></param>
    /// <returns></returns>
    IEnumerator ChangePhase(float waitTime, TurnPhase phase)
    {
        // 指定時間待ち
        yield return new WaitForSeconds(waitTime);

        Debug.Log("コルーチン処理時間：" + waitTime + "次ターンフェーズ：" + phase) ;

        // プレイヤーまたは敵ターンフェーズに遷移する場合は、UIのターンリストも更新する
        if ((phase == TurnPhase.PHASE_MY_TURN) || (phase == TurnPhase.PHASE_ENEMY_TURN))
        {
            _turnManager.ChangeTurnListUI();
        }

        _currentPhase = phase;

        // コルーチン処理が終了したので実行中フラグ初期化
        _currentCoroutine = null;
    }

    /// <summary>
    /// 次のフェーズ設定を行う(すぐに設定する)
    /// </summary>
    /// <param name="phase"></param>
    public void ChangePhase(TurnPhase phase)
    {
        _currentPhase = phase;
    }


    /// <summary>
    /// 移動、攻撃選択時に呼ばれる処理
    /// </summary>
    private void NextAction()
    {
        // 「移動」または「攻撃」が終了していない場合
        if (!_endMoveFlg || !_endAttackFlg)
        {
            // フェーズ切り替え(行動選択状態)
            _currentCoroutine = StartCoroutine(ChangePhase(PHASE_WAIT_TIME, TurnPhase.PHASE_MY_TURN));

            // フェーズ遷移待ちに変更
            SetPhaseWait(TurnPhase.PHASE_MY_TURN);
            //ChangePhase(TurnPhase.PHASE_MY_TURN);
        }
        else
        {
            //「待機」のみなので、現在のキャラクターのターンを終了とみなす
            _guiManager.EndCharacterTurn();
            _currentCoroutine = StartCoroutine(ChangePhase(PHASE_WAIT_TIME, TurnPhase.PHASE_NEXT_TURN));

            // フェーズ遷移待ちに変更
            SetPhaseWait(TurnPhase.PHASE_NEXT_TURN);
            //ChangePhase(TurnPhase.PHASE_NEXT_TURN);
        }
    }

    /// <summary>
    /// 次のプレイヤーへフェーズを回す
    /// </summary>
    private void NextPlayerPhase()
    {
        // 次の行動開始キャラが敵の場合
        if (_turnManager.ActiveTurnCharacter.GetCharactrerData.IsEnemy)
        {
            // 敵行動開始フェーズ設定
            _currentCoroutine = StartCoroutine(ChangePhase(PHASE_WAIT_TIME, TurnPhase.PHASE_ENEMY_TURN));
            SetPhaseWait(TurnPhase.PHASE_ENEMY_TURN);
            _enemyTurnFlg = true;
            Debug.Log("EnemyTurn START");
        }
        else
        {
            // プレイヤー行動開始フェーズ設定
            _currentCoroutine = StartCoroutine(ChangePhase(PHASE_WAIT_TIME, TurnPhase.PHASE_MY_TURN));
            SetPhaseWait(TurnPhase.PHASE_MY_TURN);
            _enemyTurnFlg = false;
            Debug.Log("PlayerTurn START");
        }
    }

    /// <summary>
    /// ターン待ち状態にするための設定
    /// </summary>
    private void SetPhaseWait(TurnPhase phase)
    {
        // 次に遷移させたいフェーズを設定
        _nextPhase = phase;

        // フェーズ遷移待ち
        ChangePhase(TurnPhase.PHASE_WAIT);

        // 現在のフェーズを設定
        _currentPhase = TurnPhase.PHASE_WAIT;
    }

    /// <summary>
    /// 操作キャラクター情報を初期化する
    /// </summary>
    public void ClearSelectCharacter()
    {
        _selectingCharacter = null;
    }

    // マップブロック上のキャラ指定状態を初期化する
    public void ClearSelectCharacterFlg()
    {
        _selectingCharaFlg = false;
    }


    /// <summary>
    /// 行動終了時に移動、攻撃フラグを初期化する
    /// </summary>
    public void ClearCommandMenuFlg()
    {
        Debug.Log("ClearCommandMenuFlg");
        _endAttackFlg = false;
        _endMoveFlg = false;

        // 「待機」が選択されたので、現在のキャラクターのターンを終了する
        // 行動終了なので、特にコルーチン処理は設けない
        //StartCoroutine(ChangePhase(PHASE_WAIT_TIME, TurnPhase.PHASE_NEXT_TURN));
        ChangePhase(TurnPhase.PHASE_NEXT_TURN);
    }

    /// <summary>
    /// 移動・攻撃・待機メニューや、ステータスウィンドウを表示する
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="character"></param>
    /// <param name="damage"></param>
    public void SelectPatternUI(PatternUI pattern, Character character = null, int damage = 0)
    {
        Debug.Log(pattern);

        switch (pattern)
        {
            // PHASE_MY_TURNでコール
            case PatternUI.PATTERN_COMMAND_STATUS_UI:
                
                // 移動・攻撃・待機メニューを表示
                _guiManager.SlideCommandMenu();

                // ステータス詳細を設定する
                _statusManager.SetStatusInfo(character);

                // ステータスウィンドウを表示する
                _guiManager.SlideStatusWindow();

                break;

            case PatternUI.PATTERN_STATUS_UI:
                _statusManager.SetStatusInfo(character);
                _guiManager.SlideStatusWindow();
                break;

            case PatternUI.PATTERN_STATUS_DAMAGE_UI:

                // 攻撃を受けたキャラクターのステータスを表示する
                _statusManager.SetStatusInfo(character);

                _guiManager.SlideStatusWindow();

                // 攻撃を受けたキャラクターのHPスライダーを変更する
                _statusManager.ChangeHpSlider(character, damage);

                // ステータスウィンドウを揺らす
                _guiManager.ShakeStatusWindow();
                _cameraManager.ShakeCamera();
                break;

            case PatternUI.PATTERN_HIDE_COMMAND_STATUS_UI:
                // コマンドメニュー、ステータスウィンドウを隠す
                _guiManager.HideCommandMenu();
                break;
        }
    }

    public void ShowScrollViewCharacterStatus(int uniqueId)
    {
        Character character = null;

        character = _characterManager.SearchUniqueIdCharacter(uniqueId);

        if (character != null)
        {
            _statusManager.SetStatusInfo(character);
            _guiManager.SlideStatusWindow();
        }

    }
}
