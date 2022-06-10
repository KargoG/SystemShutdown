using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The specific attack for the shooter enemy.
/// The charger, attacks the player by running
/// towards them. Once the enemy starts charging
/// at the player, it can not redirect, giving
/// the player the opportunity to dodge.
/// </summary>
public class ChargerAttack : EnemyAttack
{
    [SerializeField] private float _attackDistance = 1f;
    [SerializeField] private float _attackSpeed = 3f;
    [SerializeField] private LayerMask _obstacleMask = 0;
    [SerializeField] private LayerMask _playerMask = 0;

    [SerializeField] private float _previewDuration = 2f;
    [SerializeField] private float _previewShakeMagnitude = 2f;
    [SerializeField] private float _previewScale = 1.25f;
    [SerializeField] private GameObject _visuals = null;

    /// <summary>
    /// This method checks whether the enemy can attack the player.
    /// The charger can attack the player if its not on
    /// cooldown, in range of the player and the path to
    /// the player is not obstructed.
    /// </summary>
    /// <param name="toAttack">position to attack (the player pos)</param>
    /// <returns>Can the enemy attack the player?</returns>
    public override bool CanAttack(Vector3 toAttack)
    {
        // We can't start a second attack if the enemy is already attacking
        if (IsAttacking)
            return false;

        // We check for the cooldown
        if (Time.time - _timeOfLastAttack < _cooldownTimeAfterAttack)
            return false;

        bool inRange = (toAttack - transform.position).sqrMagnitude <= _attackDistance * _attackDistance;
        if (!inRange)
            return inRange;

        // We raycast in the direction the enemy wants to charge
        // If there are any obstacles, like wall in between the enemy and the player, the enemy can not attack
        if (Physics2D.Raycast(transform.position, toAttack - transform.position, (toAttack - transform.position).magnitude, _obstacleMask))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// This method makes the enemy attack the player by charging
    /// at them.
    /// The charge consists of 2 phases.
    /// First the shake, that indicates the attack
    /// And then the charge itself towards the player
    /// </summary>
    /// <param name="toAttack">position to attack (the player pos)</param>
    public override void Attack(Vector3 toAttack)
    {
        _attackDirection = toAttack - transform.position;
        _attackDirection.Normalize();

        IsAttacking = true;

        // Since the attack takes some time and has multiple steps it happens in a coroutine
        StartCoroutine(PerformAttack());
    }

    /// <summary>
    /// This IEnumerator performs the charge frame 
    /// by frame.
    /// The charge consists of 2 phases.
    /// First the shake, that indicates the attack
    /// And then the charge itself towards the player
    /// </summary>
    private IEnumerator PerformAttack()
    {
        // We indicate the attack with the shake
        yield return Shake();

        float distanceTraveled = 0;

        // over the course of multiple frames the enemy moves towards the attacked point
        while (distanceTraveled < _attackDistance)
        {
            Vector2 toTravel = _attackDirection * Time.deltaTime * _attackSpeed;
            distanceTraveled += toTravel.magnitude;
            _rb.MovePosition(_rb.position + toTravel);

            yield return new WaitForFixedUpdate();
        }

        _timeOfLastAttack = Time.time;
        IsAttacking = false;
    }

    /// <summary>
    /// This IEnumerator performs the shake, that
    /// indicates the enemies attack.
    /// Aside from violent shaking the enemy also
    /// gets bigger while shaking, to draw the players
    /// attention to it.
    /// </summary>
    private IEnumerator Shake()
    {
        // We save the original values so we can reset the enemy after the shake
        Vector3 originalPos = _visuals.transform.localPosition;
        Vector3 originalScale = _visuals.transform.localScale;


        float elapsed = 0.0f;
        // We shake and scale the enemy over time
        while (elapsed < _previewDuration)
        {
            float x = Random.Range(-1f, 1f) * _previewShakeMagnitude;
            float y = Random.Range(-1f, 1f) * _previewShakeMagnitude;

            _visuals.transform.localPosition = new Vector3(x, y, originalPos.z);
            _visuals.transform.localScale = originalScale * Mathf.Lerp(1, _previewScale, elapsed / _previewDuration);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // we reset the enemy for the attack
        _visuals.transform.localPosition = originalPos;
        _visuals.transform.localScale = originalScale;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            col.transform.GetComponent<Health>().Damage(1);
    }
}
