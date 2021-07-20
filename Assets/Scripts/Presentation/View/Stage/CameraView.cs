using UnityEngine;

/// <summary>
/// メインカメラのView
/// </summary>
public class CameraView : MonoBehaviour
{
    // プレイヤーとカメラのデフォルト距離
    private const float _CameraDistance = 12f;

    // プレイヤーのゲームオブジェクト
    [SerializeField] private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 2, _player.transform.position.z - _CameraDistance);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 2, _player.transform.position.z - _CameraDistance);
    }
}
