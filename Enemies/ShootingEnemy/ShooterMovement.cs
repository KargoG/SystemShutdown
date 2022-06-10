using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The specific movement for the shooter enemy.
/// Unlike the normal enemy the shooter doesn't just
/// stand around when close to the player. Instead the
/// shooter starts circling around the player while
/// keeping his distance.
/// </summary>
public class ShooterMovement : EnemyMovement
{
    [SerializeField] private float _startCirclingDistance = 3f;
    [SerializeField] private float _startEvadingDistance = 1f;
    [SerializeField] private float _directionChangeSpeed = 0.5f;


    protected override void FixedUpdate()
    {
        // If we have no path we can't move along it
        if (_path == null)
            return;

        // If the enemy is far enough away it just approaches the player like the normal enemy
        if (Vector3.SqrMagnitude(transform.position - _path.vectorPath[_path.vectorPath.Count - 1]) > _startCirclingDistance * _startCirclingDistance)
            base.FixedUpdate();
        else
            Circle();
    }

    /// <summary>
    /// This method makes the enemy circle around the player at a given range.
    /// If the player starts approaching, it moves away from them.
    /// This method is called each frame
    /// </summary>
    private void Circle()
    {
        // The player should always be our last waypoint
        Vector3 playerPos = _path.vectorPath[_path.vectorPath.Count - 1];

        Vector3 shooterToPlayer = playerPos - transform.position;

        // If the distance between the shooter enemy and the player is too short the enemy moves away
        if (shooterToPlayer.sqrMagnitude < _startEvadingDistance * _startEvadingDistance)
        {
            BuildDistance();
            return;
        }
        
        // First we set a basic direction to move around the player
        Vector3 circleDirection = Quaternion.Euler(0, 0, 90) * shooterToPlayer.normalized;
        
        // We randomize this direction with a perlin noise
        // Said noise is mapped to be between -1 and 1 so the enemy can circle in both directions
        circleDirection = circleDirection * (Mathf.PerlinNoise(Time.time * _directionChangeSpeed, Time.time * _directionChangeSpeed) * 2 - 1);

        _rb.MovePosition(transform.position + circleDirection * Time.deltaTime * _movementSpeed);
    }

    /// <summary>
    /// This method makes the enemy build distance from the player
    /// by simply moving away from them in a straight line.
    /// </summary>
    private void BuildDistance()
    {
        Vector3 movementDirection = transform.position - _path.vectorPath[_path.vectorPath.Count - 1];
        
        _rb.MovePosition(transform.position + movementDirection.normalized * Time.deltaTime * _movementSpeed);
    }
}
