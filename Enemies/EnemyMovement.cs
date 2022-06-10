using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the base class for the enemy movement.
/// Different enemy types have to subclass their movement 
/// from this component to define enemy specific behaviour.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] protected float _movementSpeed = 2;
    // The kept distance from the player
    [SerializeField] private float _keptDistance = 0.5f;

    // The component seeking a path towards the player
    private Seeker _seeker = null;
    protected Rigidbody2D _rb = null;

    // The path towards the player
    public Path _path = null;

    public float _nextWaypointDistance = 3;

    private int _currentWaypoint = 0;


    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _seeker = GetComponent<Seeker>();
    }


    protected virtual void FixedUpdate()
    {
        // If we have no path we can't move along it
        if (_path == null)
            return;

        // if we are close enough to the last waypoint (the player) the enemy doesn't move
        if (Vector3.SqrMagnitude(transform.position - _path.vectorPath[_path.vectorPath.Count - 1]) < _keptDistance * _keptDistance)
            return;

        float distanceToWaypoint;
        while (true)
        {
            // We get the squared distance to the next waypoint
            distanceToWaypoint = Vector3.SqrMagnitude(transform.position - _path.vectorPath[_currentWaypoint]);
            if (distanceToWaypoint < _nextWaypointDistance * _nextWaypointDistance) // are we close enough to the next waypoint to switch waypoints?
            {
                if (_currentWaypoint + 1 < _path.vectorPath.Count) // are there more waypoints to move to?
                {
                    // We start targeting the next waypoint
                    _currentWaypoint++;
                }
                else
                {
                    // We have reached the last waypoint
                    // We should never reach this since the last waypoint is the player from which we keep a set distance with _keptDistance anyways
                    break;
                }
            }
            else
            {
                // We still have some ways to go to the current waypoint
                break;
            }
        }

        // We move towards the player
        Vector3 dir = (_path.vectorPath[_currentWaypoint] - transform.position).normalized;

        Vector3 velocity = dir * _movementSpeed;

        _rb.MovePosition(transform.position + velocity * Time.deltaTime);
        _rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// This method Searches a path towards the player and starts moving along it.
    /// This method is being called from the Enemy Behaviour or its subclasses every
    /// couple of seconds.
    /// </summary>
    /// <param name="shouldMove">should the enemy move</param>
    /// <param name="positionToMoveTo">The position the enemy should find a way towards (usually the player)</param>
    public virtual void UpdateMovement(bool shouldMove, Vector3 positionToMoveTo)
    {
        if (shouldMove)
            _seeker.StartPath(transform.position, positionToMoveTo, OnPathComplete);
        else
            _path = null;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;

            // Reset the waypoint counter so that we start to move towards the first point in the path
            _currentWaypoint = 0;
        }
    }
}
