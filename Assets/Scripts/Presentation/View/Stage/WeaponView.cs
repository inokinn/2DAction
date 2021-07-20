using UnityEngine;

/// <summary>
/// 弾丸を表すビュー
/// </summary>
public class WeaponView : MonoBehaviour
{
    // 弾丸の距離的寿命
    private const float _LongestDistance = 25.0f;

    // 弾丸の初期位置
    private Vector3 _startPosition;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _startPosition = transform.position;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // 一定の距離を離れたら消滅
        if (Vector3.Distance(transform.position, _startPosition) > _LongestDistance)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// オブジェクトが重なった際
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
