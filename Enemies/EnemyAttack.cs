using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the base class for the enemy attacking.
/// Different enemy types have to subclass from this component
/// to define enemy specific attacks.
/// When the enemy attacks is controlled by the <see cref="EnemyBehaviour"/>
/// </summary>
public abstract class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected float _cooldownTimeAfterAttack = 1f;
    [SerializeField] protected bool _canMoveWhileAttacking = false;
    public bool CanMoveWhileAttacking { get { return _canMoveWhileAttacking; } set { _canMoveWhileAttacking = value; } }

    public bool IsAttacking { get; protected set; } = false;

    protected Rigidbody2D _rb = null;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Override this method to define whether the enemy can
    /// attack the player.
    /// </summary>
    /// <param name="toAttack">The position to attack (usually the player pos)</param>
    /// <returns>Can this enemy attack?</returns>
    public abstract bool CanAttack(Vector3 toAttack);


    protected Vector3 _attackDirection = Vector3.zero;
    protected float _timeOfLastAttack = 0;

    /// <summary>
    /// Override this method to define how the enemy attacks
    /// the player.
    /// </summary>
    /// <param name="toAttack">The position to attack (usually the player pos)</param>
    public abstract void Attack(Vector3 toAttack);

}
