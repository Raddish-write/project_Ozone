using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{

    [SerializeField] float moveSpeed;

    float singleTextureWidth;
    private float diff;

    
    void Start()
    {
        SetupTexture();
    }

    void SetupTexture()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        singleTextureWidth = sprite.texture.width / sprite.pixelsPerUnit;
    }
    
    void Scroll()                                               //transform math 
    {
        float delta = moveSpeed * Time.deltaTime;
        transform.position += new Vector3(delta, 0f, 0f);
    }

    void CheckReset()                                           //if single tile is past base tile, reset center of sprite
    {
        if (singleTextureWidth <= transform.position.x)
        {
            diff = transform.position.x - singleTextureWidth;   //fixed stutter issue with parallax 
            transform.position = new Vector3(diff, transform.position.y, transform.position.z);
        }
    }
    void Update()
    {
        Scroll();
        CheckReset();
    }
}
