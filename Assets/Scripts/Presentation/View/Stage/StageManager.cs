using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージのマネージャクラス
/// </summary>
public class StageManager : MonoBehaviour
{
    // プレイヤーのゲームオブジェクト
    [SerializeField] private GameObject player;
    // 接地判定用のゲームオブジェクト
    [SerializeField] private GameObject foot;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーと接地判定用オブジェクト同士の接触は無視する
        Physics.IgnoreCollision(player.GetComponent<CapsuleCollider>(), foot.GetComponent<BoxCollider>());
    }
}
