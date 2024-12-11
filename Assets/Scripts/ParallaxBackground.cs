using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private static float speedMultiplier = 1.0f;

    private float inverseSpeed;
    private SpriteRenderer spriteRenderer;
    private Vector2 spriteSize;
    private Vector3 origin;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables to use in Update
        spriteRenderer = GetComponent<SpriteRenderer>();
        origin = transform.position;
        spriteSize = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit * speed;

        // Resize sprite and rescale transform to adjust proximity according to speed
        transform.localScale *= speed;
        spriteRenderer.size *= 1 / speed;
    }

    // Update is called once per frame
    void Update()
    {
        // Translate by speed
        transform.Translate(speedMultiplier * speed * Time.deltaTime * Vector2.left);

        // Reposition if needed
        var dif = transform.position - origin;
        if (Mathf.Abs(dif.x) > spriteSize.x)
            transform.Translate(-new Vector2(((int)(dif.x/spriteSize.x))*spriteSize.x, 0));
        if (Mathf.Abs(dif.y) > spriteSize.y)
            transform.Translate(new Vector2(0, -((int)(dif.y / spriteSize.y)) * spriteSize.y));
    }
}
