using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerMovement : MonoBehaviour
{

    public GameObject markerBounds;
    public float movementSpeed = 5f;
    public float minScale = 1f, maxScale = 3f;
    public float scaleSpeed = 5f;

    public Slider speedSlider;
    public float sliderSpeed, minSpeed, maxSpeed;

    public GameObject Ball;
    public Transform BallSpawn;

    BoxCollider boundaryCollider;
    ListenMode listenMode = ListenMode.LINE_LENGTH;
    // Start is called before the first frame update
    void Start()
    {
        boundaryCollider = markerBounds.GetComponent<BoxCollider>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (listenMode == ListenMode.LINE_LENGTH) { 
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 newPos = transform.position + (move * movementSpeed * Time.deltaTime);
            if (boundaryCollider.bounds.Contains(newPos))
            {
                transform.position = newPos;
            }
        }else if (listenMode == ListenMode.ACCURACY)
        {
            float deltaScale = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale);
            transform.localScale = new Vector3(minScale + deltaScale, 1, minScale + deltaScale);
        }else if (listenMode == ListenMode.SPEED)
        {
            speedSlider.value = Mathf.PingPong(Time.time * sliderSpeed, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && listenMode != ListenMode.NONE)
        {
            listenMode++;
        }
    }

    private void FixedUpdate()
    {
        if (listenMode == ListenMode.NONE)
        {
            Vector3 pitchPoint = transform.position;
            float accuracy = transform.localScale.x - minScale;                 //Lower the value, Higher the accuracy
            float ballSpeed = speedSlider.value * (maxSpeed - minSpeed) + minSpeed;

            Vector2 randomPointInCircle = Random.insideUnitCircle * accuracy;
            pitchPoint.x += randomPointInCircle.x;
            pitchPoint.z += randomPointInCircle.y;

            Vector3 direction = (pitchPoint - Ball.transform.position).normalized;

            Rigidbody rb = Ball.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce(direction * ballSpeed, ForceMode.Impulse);
            listenMode = ListenMode.OFF;

            Invoke("Reset", 3.0f);
        }
    }

    void Reset()
    {
        listenMode = ListenMode.LINE_LENGTH;
        transform.localScale = new Vector3((minScale + maxScale) / 2, 1, (minScale + maxScale) / 2);
        transform.localPosition = Vector3.zero;
        speedSlider.value = 0;

        Ball.GetComponent<Rigidbody>().Sleep();
        Ball.GetComponent<Rigidbody>().useGravity = false;
        Ball.transform.position = BallSpawn.transform.position;
    }

    enum ListenMode
    {
        LINE_LENGTH,
        ACCURACY,
        SPEED,
        NONE,
        OFF
    }
}
