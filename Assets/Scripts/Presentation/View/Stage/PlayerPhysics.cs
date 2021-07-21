using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerPhysics : MonoBehaviour
{
    // 弾丸のチカラ
    private const float _WeaponForce = 550.0f;
    // 弾丸の開始位置
    private const float _WeaponForward = 0.6f;
    // 弾丸のY座標
    private const float _WeaponY = 1.0f;

    // 移動速度
    [SerializeField] private float _speed = 0.1f;
    // ジャンプ時に上方向に掛かる力の強さ
    [SerializeField] private float _jumpPower = 400f;
    // 無敵時間(ミリ秒)
    [SerializeField] private long _invincibleTime = 3000;
    // 接地判定オブジェクト
    [SerializeField] private GameObject _foot;
    // 表示用オブジェクト
    [SerializeField] private GameObject _playerView;
    // 弾丸
    [SerializeField] private GameObject _weapon;
    // AudioManagerオブジェクト
    [SerializeField] private GameObject _audioManagerObj;

    // ジャンプ中フラグ
    private bool _isGround = false;
    // アニメーター
    private Animator _animator;
    // リジッドボディ
    private Rigidbody _rigitBody;
    // 接地判定用オブジェクトのスクリプト
    private FootView _footView;
    // AudioManager
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _animator = _playerView.GetComponent<Animator>();
        _rigitBody = transform.GetComponent<Rigidbody>();
        _footView = _foot.GetComponent<FootView>();
        _audioManager = _audioManagerObj.GetComponent<AudioManager>();

        // 接地判定のイベント
        _footView.OnGroundChanged.Subscribe(isGround =>
        {
            _isGround = isGround;
            if (_isGround)
            {
                _animator.SetBool("jumping", false);
            }
        })
        .AddTo(this);

        // ジャンプボタンの入力
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Jump"))
            .Where(_ => _isGround)
            .ThrottleFirst(new System.TimeSpan(100 * TimeSpan.TicksPerMillisecond))
            .Subscribe(_ =>
            {
                this.Jump();
            });

        // 攻撃ボタンの入力
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Fire1"))
            .ThrottleFirst(new System.TimeSpan(300 * TimeSpan.TicksPerMillisecond))
            .Subscribe(_ =>
            {
                this.Attack();
            })
            .AddTo(this);

        // 被弾イベント
        this.OnTriggerEnterAsObservable()
            .Where(other => other.gameObject.tag == "Enemy")
            .ThrottleFirst(new System.TimeSpan(_invincibleTime * TimeSpan.TicksPerMillisecond))
            .Subscribe(other =>
            {
                _audioManager.PlaySound(SoundType.PlayerDamage);
            })
            .AddTo(this);
    }

    void FixedUpdate()
    {
        // 入力による操作
        this.Run();
    }

    /// <summary>
    /// スティック入力で走る
    /// </summary>
    private void Run()
    {
        float x = Input.GetAxis("Horizontal");
        float addX = _speed * x;

        // キー入力に合わせてキャラクターを移動する
        Vector3 newPosition = transform.position + (new Vector3(addX, 0, 0) * Time.deltaTime);
        _rigitBody.MovePosition(newPosition);
        _animator.SetBool("walking", addX != 0);
        // 移動する方向に顔を向ける
        transform.LookAt(newPosition);
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    private void Jump()
    {
        if (_isGround)
        {
            _animator.SetBool("jumping", true);
            _rigitBody.AddForce(new Vector3(0, _jumpPower, 0));
        }
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    private void Attack()
    {
        _animator.SetTrigger("attackStart");
        GameObject weapon = Instantiate(_weapon).gameObject as GameObject;
        weapon.transform.position = new Vector3(0, _WeaponY, 0) + transform.position + transform.forward * _WeaponForward;
        weapon.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * _WeaponForce);
        _audioManager.PlaySound(SoundType.Weapon);
    }
}