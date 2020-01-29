using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Essential Components
    private CharacterController controller;
    [SerializeField]
    private GameObject projectile;
    // private AudioSource pewSound;
    private ManagerScript managerScript;

    // Constants
    private const float playerSpeed = 5.0f;
    private const float leftBound = -3.0f;
    private const float rightBound = 3.0f;

    // Controlling Variables
    private Vector3 move;

    // Shooting Handling Variables
    private const float shootingCooldown = 0.3f;
    private float currentShootingCooldown;

    // Start is called before the first frame update
    void Start()
    {
        // pewSound = GetComponent<AudioSource>();
        managerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManagerScript>();
        currentShootingCooldown = 0.0f;
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
        if (Input.GetKey(KeyCode.Space) && currentShootingCooldown > shootingCooldown / managerScript.GetShootingSpeedMult())
        {
            Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
            //if (pewSound != null) pewSound.PlayOneShot(pewSound.clip);
            currentShootingCooldown = 0.0f;
        }
        if(currentShootingCooldown < shootingCooldown)
            currentShootingCooldown += Time.deltaTime;
    }

    private void OnDestroy()
    {
        //pewSound = null;
    }
}
