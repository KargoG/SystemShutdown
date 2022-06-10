using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The specific attack for the shooter enemy.
/// The shooter, as the name suggests, shoots the
/// player from a distance.
/// </summary>
public class ShooterAttack : EnemyAttack
{
    [SerializeField] private GameObject _bulletPrefab = null;
    [SerializeField] private float _attackDistance = 2f;
    [SerializeField] private LayerMask _playerMask = 0;

    private float _shootCooldownTimer = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        _shootCooldownTimer = _cooldownTimeAfterAttack;
    }

    private void Update()
    {
        if (IsOnCooldown())
        {
            _shootCooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// This method makes the enemy attack the player by shooting
    /// a bullet at them.
    /// </summary>
    /// <param name="toAttack">The position to attack (the player pos)</param>
    public override void Attack(Vector3 toAttack)
    {
        _attackDirection = (toAttack - transform.position).normalized;

        GameObject bullet = Instantiate(_bulletPrefab, this.transform.position, Quaternion.identity, null);

        // we set the direction the bullet flies by setting its transform.right
        bullet.transform.right = _attackDirection;
        // we set the bullets HitableLayer to the player layer
        bullet.GetComponent<Bullet>().HitableLayers = _playerMask;

        _shootCooldownTimer = _cooldownTimeAfterAttack;
    }

    /// <summary>
    /// This method checks whether the enemy can attack the
    /// player.
    /// The enemy can attack if the attack is not on cooldown
    /// and it is close enough to the player.
    /// </summary>
    /// <param name="toAttack">The position to attack (usually the player pos)</param>
    /// <returns>Can this enemy attack?</returns>
    public override bool CanAttack(Vector3 toAttack)
    {
        if (IsOnCooldown())
            return false;

        return _attackDistance * _attackDistance > (toAttack - transform.position).sqrMagnitude;
    }

    private bool IsOnCooldown()
    {
        return _shootCooldownTimer > 0.0f;
    }
}
