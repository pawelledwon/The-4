using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumerableExtension
{
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
}

public class TowerShoot : MonoBehaviour
{
    [SerializeField]
    private float shootForce = 10f;

    private GameObject currentCannonball;
    private GameObject[] towers;
    private Vector3 currentShootPosition; // Store the shoot position
    private Vector3 currentShootDirection; // Store the shoot direction

    private void Start()
    {
        towers = GameObject.FindGameObjectsWithTag("Tower");
        RandomTower();
    }

    private void RandomTower()
    {
        // Pick a random tower
        GameObject chosenTower = towers.PickRandom();

        foreach (GameObject tow in towers)
        {
            if (tow != chosenTower)
            {
                // Freeze all other cannonballs
                var towCannonBall = FindChildWithTag(tow, "CannonBall");
                if (towCannonBall != null)
                {
                    towCannonBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }
            }
            else
            {
                // Unfreeze the chosen cannonball
                var towCannonBall = FindChildWithTag(tow, "CannonBall");
                if (towCannonBall != null)
                {
                    towCannonBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    currentCannonball = towCannonBall;

                    // Calculate the shoot position and direction based on the tower's orientation
                    currentShootPosition = towCannonBall.transform.position;
                    currentShootDirection = tow.transform.right; // Assume the cannon shoots along its local X-axis
                }
            }
        }

        if (currentCannonball != null)
        {
            ShootCannonball();
        }
    }

    private void ShootCannonball()
    {
        if (currentCannonball != null)
        {
            // Move cannonball to the calculated shoot position
            currentCannonball.transform.position = currentShootPosition;

            // Add force to the cannonball in the calculated direction
            Rigidbody rb = currentCannonball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // Reset any previous velocity
            rb.AddForce(currentShootDirection * shootForce, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CannonBall"))
        {
            // Reset cannonball position to the calculated shoot position and shoot again
            if (currentCannonball != null)
            {
                currentCannonball.transform.position = currentShootPosition;
                currentCannonball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                RandomTower();
            }
        }
    }

    private GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return null;
    }
}
