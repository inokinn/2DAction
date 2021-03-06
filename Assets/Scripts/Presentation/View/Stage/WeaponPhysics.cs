using UnityEngine;

/// <summary>
/// 弾丸の物理演算
/// </summary>
public class WeaponPhysics : MonoBehaviour
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
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
