using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    // Essential Components
    protected CharacterController controller;
    protected GameObject player;
    protected GameObject originalPos;
    protected GameObject spawnLocationLeft;
    protected GameObject spawnLocationRight;
    protected ManagerScript managerScript;

    // Constants
    protected const float repositionSpeed = 5.0f;
    protected const float horizontalSpeed = 1.2f;
    protected const float leftBound = -3.7f;
    protected const float rightBound = 3.7f;
    protected const float upperBound = 5.2f;
    protected const float lowerBound = -6f;
    protected const float repositionDistance = 0.05f;

    // Checking Variables
    protected const int ensureRelocation = 60;
    protected int relocationCounter = 0;

    // Direction
    protected Vector3 move;

    protected abstract void HandleState();
    protected abstract void HandleBounds();
    protected abstract void FlyToPlayer();
    protected abstract void Reposition();
    public abstract void ChasePlayer();


    protected void LookAtPlayer()
    {
        transform.up = player.transform.position - transform.position;
    }
    

    protected void ResetRotation()
    {
        transform.up = Vector3.down;
    }

    protected bool IsRepositioned()
    {
        float dist = Vector3.Distance(originalPos.transform.position, gameObject.transform.position);
        return dist < repositionDistance;
    }
}
