using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    // Essential components
    PlayerLife playerLife;
    GameObject gameManager;
    ManagerScript m;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager");
        playerLife = gameManager.GetComponent<PlayerLife>();
        m = gameManager.GetComponent<ManagerScript>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Player" && gameObject.tag == "Enemy")
        {
            if(!playerLife.GetInvulnerableStatus())
            {
                playerLife.Respawn();
                Destroy(transform.parent.gameObject);
                Destroy(hit.gameObject);
                m.EnemyCountDecrement();
                m.BlueEnemyDestroyed(false);
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
                    Destroy(hit.transform.parent.gameObject);
                    Destroy(gameObject);
                    m.EnemyCountDecrement();
                    m.BlueEnemyDestroyed(true);
                }
                
            }
        }
    }
}
