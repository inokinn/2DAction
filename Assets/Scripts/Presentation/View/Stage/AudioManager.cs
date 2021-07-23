using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Pause,
    PlayerDamage,
    Weapon,
    EnemyDamage,
    Death,
    GameOver,
}


/// <summary>
/// 効果音再生マン
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ポーズ音
    [SerializeField] private AudioClip _pauseSound;
    // ダメージ音
    [SerializeField] private AudioClip _playerDamageSound;
    // ショット音
    [SerializeField] private AudioClip _weaponSound;
    // 敵のダメージ音
    [SerializeField] private AudioClip _enemyDamageSound;
    // 死亡時音
    [SerializeField] private AudioClip _deathSound;
    // ゲームオーバー音
    [SerializeField] private AudioClip _gameOverSound;

    // オーディオソース
    private AudioSource _audioSource;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 効果音を再生
    /// </summary>
    /// <param name="type"></param>
    public void PlaySound(SoundType type)
    {
        switch (type)
        {
            case SoundType.Pause:
                _audioSource.PlayOneShot(_pauseSound);
                break;
            case SoundType.PlayerDamage:
                _audioSource.PlayOneShot(_playerDamageSound);
                break;
            case SoundType.Weapon:
                _audioSource.PlayOneShot(_weaponSound);
                break;
            case SoundType.EnemyDamage:
                _audioSource.PlayOneShot(_enemyDamageSound);
                break;
            case SoundType.Death:
                _audioSource.PlayOneShot(_deathSound);
                break;
            case SoundType.GameOver:
                _audioSource.PlayOneShot(_gameOverSound);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void AudioStop()
    {
        _audioSource.Stop();
    }
}
