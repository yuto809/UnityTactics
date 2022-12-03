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
        _gameManager = GameManager.instance;
        _turnManager = TurnManager.instance;
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
                var damage = _turnManager.ActiveTurnCharacter.GetCharactrerData.Atk;  //_target.GetCharactrerData.Def - _turnManager.ActiveTurnCharacter.GetCharactrerData.Atk;

                Debug.Log("自分の防御力：" + _target.GetCharactrerData.Def + "相手の攻撃力：" + _turnManager.ActiveTurnCharacter.GetCharactrerData.Atk);

                // 絶対値を取る
                if (damage < 0)
                {
                    damage *= -1;
                }

                // HITモーションのタイミングで、UIを揺らす
                _gameManager.SelectPatternUI(GameManager.PatternUI.PATTERN_STATUS_DAMAGE_UI, _target, damage);

                // コライダーは無効状態に戻す
                // 対象攻撃時に一瞬有効に戻す
                this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            });
    }
}
