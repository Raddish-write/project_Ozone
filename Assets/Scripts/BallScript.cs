using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public List<int> adjList;  //points to connected balls 
    PlayScript playScript;
    ScoreBoardScript scoreBoard;

    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip destroySound;
    
    
    public bool isDestroyed;   //informs playScript to delete or not 
    public int ballID;
    public int colorID;
    private bool hasSnapped = false; //used to fix issue where balls swap places on collision

    private bool isWiggling = false;
    private float wiggleTimer = 0f;
    private Vector3 originalPosition;
    private float wiggleAmount = 0.15f;
    private float wiggleDuration = 0.8f; // Total duration of the wiggle
    private float wiggleSpeed = 20f; // Speed of the wiggle

    void Awake()
    {
        adjList = new List<int>();
        playScript = FindAnyObjectByType<PlayScript>();
        scoreBoard = FindAnyObjectByType<ScoreBoardScript>();
    }

    void OnCollisionEnter2D(Collision2D collision) //handles when new balls collide
    {
     
        AVHandlerScript.instance.playSoundFXClip(impactSound);

        BallScript otherBall = collision.gameObject.GetComponent<BallScript>();
        if (otherBall != null && !hasSnapped)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (otherBall.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.constraints = RigidbodyConstraints2D.FreezePosition;

                PlayScript.ballGraph[ballID] = gameObject;
                
                if (!otherBall.adjList.Contains(ballID))
                {
                    otherBall.adjList.Add(ballID);
                    adjList.Add(otherBall.ballID);
                }

                StartCoroutine(setHitNextFrame()); //see function
                

                Vector3 preSnap = transform.position;
                transform.position = snapToGrid(preSnap);
                hasSnapped = true;
                
            }
        }   
    }

    // snaps fired balls to grid based math used to generate map in playscript
    Vector3 snapToGrid(Vector3 worldPos)
    {
        // Grid dimensions: rows alternate between 12 and 11 balls wide
        const float verticalSpacing = 0.8f;

        // Reference position from PlayScript's getStartPos
        Bounds bounds = playScript.GetComponent<SpriteRenderer>().bounds;
        float centerX = bounds.min.x + 0.7f;
        float centerY = (bounds.max.y - 0.7f) - (PlayScript.stepCount * 0.8f);

        // Calculate relative position from grid origin
        float relX = worldPos.x - centerX;
        float relY = centerY - worldPos.y;

        // Determine row (every verticalSpacing units)
        int row = Mathf.RoundToInt(relY / verticalSpacing);

        // Determine column based on row type (alternating 12 and 11 wide)
        int maxCol = (row % 2 == 0) ? 11 : 10;
        int col = Mathf.Clamp(Mathf.RoundToInt(relX), 0, maxCol);

        // Account for odd rows offset
        float xOffset = col;
        if (row % 2 == 1)
        {
            xOffset += 0.5f;
        }

        // Calculate snapped position
        float snappedX = centerX + xOffset;
        float snappedY = centerY - (row * verticalSpacing);

        Vector3 snappedPos = new Vector3(snappedX, snappedY, worldPos.z);

        // Check if position is already occupied
        Collider2D[] colliders = Physics2D.OverlapCircleAll(snappedPos, 0.1f);
        foreach (Collider2D colid in colliders)
        {
            if (colid.gameObject != gameObject)
            {
                // Position occupied, return closest empty adjacent grid position instead
                return findNearestEmpty(snappedPos, row, colid);
            }
        }

        return snappedPos;

    }

    // helper function made to resolve position conflicts
    Vector3 findNearestEmpty(Vector3 preferredPos, int row, Collider2D occupant)
    {
        //finds most conflicting direction, "nudges" it, then sends it back through snapToGrid

        Vector3 occupantPos = occupant.transform.position;
        float diffX = preferredPos.x - occupantPos.x;
        float diffY = preferredPos.y - occupantPos.y;

        Vector3 adjustedPos = preferredPos;

        if (Mathf.Abs(diffX) > Mathf.Abs(diffY))
        {
            adjustedPos.x = preferredPos.x + Mathf.Sign(diffX)*1.0f;
        }
        else
        {
            adjustedPos.y = preferredPos.y + Mathf.Sign(diffY)*0.8f;
        }

            return snapToGrid(adjustedPos);
    } 

    // changed from on entry to Stay to handle both persistant and fired balls
    void OnTriggerStay2D(Collider2D collision) 
    {

        if (PlayScript.isChecking) //added to circumvent race condition
        {
            return;
        }

        BallScript otherBall = collision.GetComponent<BallScript>();
        if (otherBall != null && !adjList.Contains(otherBall.ballID))
        {
            adjList.Add(otherBall.ballID);
        }
    }

    // handles removing balls from adjlist
    private void OnTriggerExit2D(Collider2D collision) //handles when balls are deleted
    { 
        
        BallScript otherBall = collision.GetComponent<BallScript>();
        if (otherBall != null)
        {
            adjList.Remove(otherBall.ballID);
        }
    }

    public void StepDown()
    {
        transform.position += Vector3.down * 0.8f;
        if (transform.position.y < -2.5)
        {
            EndGameScript.instance.loseGame();
            //UnityEditor.EditorApplication.isPlaying = false; //placeholder, update with UI update
        } 
    }

    // used to fix bug were all delete checks were outpacing adjLists forming
    IEnumerator setHitNextFrame()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        playScript.hasHit = true;
    }

    public void wiggleBall()
    {
        if (!isWiggling)
        {
            isWiggling = true;
            wiggleTimer = 0f;
            originalPosition = transform.position;
        }
    }

    void Update()
    {
        if (isWiggling)
        {
            wiggleTimer += Time.deltaTime;

            if (wiggleTimer < wiggleDuration)
            {
                // Wiggle side-to-side
                float offset = Mathf.Sin(wiggleTimer * wiggleSpeed) * wiggleAmount;
                transform.position = new Vector3(originalPosition.x + offset, originalPosition.y, originalPosition.z);
            }
            else
            {
                // End wiggle and reset position
                transform.position = originalPosition;
                isWiggling = false;
            }
        }
    }

    public void deleteBall()
    {
        List<int> adjListCopy = new List<int>(adjList); //dodges enumeration error
        foreach(int nearID in adjListCopy)
        {
            BallScript toRemove = PlayScript.ballGraph[nearID].GetComponent<BallScript>();
            toRemove.adjList.Remove(ballID);
        }

        AVHandlerScript.instance.playSoundFXClip(destroySound, 0.02f);
        scoreBoard.scoreCounter();
        PlayScript.ballGraph.Remove(ballID);
        Destroy(gameObject);
    }

}
