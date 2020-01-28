using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupMovement : MonoBehaviour
{
    // Essential Components
    private ManagerScript managerScript;

    // Constants
    private const float moveSpeed = 1.5f;
    private const float leftBound = -1.0f;
    private const float rightBound = 1.0f;

    // Controlling Variables
    private Vector3 move;
    private const int upDownFrameCount = 30; // The enemy group will move up and down for 30 frames
    private int currentFrameCount;

    // State Variable
    private enum State
    {
        Left, Right, Up, Down
    }
    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManagerScript>();
        float curX = gameObject.transform.position.x;
        currentState = State.Right; // Move right first
        currentFrameCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float curX = gameObject.transform.position.x;
        move = Vector3.zero;
        if(currentState == State.Right)
        {
            if(curX < rightBound) // Still can move right
            {
                move += Vector3.right * moveSpeed;
            }
            else
            {
                currentState = State.Up;
            }
        }
        else if(currentState == State.Left)
        {
            if (curX > leftBound) // Still can move right
            {
                move += Vector3.left * moveSpeed;
            }
            else
            {
                currentState = State.Down;
            }
        }
        else if(currentState == State.Up)
        {
            if(currentFrameCount < upDownFrameCount)
            {
                move += Vector3.up * moveSpeed;
                ++currentFrameCount;
            }
            else
            {
                currentState = State.Left;
                currentFrameCount = 0;
            }
        }
        else if (currentState == State.Down)
        {
            if (currentFrameCount < upDownFrameCount)
            {
                move += Vector3.down * moveSpeed;
                ++currentFrameCount;
            }
            else
            {
                currentState = State.Right;
                currentFrameCount = 0;
            }
        }

        float speedMult = managerScript.GetEnemySpeedMult();
        if (speedMult < 0) speedMult = 0;
        transform.position += move * Time.deltaTime * speedMult;
    }

    public void ResetState()
    {
        transform.position = Vector3.zero;
        currentFrameCount = 0;
        currentState = State.Right;
    }
}
