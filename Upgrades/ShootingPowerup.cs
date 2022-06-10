using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The different shooting settings that can be enabled with upgrades
/// </summary>
public enum ShootingSettings
{
    SingleShot,
    DoubleShot,
    TrippleShot,
    End
}

/// <summary>
/// This component is meant to be added to the player.
/// It is responsible for shooting bullets and handles
/// how they are shot based on active powerups.
/// </summary>
public class ShootingPowerup : MonoBehaviour
{
    [SerializeField] private float _upgradeTime = 5f;
    [SerializeField] private ShootingSettings _shootingSettings = ShootingSettings.SingleShot;

    private BulletPowerups _bulletHandler = null;

    private float _leftOverUpgradeTime = 0;

    private void Awake()
    {
        _bulletHandler = FindObjectOfType<BulletPowerups>();
    }

    private void Update()
    {
        // if the shooting settings are changed we check the remaining upgrade duration
        if (_shootingSettings != ShootingSettings.SingleShot)
        {
            _leftOverUpgradeTime -= Time.deltaTime;
            if (_leftOverUpgradeTime <= 0)
                _shootingSettings = ShootingSettings.SingleShot;
        }
    }

    /// <summary>
    /// This method handles choosing how to shoot bullets
    /// </summary>
    /// <returns>The cooldown time of the bullets</returns>
    public float ShootBullets()
    {
        switch (_shootingSettings)
        {
            case ShootingSettings.SingleShot:
                return SingleShot();
            case ShootingSettings.DoubleShot:
                return DoubleShot();
            case ShootingSettings.TrippleShot:
                return TrippleShot();
            default:
                return SingleShot();
        }
    }

    /// <summary>
    /// This method handles the normal single shot.
    /// </summary>
    /// <returns>The cooldown time of the shot.</returns>
    private float SingleShot()
    {
        return _bulletHandler.SpawnBullet(transform.position, transform.right);
    }

    /// <summary>
    /// This method handles the double shot.
    /// </summary>
    /// <returns>The cooldown time of the shot.</returns>
    private float DoubleShot()
    {
        Vector3 shotDirection = transform.right;

        // We tell the bulletHandler to spawn the first bullet with the direction rotated to the left
        float cooldown = _bulletHandler.SpawnBullet(transform.position, Quaternion.AngleAxis(15, Vector3.back) * shotDirection);
        // We tell the bulletHandler to spawn the second bullet with the direction rotated to the right
        cooldown += _bulletHandler.SpawnBullet(transform.position, Quaternion.AngleAxis(-15, Vector3.back) * shotDirection);

        // we adjust and return the cooldown
        return cooldown * 0.75f;
    }

    /// <summary>
    /// This method handles the tripple shot.
    /// </summary>
    /// <returns>The cooldown time of the shot.</returns>
    private float TrippleShot()
    {
        Vector3 shotDirection = transform.right;

        // We tell the bulletHandler to spawn the first bullet with the direction rotated to the left
        float cooldown = _bulletHandler.SpawnBullet(transform.position, Quaternion.AngleAxis(22.5f, Vector3.back) * shotDirection);
        // We tell the bulletHandler to spawn the second bullet
        cooldown += _bulletHandler.SpawnBullet(transform.position, shotDirection);
        // We tell the bulletHandler to spawn the third bullet with the direction rotated to the right
        cooldown += _bulletHandler.SpawnBullet(transform.position, Quaternion.AngleAxis(-22.5f, Vector3.back) * shotDirection);

        // we adjust and return the cooldown
        return cooldown * 0.75f;
    }

    public void ActivateUpgrade(ShootingSettings upgrade)
    {
        _leftOverUpgradeTime = _upgradeTime;
        _shootingSettings = upgrade;
    }
}
