using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// ゲームの進捗に関するデータのモデル
/// </summary>
[System.Serializable]
public class GameData
{
    // ステージ数
    private int _stageNumber;
    public int StageNumber
    {
        get => _stageNumber;
        set { _stageNumber = value; }
    }

    // 残機数
    private int _playerStock;
    public int PlayerStock
    {
        get => _playerStock;
        set { _playerStock = value; }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="stageNumber"></param>
    /// <param name="playerStock"></param>
    public GameData(int stageNumber, int playerStock)
    {
        _stageNumber = stageNumber;
        _playerStock = playerStock;
    }

    /// <summary>
    /// 残機を増やす
    /// </summary>
    /// <param name="value"></param>
    public void AddStock(int value)
    {
        _playerStock += value;
    }

    /// <summary>
    /// 残機を減らす
    /// </summary>
    /// <param name="value"></param>
    public void CutStock(int value)
    {
        _playerStock -= value;
    }

    /// <summary>
    /// ゲーム情報の読み込み
    /// </summary>
    public void Load()
    {
        string dataPath = null;
#if UNITY_EDITOR
        dataPath = Application.dataPath + "/SaveData/save.bin";
#elif UNITY_STANDALONE
        dataPath = Application.dataPath + "/../SaveData/save.bin";
#endif

        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            FileStream file = File.Open(dataPath, FileMode.Open);
            try
            {
                GameData loadedData = (GameData)binaryFormatter.Deserialize(file);
                this._stageNumber = loadedData.StageNumber;
                this._playerStock = loadedData._playerStock;
            }
            finally
            {

                if (file != null)
                {
                    file.Close();
                }
            }
        }
    }

    /// <summary>
    /// セーブデータがあるかチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckSaveData()
    {
        string dataPath = null;
#if UNITY_EDITOR
        dataPath = Application.dataPath + "/SaveData/save.bin";
#elif UNITY_STANDALONE
        dataPath = Application.dataPath + "/../SaveData/save.bin";
#endif

        return File.Exists(dataPath);
    }

    /// <summary>
    /// ゲーム情報の書き込み
    /// </summary>
    public void Save()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        // セーブデータ用ディレクトリがあるか確認、なければ作る
        string dirPath = null;
#if UNITY_EDITOR
        dirPath = Application.dataPath + "/SaveData";
#elif UNITY_STANDALONE
            dirPath = Application.dataPath + "/../SaveData";
#endif

        if (System.IO.Directory.Exists(dirPath) == false)
        {
            Directory.CreateDirectory(dirPath);
        }

        FileStream file = File.Create(dirPath + "/save.bin");

        try
        {
            binaryFormatter.Serialize(file, this);
        }
        finally
        {

            if (file != null)
            {
                file.Close();
            }
        }
    }
}
