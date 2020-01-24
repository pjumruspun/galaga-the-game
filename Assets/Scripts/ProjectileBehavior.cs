using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    // Essential Components
    private CharacterController controller;

    // Constants
    private const float projectileSpeed = 10.0f;
    private const float leftBound = -3.7f;
    private const float rightBound = 3.7f;
    private const float upperBound = 5.2f;
    private const float lowerBound = -5.2f;

    // Controlling Variables
    Vector2 move;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move = Vector2.zero;
        move += Vector2.up * projectileSpeed;
        controller.Move(move * Time.deltaTime);
        HandleBounds();
    }

    private void HandleBounds()
    {
        Vector2 currentPosition = gameObject.transform.position;
        if (currentPosition.x < leftBound || currentPosition.x > rightBound || currentPosition.y > upperBound || currentPosition.y < lowerBound)
            Destroy(gameObject);
    }
}
