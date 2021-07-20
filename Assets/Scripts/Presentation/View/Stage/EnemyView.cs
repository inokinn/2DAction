using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 敵ビュー
/// </summary>
public class EnemyView : MonoBehaviour
{
    // 移動速度
    [SerializeField] private float _speed = 2.0f;
    // 最大体力
    [SerializeField] private int _maxHp = 3;
    // 無敵時間(ミリ秒)
    [SerializeField] private long _invincibleTime = 0;
    // 敵のダメージ音
    [SerializeField] private AudioClip _enemyDamageSound;

    // オーディオソース
    private AudioSource _audioSource;
    // リジッドボディ
    private Rigidbody _rigitBody;

    // 現在体力
    private int _currentHp;
    // 1フレーム前の位置
    private Vector3 _prevPosition;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _rigitBody = transform.GetComponent<Rigidbody>();

        _currentHp = _maxHp;

        // 被弾イベント
        this.OnTriggerEnterAsObservable()
            .Where(other => other.gameObject.tag == "Weapon")
            .ThrottleFirst(new System.TimeSpan(_invincibleTime * TimeSpan.TicksPerMillisecond))
            .Subscribe(other =>
            {
                _currentHp -= 1;
                _audioSource.PlayOneShot(_enemyDamageSound);

                if (_currentHp <= 0)
                {
                    Destroy(gameObject);
                }
            })
            .AddTo(this);
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        this.Move();
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        Vector3 newPosition = transform.position + transform.forward * _speed * Time.deltaTime;
        _rigitBody.MovePosition(newPosition);

        // 動けなくなった場合、反転する
        if (_prevPosition != null && _prevPosition == transform.position)
        {
            transform.LookAt(-transform.forward);
        }
        _prevPosition = transform.position;
    }
}
