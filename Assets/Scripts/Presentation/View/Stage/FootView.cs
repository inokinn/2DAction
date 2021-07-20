using System;
using UnityEngine;
using UniRx;

/// <summary>
/// 接地判定用のView
/// </summary>
public class FootView : MonoBehaviour
{
    // 接地判定装置に触れているオブジェクトの数
    private int _enterCount = 0;
    // 接地判定のSubject
    private Subject<bool> _isGroundSubject = new Subject<bool>();
    // 接地判定のイベント購読側
    public IObservable<bool> OnGroundChanged => _isGroundSubject;

    /// <summary>
    /// オブジェクトに衝突した際
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Enemy")
        {
            _enterCount += 1;
            this.PublishOnGround();
        }
    }

    /// <summary>
    /// オブジェクトから離れた際
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag != "Enemy")
        {
            _enterCount -= 1;
            this.PublishOnGround();
        }
    }

    /// <summary>
    /// 接地状態を通知
    /// </summary>
    public void PublishOnGround()
    {
        _isGroundSubject.OnNext(_enterCount > 0);
    }
}
