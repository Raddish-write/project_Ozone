using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayScript : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    public Transform firePoint;
    public float ballSpeed = 10f;
    public Reticle reticle;

    private Queue<(int ballId, int colorID)> ballQueue = new Queue<(int ballId, int colorID)>();
    private bool hasFired = false;
    public bool hasHit = false;
    public static bool isChecking = false;

    public static Dictionary<int, GameObject> ballGraph;
    public Levels levelData;
    public LevelProg ballData;
    public int firedBalID;
    public bool hasWiggled;

    GameObject lastFired;
    PreviewScript preview;
    CielingScript cieling;
    ScoreBoardScript scoreBoard;
 

    private float stepTimer;
    private bool stepPending;
    private float stepInterval = 10f;
    public static int stepCount = 0;

    void loadLevel(int levelNumber)
    {
        // brings in data from level storage files
        Dictionary<int, int> level = levelData.getLevel(levelNumber);
        ballQueue = ballData.getLevelBag(levelNumber);

        if (level == null)
        {
            Debug.LogError($"Failed to load level {levelNumber}");
            return;
        }

        ballGraph = new Dictionary<int, GameObject>();

        // Instantiate all balls and store them in ballGraph
        foreach (var kvp in level)
        {
            int ballKey = kvp.Key;
            int colorIndex = kvp.Value;

            Vector3 startPos = getStartPos(ballKey);
            GameObject ball = Instantiate(ballPrefabs[colorIndex], startPos, Quaternion.identity);
            ball.GetComponent<BallScript>().ballID = ballKey;
            ball.GetComponent<BallScript>().colorID = colorIndex;
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            ballGraph[ballKey] = ball;
        }

        BallScript sourceBall = ballGraph[-1].GetComponent<BallScript>(); //manually adds balls to sourceBall because SourceBall has no collission
        for (int i = 0; i <= 11; i++)
        {
            if (ballGraph.ContainsKey(i))
            {
                sourceBall.adjList.Add(i);
                ballGraph[i].GetComponent<BallScript>().adjList.Add(-1);
            }
        }

        preview.loadPreview(ballQueue);
        cieling.setReset(); //moves cieling back for new level 
        scoreBoard.setReset(); //resets score stuff for new level
        stepCount = 0;
    }

    Vector3 getStartPos(int ballKey) //helper function for loadLevel
    {
        int row = ballKey / 100;
        int pos = ballKey % 100;

        // Constrain to valid column range (alternating 12 and 11 wide)
        int maxCol = (row % 2 == 0) ? 11 : 10;

        float xOffset = pos * 1.0f;
        float yOffset = row * -.8f;

        if (row % 2 == 1)
        {
            xOffset += 0.5f;
        }

        Bounds bounds = GetComponent<SpriteRenderer>().bounds; //used to modify positions based on transform of play area
        float centerX = bounds.min.x + 0.7f; // fixed offet to avoid overlap 
        float centerY = bounds.max.y - 0.7f;


        return new Vector3(centerX + xOffset, centerY + yOffset, 0f);
    }

    void fire()
    {
        checkBagEmpty();
        hasFired = true;
        reticle.stopRotating();

        var (ballId, colorIndex) = ballQueue.Dequeue();
        Vector2 direction = reticle.getFireDir();

        // enters ball into play area based on reticle directin
        GameObject ball = Instantiate(ballPrefabs[colorIndex], firePoint.position, Quaternion.identity);
        lastFired = ball;
        ball.GetComponent<BallScript>().ballID = ballId;
        ball.GetComponent<BallScript>().colorID = colorIndex;
        ball.GetComponent<Rigidbody2D>().linearVelocity = direction * ballSpeed;
        preview.shiftPreview();
    }

    void reload()
    {
        hasFired = false;
        reticle.startRotating();
    }//fire and reload encapsulated for future seperation for game logic

    IEnumerator checkHit(GameObject ball)
    {
        int ballID = ball.GetComponent<BallScript>().ballID;
        int colorID = ball.GetComponent<BallScript>().colorID;
        isChecking = true;

        depthFirstSearch(ballID, colorID);

        yield return StartCoroutine(destroyMarkedCoroutine());
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(checkRoofCoroutine());
        yield return StartCoroutine(checkForWinCoroutine());

        hasHit = false;
        hasFired = false;
    }

    IEnumerator destroyMarkedCoroutine()
    {
        destroyMarked(false);
        yield return null;
    }

    IEnumerator checkRoofCoroutine()
    {
        checkRoof();
        yield return null;
    }

    IEnumerator checkForWinCoroutine()
    {
        checkForWin();
        yield return null;
    }

    void destroyMarked(bool isCieling)
    {
        List<int> markedBalls = new List<int>();
        foreach (KeyValuePair<int, GameObject> ball in ballGraph)
        {
            if (ball.Key != -1) //keeps from deleting the source
            {
                bool mark = ball.Value.GetComponent<BallScript>().isDestroyed;
                if (mark)
                {
                    markedBalls.Add(ball.Key);
                }
            }
        }

        if (markedBalls.Count == 1 && !isCieling) { return; }

        foreach (int ballID in markedBalls)
            {
                if (ballGraph.ContainsKey(ballID))
                {
                    ballGraph[ballID].GetComponent<BallScript>().deleteBall();
                }
            }

        isChecking = false;
    }

    //searches for balls not connected to cieling and removes them
    void checkRoof()
    {
        BallScript source = ballGraph[-1].GetComponent<BallScript>();
        
        isChecking = true;
        foreach (KeyValuePair<int, GameObject> ball in ballGraph)
        {
            ball.Value.GetComponent<BallScript>().isDestroyed = true;
        }

        depthFirstSearch(-1, -1);
        // prior to deleting balls and thus scoring points in the cieling check phase
        // game checks number of balls that will be deleted and counts them towards 
        // the combo multiplier
        int marked = ballGraph.Values.Count(b => b.GetComponent<BallScript>().isDestroyed);
        if (marked > 0)
        {
            scoreBoard.combo(marked);
        }
        destroyMarked(true);
    }

    void checkForWin()
    {
        if (ballGraph[-1].GetComponent<BallScript>().adjList.Count == 0)
        {
            EndGameScript.instance.winGame();
            //UnityEditor.EditorApplication.isPlaying = false; //placeholder, update with UI update
        }
    }

    void depthFirstSearch(int ballID, int colorID)
    {
        depthFirstSearch(ballID, colorID, new HashSet<int>());
    }
    void depthFirstSearch(int ballID, int colorID, HashSet<int> visited)
    {
        if (!ballGraph.ContainsKey(ballID) || visited.Contains(ballID))
        {
            return;
        }

        visited.Add(ballID);

        BallScript ball = ballGraph[ballID].GetComponent<BallScript>();

        // for matching, continues until no match
        // for top search, go until all connected componants marked
        if (ball.colorID != colorID && colorID != -1)
        {
            return;
        }

        foreach(int adjBallID in ball.adjList)
        {
            depthFirstSearch(adjBallID, colorID, visited);
        }
        ball.isDestroyed = !ball.isDestroyed;

    }

    public void StepDown()
    {
        foreach (KeyValuePair<int, GameObject> ball in ballGraph)
        {
            ball.Value.GetComponent<BallScript>().StepDown();
        }

        FindAnyObjectByType<CielingScript>().StepDown();
        stepCount++;
    }

    void stepClock()
    {
        stepTimer += Time.deltaTime;
        if (stepTimer >= stepInterval - 2f && hasWiggled == false)
        {
            foreach(KeyValuePair<int, GameObject> ball in ballGraph){
                ball.Value.GetComponent<BallScript>().wiggleBall();
            }
            hasWiggled = true;
        }

        if (stepTimer >= stepInterval)
        {
            stepTimer = 0f;
            if (hasFired || isChecking)
                stepPending = true;
            else
                StepDown();
        }

        if (stepPending && !hasFired && !isChecking)
        {
            stepPending = false;
            hasWiggled = false;
            StepDown();
        }
    }

    void checkBagEmpty()
    {
        if (ballQueue.Count == 0)
        {
            EndGameScript.instance.loseGame();
            //UnityEditor.EditorApplication.isPlaying = false; //placeholder, update with UI update 
        }
    }

    void Start()
    {
        preview = FindAnyObjectByType<PreviewScript>();
        cieling = FindAnyObjectByType<CielingScript>();
        scoreBoard = FindAnyObjectByType<ScoreBoardScript>();
        loadLevel(1);
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !hasFired) //checks for spaceBar and if any other animations are goind
        {
            fire();
        }

        if (hasHit) 
        {
            hasHit = false;
            StartCoroutine(checkHit(lastFired));
            reload();
        }

        stepClock();
    }
}
