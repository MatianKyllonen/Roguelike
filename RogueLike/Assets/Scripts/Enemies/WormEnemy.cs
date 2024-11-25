using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float burrowDelay = 4f;
    public int attackDamage = 10;
    public float attackRange = 2f;

    private Transform targetPlayer;
    private bool isPaused = false;


    public GameObject burrowed;
    public GameObject unBurrowed;

    void Update()
    {
        if (unBurrowed == null)
        {
            Destroy(gameObject);
        }
        if (FindObjectOfType<UpgradeManager>().shopOpen)
        {
            return;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0) return;

        float closestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < closestDistance && !player.gameObject.GetComponent<Movement>().knocked)
            {
                closestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }

        if (nearestPlayer != null && !isPaused)
        {
            targetPlayer = nearestPlayer.transform;

            if (Vector3.Distance(transform.position, targetPlayer.position) <= attackRange - 1)
            {
                StartCoroutine(BurrowAndAttack(targetPlayer.gameObject));
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (targetPlayer == null)
            return;

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
    }

    IEnumerator BurrowAndAttack(GameObject player)
    {
        isPaused = true;
        yield return new WaitForSeconds(0.8f);

        unBurrowed.SetActive(true);
        burrowed.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            player.GetComponent<Movement>().TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(burrowDelay);


        unBurrowed.SetActive(false);
        burrowed.SetActive(true);

        isPaused = false;
    }
}
