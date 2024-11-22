using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagMovement : MonoBehaviour
{
    public float speed = 2f; // Speed of the movement
    public float height = 2f; // Maximum height of the jump

    private Vector3 startPosition;

    void Start()
    {
        // Save the starting position of the flag
        startPosition = transform.position;
    }

    void Update()
    {
        // Move the flag up and down, ensuring it doesn't go below the starting position
        float newY = startPosition.y + Mathf.Abs(Mathf.Sin(Time.time * speed) * height);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}