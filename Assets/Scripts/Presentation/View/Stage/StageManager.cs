using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Zenject;

/// <summary>
/// ステージのマネージャクラス
/// </summary>
public class StageManager : MonoBehaviour
{
    // ゲーム情報
    [Inject] private GameData _gameData;
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
            // 残機を減らす
            _gameData.CutStock(1);

            bool isGameOver = false;
            if (0 > _gameData.PlayerStock)
            {
                // ゲーム情報のリセット
                _gameData.StageNumber = 1;
                _gameData.PlayerStock = 3;
                isGameOver = true;
            }
            _gameData.Save();

            Observable
                .Timer(TimeSpan.FromMilliseconds(2000))
                .Subscribe(_ =>
                {
                    if (isGameOver)
                    {
                        // ゲームオーバー
                        SceneManager.LoadScene("GameOverScene");
                    }
                    else
                    {
                        // リスタート
                        SceneManager.LoadScene("IntroScene");
                    }
                });
        })
        .AddTo(this);
    }
}
