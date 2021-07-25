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
    // AudioManagerオブジェクト
    [SerializeField] private GameObject _audioManagerObj;

    // リジッドボディ
    private Rigidbody _rigitBody;
    // AudioManager
    private AudioManager _audioManager;
    // Camera
    private Camera _camera;
    // レンダラー
    private Renderer _renderer;

    // 現在体力
    private int _currentHp;
    // 1フレーム前の位置
    private Vector3 _prevPosition;
    // 初期位置
    private Vector3 _defaultPosition;
    // 初期向き
    private Vector3 _defaultForward;
    // リポップ直後か
    private bool _isRepoped = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _rigitBody = transform.GetComponent<Rigidbody>();
        _audioManager = _audioManagerObj.GetComponent<AudioManager>();
        _camera = _audioManagerObj.GetComponent<Camera>();
        _renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        _defaultPosition = _rigitBody.position;
        _defaultForward = transform.forward;

        _currentHp = _maxHp;

        // 被弾イベント
        this.OnTriggerEnterAsObservable()
            .Where(other => other.gameObject.tag == "Weapon")
            .ThrottleFirst(new System.TimeSpan(_invincibleTime * TimeSpan.TicksPerMillisecond))
            .Subscribe(other =>
            {
                _currentHp -= 1;
                _audioManager.PlaySound(SoundType.EnemyDamage);

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
        if (_renderer.isVisible)
        {
            // 画面内にいる時のみ動く
            this.Move();
            _isRepoped = false;
        }
        else
        {
            // 画面外に出た際、一旦リポップを試みるが、リポップ先が画面外だった場合はリポップ前の位置に戻る
            Vector3 currentPosition = _rigitBody.position;
            _rigitBody.MovePosition(_defaultPosition);
            _isRepoped = true;
            if (_camera.ViewportToWorldPoint(new Vector3(0, 1, 12)).x <= _rigitBody.position.x && _camera.ViewportToWorldPoint(new Vector3(1, 0, 12)).x >= _rigitBody.position.x
            && _camera.ViewportToWorldPoint(new Vector3(0, 1, 12)).y >= _rigitBody.position.y && _camera.ViewportToWorldPoint(new Vector3(1, 0, 12)).y <= _rigitBody.position.y)
            {
                _rigitBody.MovePosition(currentPosition);
                _isRepoped = false;
            }
            else
            {
                // リポップ直後は方向を初期化
                transform.forward = _defaultForward;
            }
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        Vector3 newPosition = transform.position + transform.forward * _speed * Time.deltaTime;
        _rigitBody.MovePosition(newPosition);

        // 動けなくなった場合、反転する(リポップ直後のフレームは無視)
        if (_prevPosition != null && _prevPosition == transform.position && !_isRepoped)
        {
            transform.LookAt(-transform.forward);
        }
        _prevPosition = transform.position;
    }
}
