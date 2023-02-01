using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Damaged : MonoBehaviour
{
    private Animator _animator;
    private TurnManager _turnManager;
    private GameManager _gameManager;
    private Character _target;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Damaged Function Start");
        _gameManager = GameManager.Instance;
        _turnManager = TurnManager.Instance;
        _animator = GetComponent<Animator>();
        _target = GetComponent<Character>();

        // コライダーの半径を2以上にすればヒットする
        this.OnTriggerEnterAsObservable()
            .Where(colObj => colObj.gameObject.CompareTag("weapon"))
            .Subscribe(_ =>
            {
                Debug.Log("OnTriggerEnter : UniRx");
                _animator.SetTrigger("Hit");

                // ダメージ計算
                var damage = _turnManager.ActiveTurnCharacter.GetCharacterData.Atk;  //_target.GetCharactrerData.Def - _turnManager.ActiveTurnCharacter.GetCharactrerData.Atk;

                Debug.Log("自分の防御力：" + _target.GetCharacterData.Def + "相手の攻撃力：" + _turnManager.ActiveTurnCharacter.GetCharacterData.Atk);

                // HITモーションのタイミングで、UIを揺らす
                // ダメージ値は絶対値で計算
                _gameManager.SelectPatternUI(GameManager.PatternUI.PATTERN_STATUS_DAMAGE_UI, _target, Mathf.Abs(damage));

                // コライダーは無効状態に戻す
                // 対象攻撃時に一瞬有効に戻す
                this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            });
    }
}
