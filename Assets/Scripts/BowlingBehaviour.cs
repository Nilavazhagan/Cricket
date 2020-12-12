using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowlingBehaviour : MonoBehaviour, ICricketBehaviour
{

    public GameObject markerBounds, marker;
    public float movementSpeed = 5f;
    public float minScale = 1f, maxScale = 3f;
    public float scaleSpeed = 5f;

    public Slider speedSlider;
    public float sliderSpeed, minSpeed, maxSpeed;

    public Transform BallSpawn;

    public string helpHeader;
    [TextArea]
    public string helpContentMovement, helpContentAccuracy, helpContentSpeed;

    string[] helpContents;
    public InputsReceived OnInputsReceived { get; set; }

    BoxCollider boundaryCollider;
    ListenMode listenMode = ListenMode.LINE_LENGTH;

    void Awake()
    {
        helpContents = new string[] { helpContentMovement, helpContentAccuracy, helpContentSpeed };
    }
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
            Vector3 newPos = marker.transform.position + (move * movementSpeed * Time.deltaTime);
            if (boundaryCollider.bounds.Contains(newPos))
            {
                marker.transform.position = newPos;
            }
        }else if (listenMode == ListenMode.ACCURACY)
        {
            float deltaScale = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale);
            marker.transform.localScale = new Vector3(minScale + deltaScale, 1, minScale + deltaScale);
        }else if (listenMode == ListenMode.SPEED)
        {
            speedSlider.value = Mathf.PingPong(Time.time * sliderSpeed, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && listenMode != ListenMode.NONE)
        {
            listenMode++;
            UpdateHelpContent();
        }

        if (listenMode == ListenMode.NONE)
        {
            Vector3 pitchPoint = marker.transform.position;
            float accuracy = marker.transform.localScale.x - minScale;                 //Lower the value, Higher the accuracy
            ballSpeed = speedSlider.value * (maxSpeed - minSpeed) + minSpeed;

            Vector2 randomPointInCircle = Random.insideUnitCircle * accuracy;
            pitchPoint.x += randomPointInCircle.x;
            pitchPoint.z += randomPointInCircle.y;
            ballThrowDirection = (pitchPoint - transform.position).normalized;

            listenMode = ListenMode.OFF;
            OnInputsReceived?.Invoke();
        }
    }


    Vector3 ballThrowDirection;
    float ballSpeed;
    public void Play()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(ballThrowDirection * ballSpeed, ForceMode.Impulse);
    }

    public void Reset()
    {
        listenMode = ListenMode.LINE_LENGTH;
        marker.transform.localScale = new Vector3((minScale + maxScale) / 2, 1, (minScale + maxScale) / 2);
        marker.transform.localPosition = Vector3.zero;
        speedSlider.value = 0;

        GetComponent<Rigidbody>().Sleep();
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = BallSpawn.transform.position;

    }

    public void ListenToInput()
    {
        listenMode = ListenMode.LINE_LENGTH;
        UpdateHelpContent();
    }

    public void Silence() {
        listenMode = ListenMode.OFF;
    }

    private void UpdateHelpContent() { 
        if((int)listenMode < helpContents.Length)
        {
            HelpManager.Instance.UpdateHelpContent(helpHeader, helpContents[(int)listenMode]);
        }
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
