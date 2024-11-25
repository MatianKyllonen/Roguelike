using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SploogBulletEnemy : MonoBehaviour
{

    public int damage = 10;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Movement>().TakeDamage(damage);
            collision.gameObject.GetComponent<Movement>().ApplySploog();
            Destroy(gameObject);
        }
    }
}

