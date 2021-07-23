using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using Zenject;

public class IntroManager : MonoBehaviour
{
    // ゲーム情報
    [Inject] private GameData _gameData;
    // 残機数表示用テキスト
    [SerializeField] private Text _stockText;

    // Start is called before the first frame update
    void Start()
    {
        _stockText.text = _gameData.PlayerStock.ToString();

        Observable
            .Timer(TimeSpan.FromMilliseconds(2000))
            .Subscribe(_ =>
            {
                // ステージへ遷移
                SceneManager.LoadScene("SampleScene");
            });
    }
}
