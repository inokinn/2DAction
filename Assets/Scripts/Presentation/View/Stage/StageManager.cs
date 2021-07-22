using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

/// <summary>
/// ステージのマネージャクラス
/// </summary>
public class StageManager : MonoBehaviour
{
    // プレイヤーのゲームオブジェクト
    [SerializeField] private GameObject _player;
    // 接地判定用のゲームオブジェクト
    [SerializeField] private GameObject _foot;
    // プレイヤーのHPバーのゲームオブジェクト
    [SerializeField] private GameObject _playerHpBar;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーと接地判定用オブジェクト同士の接触は無視する
        Physics.IgnoreCollision(_player.GetComponent<CapsuleCollider>(), _foot.GetComponent<BoxCollider>());

        // ダメージ時のイベントを購読
        _player.GetComponent<PlayerPhysics>().CurrentHP.Subscribe(currentHP =>
        {
            // HPバーを操作
            var currentHpBarRectTransform = _playerHpBar.GetComponent<RectTransform>();
            float currentHpPercent = (float)currentHP / (float)_player.GetComponent<PlayerPhysics>().MaxHP;
            currentHpBarRectTransform.sizeDelta = new Vector2(currentHpBarRectTransform.sizeDelta.x, 180 * currentHpPercent);
        })
        .AddTo(this);

        // 死亡時のイベント
        _player.GetComponent<PlayerPhysics>().Death.Subscribe(_ =>
        {
            Observable
                .Timer(TimeSpan.FromMilliseconds(2000))
                .Subscribe(_ =>
                {
                    // リスタート
                    SceneManager.LoadScene("SampleScene");
                });
        })
        .AddTo(this);
    }
}
