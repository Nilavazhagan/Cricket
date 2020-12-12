using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerMovement : MonoBehaviour
{

    public GameObject markerBounds;
    public float movementSpeed = 5f;
    public float minScale = 1f, maxScale = 3f;
    public float scaleSpeed = 5f;

    BoxCollider boundaryCollider;
    bool listenForMovement = true, listenForAccuracy = false;
    // Start is called before the first frame update
    void Start()
    {
        boundaryCollider = markerBounds.GetComponent<BoxCollider>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (listenForMovement) { 
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 newPos = transform.position + (move * movementSpeed * Time.deltaTime);
            if (boundaryCollider.bounds.Contains(newPos))
            {
                transform.position = newPos;
            }
        }else if (listenForAccuracy)
        {
            float deltaScale = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale);
            transform.localScale = new Vector3(minScale + deltaScale, 1, minScale + deltaScale);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (listenForMovement)
            {
                listenForMovement = false;
                listenForAccuracy = true;
            }else if (listenForAccuracy)
            {
                listenForAccuracy = false;
            }
        }
    }

    void Reset()
    {
        listenForMovement = true;
        listenForAccuracy = false;
        transform.localScale = new Vector3((minScale + maxScale) / 2, 1, (minScale + maxScale) / 2);
        transform.localPosition = Vector3.zero;
    }
}
