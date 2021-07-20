using UnityEngine;

/// <summary>
/// 弾丸を表すビュー
/// </summary>
public class WeaponView : MonoBehaviour
{
    /// <summary>
    /// オブジェクトが重なった際
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Weapon")
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
