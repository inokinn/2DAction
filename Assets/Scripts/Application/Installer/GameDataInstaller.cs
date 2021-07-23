using UnityEngine;
using Zenject;

/// <summary>
/// ゲームデータをDIするインストーラ
/// </summary>
public class GameDataInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameData>().FromMethod(InitializeGameData).AsSingle();
    }

    /// <summary>
    /// ゲーム情報を初期化
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private GameData InitializeGameData(InjectContext context)
    {
        // インスタンス生成
        return new GameData(1, 3);
    }
}