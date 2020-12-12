using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerMovement : MonoBehaviour
{

    public GameObject markerBounds;
    public float movementSpeed = 5f;

    BoxCollider boundaryCollider;
    // Start is called before the first frame update
    void Start()
    {
        boundaryCollider = markerBounds.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 newPos = transform.position + (move * movementSpeed * Time.deltaTime);
        if (boundaryCollider.bounds.Contains(newPos))
        {
            transform.position = newPos;
        }
    }
}
