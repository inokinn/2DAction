using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    PlayerDamage,
    Weapon,
    EnemyDamage,
}


/// <summary>
/// 効果音再生マン
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ダメージ音
    [SerializeField] private AudioClip _playerDamageSound;
    // ショット音
    [SerializeField] private AudioClip _weaponSound;
    // 敵のダメージ音
    [SerializeField] private AudioClip _enemyDamageSound;

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
            case SoundType.PlayerDamage:
                _audioSource.PlayOneShot(_playerDamageSound);
                break;
            case SoundType.Weapon:
                _audioSource.PlayOneShot(_weaponSound);
                break;
            case SoundType.EnemyDamage:
                _audioSource.PlayOneShot(_enemyDamageSound);
                break;
            default:
                break;
        }
    }
}
