using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is meant for a collectable upgrade thats spawned in the level.
/// It randomly decides what type of upgrade when spawning.
/// </summary>
public class Upgrade : MonoBehaviour
{
    private ShootingSettings _shootingUpgrade = ShootingSettings.SingleShot;
    private BulletSettings _bulletUpgrade = BulletSettings.NormalBullet;

    [SerializeField] private SpriteRenderer _renderer = null;

    // Start is called before the first frame update
    void Awake()
    {

        // first -1 to exclude the end and then -2 to exclude the default upgrades
        // We randomly decide what upgrade to get
        // We start from 1 to exclude BulletSettings.NormalBullet and do -2 to exclude BulletSettings.End and ShootingSettings.SingleShot
        // We dont need to exclude ShootingSettings.End since its automatically excluded
        int upgrade = Random.Range(1, (int)BulletSettings.End + (int)ShootingSettings.End - 2);

        switch (upgrade)
        {
            case 0: // 1 is the bomb
                _renderer.sprite = Resources.Load<Sprite>("Sprites/Bomb");

                _bulletUpgrade = BulletSettings.Bomb;

                break;
            case 1: // 1 is the double shot
                _renderer.sprite = Resources.Load<Sprite>("Sprites/DoubleShot");

                _shootingUpgrade = ShootingSettings.DoubleShot;
                break;

            case 2: // 2 is the triple shot
                _renderer.sprite = Resources.Load<Sprite>("Sprites/TripleShot");

                _shootingUpgrade = ShootingSettings.TrippleShot;
                break;

        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            // if we the upgrade is a shooting upgrade
            if(_shootingUpgrade > 0)
            {
                collision.GetComponent<ShootingPowerup>().ActivateUpgrade(_shootingUpgrade);
            }
            else // otherwise it is a bullet upgrade
            {
                collision.GetComponent<BulletPowerups>().ActivateUpgrade(_bulletUpgrade);
            }

            Destroy(gameObject);
        }
    }
}
