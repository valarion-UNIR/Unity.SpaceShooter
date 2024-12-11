
using UnityEngine;

public class EnemyLinear : Enemy
{

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.left);
    }
}