using UnityEngine;
using UniRx;

/// <summary>
/// プレイヤーを表すView
/// </summary>
public class PlayerView : MonoBehaviour
{
    // 移動速度
    [SerializeField] private float _speed = 0.1f;
    // ジャンプ時に上方向に掛かる力の強さ
    [SerializeField] private float _jumpPower = 400f;
    // 接地判定オブジェクト
    [SerializeField] private GameObject _foot;

    // ジャンプ中フラグ
    private bool _isGround = false;
    // アニメーター
    private Animator _animator;
    // リジッドボディ
    private Rigidbody _rigitBody;
    // 接地判定用オブジェクトのスクリプト
    private FootView _footView;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigitBody = transform.GetComponent<Rigidbody>();
        _footView = _foot.GetComponent<FootView>();

        // 接地判定のイベント
        _footView.OnGroundChanged.Subscribe(isGround =>
        {
            _isGround = isGround;
            if (_isGround)
            {
                _animator.SetBool("jumping", false);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        // 入力による操作
        this.Jump();
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
    /// キー入力でジャンプする
    /// </summary>
    private void Jump()
    {
        if (_isGround && Input.GetButtonDown("Jump"))
        {
            _animator.SetBool("jumping", true);
            _rigitBody.AddForce(new Vector3(0, _jumpPower, 0));
        }
    }

    private void OnCollisionStay(Collision other) {
        var rightRay = new Ray(this.transform.position, Vector3.right);
        var leftRay = new Ray(this.transform.position, Vector3.right);
        Physics.Raycast(rightRay, 0.3f, LayerMask.GetMask("Stage"));
        Physics.Raycast(leftRay, 0.3f, LayerMask.GetMask("Stage"));
        
    }
}
