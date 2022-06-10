using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the base class for the enemy behaviours.
/// Different enemy types have to subclass from this component
/// to define enemy specific behaviour.
/// </summary>
[RequireComponent(typeof(EnemyAttack), typeof(EnemyMovement), typeof(Rigidbody2D))]
public abstract class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float _timeBetweenUpdates = 0.5f;

    protected EnemyAttack _enemyAttack = null;
    protected EnemyMovement _enemyMovement = null;

    protected Transform _player = null;

    protected bool _isUpdating = false;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _enemyAttack = FindObjectOfType<EnemyAttack>();
        _enemyMovement = FindObjectOfType<EnemyMovement>();
    }

    protected virtual void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        StartActing();
    }


    public virtual void StartActing()
    {
        _isUpdating = true;
        StartCoroutine(UpdateBehaviour());
    }

    /// <summary>
    /// This IEnumerator handles the update loop of the Enemy,
    /// ensuring the enemy waits the time given in _timeBetweenUpdates
    /// between behaviour updates.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator UpdateBehaviour()
    {
        while (true)
        {
            NormalBehaviour();

            yield return new WaitForSeconds(_timeBetweenUpdates);
        }
    }

    /// <summary>
    /// This method handles the behaviour and decides what the enemy does
    /// at any given point. The default behaviour is fairly simple.
    /// If the player is in range of its attack, it attack. If not
    /// it moves towards the player.
    /// Override this method if you want to define specific behaviour
    /// for an enemy type.
    /// </summary>
    protected virtual void NormalBehaviour()
    {
        // Handle attacking
        bool canAttack = _enemyAttack.CanAttack(_player.position);

        if (canAttack)
            _enemyAttack.Attack(_player.position);

        // Handle movement
        _enemyMovement.UpdateMovement(!_enemyAttack.IsAttacking || _enemyAttack.CanMoveWhileAttacking, _player.position);
    }
}
