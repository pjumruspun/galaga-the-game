using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    // Essential components
    private PlayerLife playerLife;
    private GameObject gameManager;
    private ManagerScript m;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager");
        playerLife = gameManager.GetComponent<PlayerLife>();
        m = gameManager.GetComponent<ManagerScript>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Debug.Log("13 - " + gameObject.tag + " just hit " + hit.gameObject.tag);
        if (hit.gameObject.tag == "SpeedPU" && gameObject.tag == "Player")
        {
            // Debug.Log("SPEED2");
            Destroy(hit.gameObject);
            m.ShootingSpeedLevelUp();
        }

        if (hit.gameObject.tag == "Player" && (gameObject.tag == "Enemy" || gameObject.tag == "RedEnemy" || gameObject.tag == "Rocket" || gameObject.tag == "GreenEnemy" || gameObject.tag == "Laser" || gameObject.tag == "SpeedPU"))
        {
            // DebugMissingObj();
            //Debug.Log("6");
            if (!playerLife.GetInvulnerableStatus())
            {
                //Debug.Log("5");
                if (gameObject.tag == "Enemy")
                {
                    //Debug.Log("4");
                    playerLife.Respawn();
                    Destroy(transform.parent.gameObject);
                    Destroy(hit.gameObject);
                    m.EnemyCountDecrement();
                    m.BlueEnemyDestroyed(false);
                }
                else if(gameObject.tag == "RedEnemy")
                {
                    //Debug.Log("3");
                    playerLife.Respawn();
                    Destroy(transform.parent.gameObject);
                    Destroy(hit.gameObject);
                    m.EnemyCountDecrement();
                    m.RedEnemyDestroyed(false);
                }
                else if (gameObject.tag == "GreenEnemy")
                {
                    //Debug.Log("2");
                    playerLife.Respawn();
                    Destroy(transform.parent.gameObject);
                    Destroy(hit.gameObject);
                    m.EnemyCountDecrement();
                    m.GreenEnemyDestroyed(false);
                }
                else if(gameObject.tag == "Rocket")
                {
                    //Debug.Log("1");
                    playerLife.Respawn();
                    Destroy(gameObject);
                    Destroy(hit.gameObject);
                }
                else if (gameObject.tag == "Laser")
                {
                    //Debug.Log("LASER");
                    playerLife.Respawn();
                    Destroy(hit.gameObject);
                }
                else if (gameObject.tag == "SpeedPU")
                {
                    // Debug.Log("SPEED");
                    Destroy(gameObject);
                    m.ShootingSpeedLevelUp();
                }
            }
        }
        else
        {
            if (gameObject.tag != hit.gameObject.tag)
            {
                /*
                if (gameObject.tag == "Enemy")
                {
                    Debug.Log("1");
                    Destroy(transform.parent.gameObject);
                    m.EnemyCountDecrement();
                }
                */
                if (hit.gameObject.tag == "Enemy" && gameObject.tag == "Projectile")
                {
                    SpawnShootingSpeedPowerUp(hit.transform.position, 2.5f);
                    Destroy(hit.transform.parent.gameObject);
                    Destroy(gameObject);
                    m.EnemyCountDecrement();
                    m.BlueEnemyDestroyed(true);
                }
                else if (hit.gameObject.tag == "RedEnemy" && gameObject.tag == "Projectile")
                {
                    SpawnShootingSpeedPowerUp(hit.transform.position, 5.0f);
                    Destroy(hit.transform.parent.gameObject);
                    Destroy(gameObject);
                    m.EnemyCountDecrement();
                    m.RedEnemyDestroyed(true);
                }
                else if (hit.gameObject.tag == "GreenEnemy" && gameObject.tag == "Projectile")
                {
                    GreenEnemyBehavior g = hit.transform.gameObject.GetComponent<GreenEnemyBehavior>();
                    SpawnShootingSpeedPowerUp(hit.transform.position, 10.0f);
                    Destroy(hit.transform.gameObject);
                    Destroy(g.GetOriginalPosGameObject());
                    Destroy(gameObject);
                    m.EnemyCountDecrement();
                    m.GreenEnemyDestroyed(true);
                    
                }

            }
        }
    }

    private void DebugMissingObj()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.Find("Game Manager");
        }

        if (playerLife == null)
        {
            playerLife = gameManager.GetComponent<PlayerLife>();
        }
    }

    private void SpawnShootingSpeedPowerUp(Vector3 pos, float chance)
    {
        float randFloat = GetRandFloat();
        if (randFloat < chance)
            Instantiate(m.GetSpeedPowerUp(), pos, Quaternion.identity);
    }

    private static int GetRandFloat()
    {
        return Random.Range(0, 100);
    }
}
