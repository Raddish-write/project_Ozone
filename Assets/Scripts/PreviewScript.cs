using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreviewScript : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    private (int ballId, int colorID)[] previewArray;
    private GameObject[] previewBalls = new GameObject[3];

    //begins preview
    public void loadPreview(Queue<(int ballId, int colorID)> ballQueue)
    {
        previewArray = ballQueue.ToArray();
        renderPreview();
    }

    //updates after ball is fired
    public void shiftPreview()
    {
        if (previewArray.Length == 0) return;
        // Shift array left by 1 and shrinks it 
        var newArray = new (int ballId, int colorID)[previewArray.Length - 1];
        for (int i = 0; i < previewArray.Length - 1; i++)
        {
            newArray[i] = previewArray[i + 1];
        }
        previewArray = newArray;
        renderPreview();
    }

    //establishes boundary area for preview balls 
    void renderPreview()
    {
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        float centerX = (bounds.max.x + bounds.min.x) / 2f;
        float startY = bounds.min.y + 0.8f;

        for (int i = 0; i < 3; i++)
        {
            if (previewBalls[i] != null) Destroy(previewBalls[i]);
            previewBalls[i] = null;

            if (i >= previewArray.Length) continue;

            int colorID = previewArray[i].colorID;
            float yPos = startY + (i * 1.0f) + 0.02f;
            previewBalls[i] = Instantiate(ballPrefabs[colorID], new Vector3(centerX, yPos, 0f), Quaternion.identity);
            Rigidbody2D rb = previewBalls[i].GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void Start() { }
    void Update() { }
}
