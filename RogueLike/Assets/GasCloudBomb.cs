using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloudBomb : MonoBehaviour
{
    public GameObject gasCloudPrefab;  
    public float explosionDelay = 2f; 
    public float gasDuration = 5f;   
    public float damagePerSecond = 10f; 

    private bool hasExploded = false;

    void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        GameObject gasCloud = Instantiate(gasCloudPrefab, transform.position, Quaternion.identity);

        Destroy(gasCloud, gasDuration);

        Destroy(gameObject);
    }
}
