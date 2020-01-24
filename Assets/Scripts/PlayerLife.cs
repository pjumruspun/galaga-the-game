using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    // Essential components
    public GameObject player;
    public GameObject currentPlayerObj;
    public ManagerScript managerScript;

    // Lives Implementation
    [SerializeField]
    private int lives = 2;
    private int currentLives;
    
    // Respawn Location
    private Vector3 respawnLocation = new Vector3(0.0f, -4.0f, 0.0f);

    // UI implementation
    private Text livesText;
    private Text gameOverText;

    // Invulnerable Variables
    private bool isInvulnerable;
    private const float invulnerableTime = 3.0f;
    private float currentInvulTime = 0.0f;
    private SpriteRenderer r;
    private bool isRendererActive;
    private int invulFrameCount;
    private const int numberOfFramesToToggle = 9;

    // Handle Game Over status
    private bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        currentLives = lives;
        managerScript = GameObject.Find("Game Manager").GetComponent<ManagerScript>();
        isInvulnerable = false;
        currentPlayerObj = GameObject.FindGameObjectWithTag("Player");
        r = currentPlayerObj.GetComponentInChildren<SpriteRenderer>();
        r.enabled = true;
        isRendererActive = true;
        invulFrameCount = 0;
        isGameOver = false;

        livesText = GameObject.FindGameObjectWithTag("LivesText").GetComponent<Text>();
        gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<Text>();

        gameOverText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLivesUI();
        if (isInvulnerable)
        {
            isRendererActive = !isRendererActive;
            if(invulFrameCount % numberOfFramesToToggle == 0 && r != null)
            {
                r.enabled = isRendererActive; 
            }
                
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

        if(Input.GetKeyDown(KeyCode.Space) && isGameOver)
        {
            Debug.Log("Restarting game");
            CreatePlayerObj();
            currentLives = lives;
            managerScript.RestartGame();

            r.enabled = true;
            isRendererActive = true;
            invulFrameCount = 0;
            Physics.IgnoreLayerCollision(8, 9, false);
            Physics.IgnoreLayerCollision(10, 9, false);
            isInvulnerable = false;
            currentInvulTime = 0.0f;

            isGameOver = false;
            gameOverText.enabled = false;
        }
    }

    public void Respawn()
    {
        SetInvulnerable();
        --currentLives;
        UpdateLivesUI();
        if (currentLives > 0)
        {
            CreatePlayerObj();
            //s.sortingOrder = 2;
        }
        else
        {
            isGameOver = true;
            GameOver();
            Time.timeScale = 0;
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!!");
        gameOverText.enabled = true;
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
        string livesStr = "Lives: " + currentLives.ToString();
        livesText.text = livesStr;
    }

    private void CreatePlayerObj()
    {
        currentPlayerObj = Instantiate(player, respawnLocation, Quaternion.identity);
        r = currentPlayerObj.GetComponentInChildren<SpriteRenderer>();
    }
}
