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

    // プレイヤーのスクリプト
    private PlayerPhysics _playerPhisics;
    // カメラ
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _playerPhisics = _player.GetComponent<PlayerPhysics>();
        _camera = gameObject.GetComponent<Camera>();
        gameObject.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 2f, _player.transform.position.z - _CameraDistance);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 2f, _player.transform.position.z - _CameraDistance);
        float newX = gameObject.transform.position.x;
        if (_camera.ViewportToWorldPoint(new Vector3(0, 1, 12)).x <= _playerPhisics._leftTop.x || _camera.ViewportToWorldPoint(new Vector3(1, 0, 12)).x >= _playerPhisics._rightBottom.x)
        {
            newX = currentPos.x;
        }
        float newY = gameObject.transform.position.y;
        if (_camera.ViewportToWorldPoint(new Vector3(0, 1, 12)).y >= _playerPhisics._leftTop.y || _camera.ViewportToWorldPoint(new Vector3(1, 0, 12)).y <= _playerPhisics._rightBottom.y)
        {
            newY = currentPos.y;
        }
        gameObject.transform.position = new Vector3(newX, newY, gameObject.transform.position.z);
    }
}
