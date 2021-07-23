using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UniRx;
using Zenject;

public class TitleManager : MonoBehaviour
{
    // ゲーム情報
    [Inject] private GameData _gameData;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _continueButton;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (_gameData.CheckSaveData())
        {
            // コンティニューを初期カーソル位置とする
            EventSystem.current.SetSelectedGameObject(_continueButton.gameObject);
        }
        else
        {
            // GAME STARTを初期カーソル位置とする
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            // コンティニューを非アクティブにする
            _continueButton.interactable = false;
        }

        _startButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                // ゲーム情報のリセット
                _gameData.StageNumber = 1;
                _gameData.PlayerStock = 3;

                // セーブデータの作成
                _gameData.Save();

                // さいしょから
                SceneManager.LoadScene("IntroScene");
            });

        _continueButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                // ゲーム情報をロード
                _gameData.Load();
                // つづきから
                SceneManager.LoadScene("IntroScene");
            });
    }
}
