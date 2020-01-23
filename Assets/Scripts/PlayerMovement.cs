using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Essential Components
    public CharacterController controller;
    public GameObject projectile;

    // Constants
    private const float playerSpeed = 5.0f;
    private const float leftBound = -3.0f;
    private const float rightBound = 3.0f;

    // Controlling Variables
    Vector3 move;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float curX = gameObject.transform.position.x;
        move = Vector3.zero;
        if (Input.GetAxisRaw("Horizontal") > 0 && curX < rightBound) // Wants to move right and still not out of right bound
        {
            move += Vector3.right * playerSpeed;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && curX > leftBound) // Wants to move left and still not out of left bound
        {
            move += Vector3.left * playerSpeed;
        }
        controller.Move(move * Time.deltaTime);
    }

    private void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
        }
    }
}
