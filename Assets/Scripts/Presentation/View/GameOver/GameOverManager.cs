using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Zenject;

public class GameOverManager : MonoBehaviour
{
    // ゲーム情報
    [Inject] private GameData _gameData;
    // AudioManagerオブジェクト
    [SerializeField] private GameObject _audioManagerObj;

    // AudioManager
    private AudioManager _audioManager;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _audioManager = _audioManagerObj.GetComponent<AudioManager>();
        // 効果音の実行
        _audioManager.PlaySound(SoundType.GameOver);

        // ゲーム情報のリセット
        _gameData.StageNumber = 1;
        _gameData.PlayerStock = 3;

        _gameData.Save();

        Observable
            .Timer(TimeSpan.FromMilliseconds(5000))
            .Subscribe(_ =>
            {
                // さいしょから
                SceneManager.LoadScene("TitleScene");
            });
    }
}
