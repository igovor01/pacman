using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pacman;

    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public AudioSource siren;
    public AudioSource munch1;
    public AudioSource munch2;
    public AudioSource powerPelletAudio;
    public AudioSource respawningAudio;
    public AudioSource ghostEatenAudio;

    public int currentMunch = 0;

    public int score;
    public Text scoreText;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    public EnemyController redGhostController;
    public EnemyController pinkGhostController;
    public EnemyController blueGhostController;
    public EnemyController orangeGhostController;

    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollectedOnThisLife;

    public bool hadDeathOnThisLevel = false;

    public bool gameIsRunning;

    public List<NodeController> nodeControllers = new List<NodeController>();

    public bool newGame;
    public bool clearedLevel;

    public AudioSource startGameAudio;
    public AudioSource death;

    public int lives;
    public int currentLevel;

    public Image blackBackground;

    public Text gameOverText;
    public Text livesText;

    public enum GhostMode
    {
        chase, scatter
    }
    public GhostMode currentGhostMode;

    public int[] ghostModeTimers = new int[] { 7, 20, 7, 20, 5, 20, 5 };
    public int ghostModeTimerIndex;
    public float ghostModeTimer = 0;
    public bool runningTimer; // we don't wanna be running the timer when the ghosts are in frigtened mode, we want it to pause. once we grab a power pellet runningTimer will be set to false, then once the power pellet expires it will go back to true
    public bool completedTimer; //this will stop the timer from running once we've reached the end of the ghostModeTimers list.

    public bool isPowerPelletRunning = false;
    public float currentPowerPelletTime = 0;
    public float powerPelletTimer = 8f;
    public int powerPelletMultiplyer = 1;

    // Start is called before the first frame update
    void Awake()
    {
        newGame = true;
        clearedLevel = false;
        blackBackground.enabled = false;

        redGhostController = redGhost.GetComponent<EnemyController>();
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();

      
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        pacman = GameObject.Find("Player");

        StartCoroutine(Setup());
    }


    //3 scenarios:
    //1- pacman dies - in different function, a ghost is gonna eat pacman, pacman is gonna play death animation, it's gonna immediately call Setup() after a pause and then it's not gonna pause again
    //2- we cleared the level-everything stops, pauses and flashes a border on the screen, everything back to where it was before, waits 4seconds before respawning starts
    //3- we start a new game- similar to number2
    public IEnumerator Setup()
    {
        ghostModeTimerIndex = 0;
        ghostModeTimer = 0;
        completedTimer = false;
        runningTimer = true;
        gameOverText.enabled = false;
        //If pacman clear a level, a background will appear covering the level, and the game will pause for 0.1 seconds
        if (clearedLevel)
        {
            blackBackground.enabled = true;
            //Activate background
            yield return new WaitForSeconds(0.1f);

            
        }

        blackBackground.enabled = false;

        pelletsCollectedOnThisLife = 0;
        currentGhostMode = GhostMode.scatter;
        gameIsRunning = false;
        currentMunch = 0;

        float waitTimer = 1f;
   
        if (clearedLevel || newGame)
        {
            pelletsLeft = totalPellets;
            waitTimer = 4f;
            //Pellets will respawn when pacman clears the level or starts a new game
            for (int i = 0; i < nodeControllers.Count; i++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }
       

        if (newGame)
        {
            startGameAudio.Play();
            score = 0;
            scoreText.text = "Score: " + score.ToString();
            SetLives(3);
            currentLevel = 1;
        }

        pacman.GetComponent<PlayerController>().Setup();

        redGhostController.Setup();
        blueGhostController.Setup();
        orangeGhostController.Setup();
        pinkGhostController.Setup();   
       

        newGame = false;
        clearedLevel = false;
        
        yield return new WaitForSeconds(waitTimer);

        StartGame();
    }

    void SetLives(int newLives)
    {
        lives = newLives;
        livesText.text = "Lives: " + lives;
    }

    void StartGame()
    {
        gameIsRunning = true;
        siren.Play();
    }

    void StopGame()
    {
        gameIsRunning = false;
        siren.Stop();
        powerPelletAudio.Stop();
        respawningAudio.Stop();
        pacman.GetComponent<PlayerController>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameIsRunning)
        {
            return;
        }

        if(redGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning 
            ||
            pinkGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning 
            ||
            blueGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning 
            ||
            orangeGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
            )
        {
            if (!respawningAudio.isPlaying)
            {
                respawningAudio.Play();
            }
        }
        else
        {
            if (respawningAudio.isPlaying)
            {
                respawningAudio.Stop();
            }
        }
        //if the game is running and we have not completed our cycle of scatter mode we are going to loop through ghostModeTimers array
        if (!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if(ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex++;
                if(currentGhostMode == GhostMode.chase)
                {
                    currentGhostMode = GhostMode.scatter;
                }
                else
                {
                    currentGhostMode = GhostMode.chase;
                }

                if(ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGhostMode = GhostMode.chase;
                }
            }
        }

        if (isPowerPelletRunning)
        {
            currentPowerPelletTime += Time.deltaTime;
            if(currentPowerPelletTime >= powerPelletTimer)
            {
                isPowerPelletRunning = false;
                currentPowerPelletTime = 0;
                powerPelletAudio.Stop();
                siren.Play();
                powerPelletMultiplyer = 1;
            }
        }
    }

    public void GotPelletFromNodeController(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets++;
        pelletsLeft++;
    }

    public void AddToScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }

    public IEnumerator CollectedPellet(NodeController nodeController)
    {
        if (currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        else if (currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        pelletsLeft--;
        pelletsCollectedOnThisLife++;

        int requiredBluePellets = 0;
        int requiredOrangePellets = 0;

        if (hadDeathOnThisLevel)
        {
            requiredBluePellets = 12;
            requiredOrangePellets = 32;
        }
        else
        {
            requiredBluePellets = 30;
            requiredOrangePellets = 60;
        }

        if (pelletsCollectedOnThisLife >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }

        if (pelletsCollectedOnThisLife >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }
        //Add to our score
        AddToScore(10);

        //Check if there are any pellets left
        if (pelletsLeft == 0)
        {
            currentLevel++;

            clearedLevel = true;
            
            StopGame();

            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());
        }

        //Is this a power pellet
        if (nodeController.isPowerPellet)
        {
            siren.Stop();
            powerPelletAudio.Play();
            isPowerPelletRunning = true;
            currentPowerPelletTime = 0;

            redGhostController.SetFrightened(true);
            pinkGhostController.SetFrightened(true);
            blueGhostController.SetFrightened(true);
            orangeGhostController.SetFrightened(true);
        }
    }

    public IEnumerator PauseGame(float timeToPause)
    {
        gameIsRunning = false;
        yield return new WaitForSeconds(timeToPause);
        gameIsRunning = true;
    }

    public void GhostEaten()
    {
        ghostEatenAudio.Play();
        AddToScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));
    }

    public IEnumerator PlayerEaten()
    {
        hadDeathOnThisLevel = true;
        StopGame();
        yield return new WaitForSeconds(1);

        //Make all ghosts invisible
        redGhostController.SetVisible(false);
        pinkGhostController.SetVisible(false);
        blueGhostController.SetVisible(false);
        orangeGhostController.SetVisible(false);

        //Play death animation
        pacman.GetComponent<PlayerController>().Death();
        death.Play();
        yield return new WaitForSeconds(3);
        SetLives(lives - 1);

        if(lives <= 0)
        {
            SceneManager.LoadSceneAsync("EndGameScene");

            //Display gamover text
            gameOverText.enabled = true;

            yield return new WaitForSeconds(3);

            newGame = true;
        }

        StartCoroutine(Setup());
    }
}
