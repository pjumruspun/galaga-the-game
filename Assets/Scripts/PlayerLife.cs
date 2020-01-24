using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    // Essential components
    public GameObject player;
    public GameObject currentPlayerObj;

    // Lives Implementation
    public int lives = 3;
    public Text livesText;

    // Respawn Location
    private Vector3 respawnLocation = new Vector3(0.0f, -4.0f, 0.0f);

    // Invulnerable Variables
    private bool isInvulnerable;
    private const float invulnerableTime = 3.0f;
    private float currentInvulTime = 0.0f;
    private SpriteRenderer r;
    private bool isRendererActive;
    private int invulFrameCount;
    private const int numberOfFramesToToggle = 9;

    // Start is called before the first frame update
    void Start()
    {
        isInvulnerable = false;
        currentPlayerObj = GameObject.FindGameObjectWithTag("Player");
        r = currentPlayerObj.GetComponentInChildren<SpriteRenderer>();
        r.enabled = true;
        isRendererActive = true;
        invulFrameCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLivesUI();
        if (isInvulnerable)
        {
            isRendererActive = !isRendererActive;
            if(invulFrameCount % numberOfFramesToToggle == 0)
                r.enabled = isRendererActive;
            Physics.IgnoreLayerCollision(8, 9, true); // Temporarily ignore collision between player and enemy
            Physics.IgnoreLayerCollision(10, 9, true); // Player and rocket
            currentInvulTime += Time.deltaTime;
            if (currentInvulTime > invulnerableTime)
            {
                r.enabled = true;
                isRendererActive = true;
                invulFrameCount = 0;
                Physics.IgnoreLayerCollision(8, 9, false);
                Physics.IgnoreLayerCollision(10, 9, false);
                isInvulnerable = false;
                currentInvulTime = 0.0f;
            }
            ++invulFrameCount;
        }
    }

    public void Respawn()
    {
        SetInvulnerable();
        --lives;
        UpdateLivesUI();
        if (lives > 0)
        {
            currentPlayerObj = Instantiate(player, respawnLocation, Quaternion.identity);
            r = currentPlayerObj.GetComponentInChildren<SpriteRenderer>();
            //s.sortingOrder = 2;
        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!!");
    }

    public void SetInvulnerable()
    {
        isInvulnerable = true;
    }

    public bool GetInvulnerableStatus()
    {
        return isInvulnerable;
    }

    private void UpdateLivesUI() // Update the UI
    {
        string livesStr = "Lives: " + lives.ToString();
        livesText.text = livesStr;
    }
}
