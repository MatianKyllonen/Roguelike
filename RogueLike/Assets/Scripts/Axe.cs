using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private int damage = 10;

    private void Update()
    {
        if (target != null)
        {
            transform.RotateAround(target.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Movement player = other.GetComponent<Movement>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
