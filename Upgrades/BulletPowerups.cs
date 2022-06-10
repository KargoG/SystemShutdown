using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct keeping track of everything important for spawning a bullet.
/// </summary>
[System.Serializable]
public struct BulletInformation
{
    [SerializeField] public GameObject BulletPre;
    [SerializeField] public string SoundPath;
    [SerializeField] public LayerMask Hittables;
    [SerializeField] public float CooldownTime;
}

/// <summary>
/// The different bullet settings that can be enabled with upgrades
/// </summary>
public enum BulletSettings
{
    NormalBullet,
    Bomb,
    End
}


/// <summary>
/// This component is meant to be added to the player.
/// It is responsible for spawning bullets and handles
/// what bullets are spawned based on active powerups.
/// </summary>
public class BulletPowerups : MonoBehaviour
{
    [SerializeField] private float _upgradeTime = 5f;
    [SerializeField] private BulletSettings _bullet = BulletSettings.NormalBullet;

    [SerializeField] private BulletInformation _normalBullet;
    [SerializeField] private BulletInformation _bomb;

    private AudioSource _audioSource = null;
    private float _leftOverUpgradeTime = 0;

    private void Awake()
    {
        _audioSource = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_bullet != BulletSettings.NormalBullet)
        {
            _leftOverUpgradeTime -= Time.deltaTime;
            if (_leftOverUpgradeTime <= 0)
                _bullet = BulletSettings.NormalBullet;
        }
    }


    /// <summary>
    /// This method handles choosing which bullets to shoot
    /// </summary>
    /// <returns>The cooldown time of the bullets</returns>
    public float SpawnBullet(Vector3 spawnPosition, Vector3 shootingDirection)
    {
        switch (_bullet)
        {
            case BulletSettings.NormalBullet:
                return SpawnNormalBullet(spawnPosition, shootingDirection);
            case BulletSettings.Bomb:
                return SpawnBomb(spawnPosition, shootingDirection);
            default:
                return SpawnNormalBullet(spawnPosition, shootingDirection);
        }
    }

    /// <summary>
    /// This method handles spawning the default bullet.
    /// </summary>
    /// <param name="spawnPosition">The bullets spawn position</param>
    /// <param name="shootingDirection">The direction the bullet should fly towards</param>
    /// <returns>The cooldown of the bullet</returns>
    private float SpawnNormalBullet(Vector3 spawnPosition, Vector3 shootingDirection)
    {
        shootingDirection = new Vector3(shootingDirection.y, -shootingDirection.x, shootingDirection.z);
        GameObject bullet = Instantiate(_normalBullet.BulletPre, spawnPosition, Quaternion.LookRotation(Vector3.back, shootingDirection), null);

        // We set which layers the bullet should hit
        bullet.GetComponent<Bullet>().HitableLayers = _normalBullet.Hittables;

        _audioSource.PlayOneShot(Resources.Load<AudioClip>(_normalBullet.SoundPath));

        return _normalBullet.CooldownTime;
    }

    /// <summary>
    /// This method handles spawning the bomb.
    /// </summary>
    /// <param name="spawnPosition">The bullets spawn position</param>
    /// <param name="shootingDirection">The direction the bullet should fly towards</param>
    /// <returns>The cooldown of the bullet</returns>
    private float SpawnBomb(Vector3 spawnPosition, Vector3 shootingDirection)
    {
        shootingDirection = new Vector3(shootingDirection.y, -shootingDirection.x, shootingDirection.z);
        GameObject bullet = Instantiate(_bomb.BulletPre, spawnPosition, Quaternion.LookRotation(Vector3.back, shootingDirection), null);

        // We set which layers the bomb should hit
        bullet.GetComponent<Bullet>().HitableLayers = _bomb.Hittables;

        _audioSource.PlayOneShot(Resources.Load<AudioClip>(_bomb.SoundPath));

        return _bomb.CooldownTime;
    }

    public void ActivateUpgrade(BulletSettings upgrade)
    {
        _leftOverUpgradeTime = _upgradeTime;
        _bullet = upgrade;
    }
}
