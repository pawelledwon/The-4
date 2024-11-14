using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveParkour : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float movementLimit = 10f;

    private List<Vector3> parkourObjectStartingPositions = new List<Vector3>();
    private bool moveRight = true;
    private GameObject[] parkourObjects;
    private int parkourObjectIndex = 0;
    private bool firstMove = true;

    private void Start()
    {
        parkourObjects = GameObject.FindGameObjectsWithTag("ParkourObject");

        foreach (var parkourObject in parkourObjects)
        {
            parkourObjectStartingPositions.Add(parkourObject.transform.position);
        }
    }

    private void Update()
    {
        MoveParkourObjects();
        CheckIfSwap();
    }

    private void MoveParkourObjects()
    {
        foreach(var parkourObject in parkourObjects)
        {
            if (moveRight)
            {
                if(parkourObjectIndex % 2 == 0)
                {
                    parkourObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                }
                else
                {
                    parkourObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                }
                
            }
            else
            {
                if (parkourObjectIndex % 2 == 0)
                {
                    parkourObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                }
                else
                {
                    parkourObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                }
            }

            parkourObjectIndex++;
        }

        parkourObjectIndex = 0;
    }

    private void CheckIfSwap()
    {
        float distanceFromStart = Vector3.Distance(parkourObjects[0].transform.position, parkourObjectStartingPositions[0]);

        if (distanceFromStart > (firstMove ? movementLimit : movementLimit * 2))
        {
            firstMove = false;
            moveRight = !moveRight;

            foreach (var parkourObject in parkourObjects)
            {
                parkourObjectStartingPositions[parkourObjectIndex] = parkourObject.transform.position;
                parkourObjectIndex++;
            }

            parkourObjectIndex = 0;
        }
    }
}
