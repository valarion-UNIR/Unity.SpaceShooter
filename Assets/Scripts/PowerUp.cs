using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private AbstractAmmoSpawner spawner;

    public AbstractAmmoSpawner Spawner { get => spawner; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.left, Space.World);
        transform.Rotate(new Vector3(0, 0, 90) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            Destroy(gameObject);
        }
    }
}
