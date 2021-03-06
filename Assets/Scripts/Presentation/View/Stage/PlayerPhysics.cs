using System;
using System.Collections;
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
    // ノックバックのチカラ
    private const float _KnockBackForce = 160.0f;
    // ノックバックの時間(ミリ秒)
    private const float _KnockBackTime = 400.0f;
    // プレイヤーの最大HP
    private const int _MaxHP = 4;
    public int MaxHP
    {
        get => _MaxHP;
    }

    // 画面端(左上)
    [SerializeField]
    public Vector3 _leftTop = new Vector3(0, 9.3f, 13);
    // 画面端(左上)
    [SerializeField]
    public Vector3 _rightBottom = new Vector3(52, -4.5f, 13);
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
    // 点滅中時間
    private float _flashTime = 0;
    // 無敵開始時間
    private float _invincibleStartTime = 0;
    // メッシュ1
    private GameObject _mesh1;
    // メッシュ2
    private GameObject _mesh2;
    // ダメージ中
    private bool _isDamage = false;
    // ポーズ中
    private bool _isPausing = false;

    // 現在HP
    private ReactiveProperty<int> _currentHP = new ReactiveProperty<int>();
    public ReactiveProperty<int> CurrentHP
    {
        get => _currentHP;
    }
    // 死を通知するSubject
    private Subject<bool> _deathSubject = new Subject<bool>();
    // 死のイベント購読側
    public IObservable<bool> Death => _deathSubject;
    // クリアを通知するSubject
    private Subject<bool> _clearSubject = new Subject<bool>();
    // クリアのイベント購読側
    public IObservable<bool> Clear => _clearSubject;

    // Start is called before the first frame update
    void Start()
    {
        _animator = _playerView.GetComponent<Animator>();
        _rigitBody = transform.GetComponent<Rigidbody>();
        _footView = _foot.GetComponent<FootView>();
        _audioManager = _audioManagerObj.GetComponent<AudioManager>();
        _mesh1 = _playerView.transform.Find("Character1_Reference").gameObject;
        _mesh2 = _playerView.transform.Find("mesh_root").gameObject;
        _currentHP.Value = _MaxHP;

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
            .Where(_ => !_isDamage && !_isPausing)
            .ThrottleFirst(new System.TimeSpan(100 * TimeSpan.TicksPerMillisecond))
            .Subscribe(_ =>
            {
                this.Jump();
            })
            .AddTo(this);

        // 攻撃ボタンの入力
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Fire1"))
            .Where(_ => !_isDamage && !_isPausing)
            .ThrottleFirst(new System.TimeSpan(300 * TimeSpan.TicksPerMillisecond))
            .Subscribe(_ =>
            {
                this.Attack();
            })
            .AddTo(this);

        // 被弾イベント
        _playerView.GetComponent<PlayerView>().OnTriggerStayAsObservable()
            .Where(other => other.gameObject.tag == "Enemy")
            .ThrottleFirst(new System.TimeSpan(_invincibleTime * TimeSpan.TicksPerMillisecond))
            .Subscribe(other =>
            {
                // ダメージを受けたことを通知
                // 現状、ダメージは1で決め打ちにしてあるが将来的に攻撃によって変化させたい
                _currentHP.Value -= 1;

                // HPが0以下になったら死
                if (0 >= _currentHP.Value)
                {
                    this.DeathEffect();
                }
                // HPが1以上あるならダメージエフェクト
                else
                {
                    // 効果音
                    _audioManager.PlaySound(SoundType.PlayerDamage);

                    // 点滅
                    _isDamage = true;
                    StartCoroutine(FlashCoroutine());

                    // ノックバック
                    Vector3 distination = (transform.position - other.transform.position).normalized;
                    _rigitBody.AddForce(distination * 160);
                }
            })
            .AddTo(this);

        // クリアイベント
        _playerView.GetComponent<PlayerView>().OnTriggerStayAsObservable()
            .Where(other => other.gameObject.tag == "ClearFlag")
            .Subscribe(other =>
            {
                // BGMを停止
                _audioManager.AudioStop();

                // 効果音
                _audioManager.PlaySound(SoundType.StageClear);
                gameObject.SetActive(false);

                // クリアを通知
                _clearSubject.OnNext(true);
            })
            .AddTo(this);

        // ポーズ
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Pause"))
            .Subscribe(_ =>
            {
                if (Time.timeScale == 0)
                {
                    _isPausing = false;
                    Time.timeScale = 1;
                    _audioManager.PlaySound(SoundType.Pause);
                }
                else
                {
                    _isPausing = true;
                    Time.timeScale = 0;
                    _audioManager.PlaySound(SoundType.Pause);
                }
            })
            .AddTo(this);
    }

    void FixedUpdate()
    {
        // 入力による操作
        this.Run();

        // 落下死してないか確認
        this.PublishFall();
    }

    /// <summary>
    /// 落下死をチェックし、死んだら通知
    /// </summary>
    private void PublishFall()
    {
        if (_rigitBody.position.y <= _rightBottom.y - 2)
        {
            this.DeathEffect();
        }
    }

    /// <summary>
    /// 死亡時エフェクト
    /// </summary>
    private void DeathEffect()
    {
        // BGMを停止
        _audioManager.AudioStop();

        // 効果音
        _audioManager.PlaySound(SoundType.Death);
        gameObject.SetActive(false);

        // 死を通知
        _deathSubject.OnNext(true);
    }

    /// <summary>
    /// スティック入力で走る
    /// </summary>
    private void Run()
    {
        float x = Input.GetAxis("Horizontal");
        float addX = _speed * x;

        Vector3 currentPosition = transform.position;

        // キー入力に合わせてキャラクターを移動する
        Vector3 newPosition = transform.position + (new Vector3(addX, 0, 0) * Time.deltaTime);
        if (!_isDamage)
        {
            if (newPosition.x - 3.5f >= _leftTop.x && newPosition.x + 3.0f <= _rightBottom.x)
            {
                _rigitBody.MovePosition(newPosition);
            }
        }
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

    /// <summary>
    /// 点滅コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashCoroutine()
    {
        _invincibleStartTime = Time.time;
        while (_invincibleTime / 1000 > Time.time - _invincibleStartTime)
        {
            yield return new WaitForEndOfFrame();

            // ノックバック状態を解除
            if (Time.time - _invincibleStartTime > _KnockBackTime / 1000)
            {
                _isDamage = false;
            }

            _flashTime += Time.deltaTime;
            if (_flashTime > 0.025f)
            {
                _mesh1.SetActive(true);
                _mesh2.SetActive(true);
            }
            else
            {
                _mesh1.SetActive(false);
                _mesh2.SetActive(false);
            }
            if (_flashTime > 0.05f)
            {
                _flashTime = 0f;
            }
        }
        _mesh1.SetActive(true);
        _mesh2.SetActive(true);
    }
}
