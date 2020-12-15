using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    Rigidbody rigidBody;
    SphereCollider accuracyCollider;
    ListenMode listenMode = ListenMode.LINE_LENGTH;

    void Awake()
    {
        helpContents = new string[] { helpContentMovement, helpContentAccuracy, helpContentSpeed };
        boundaryCollider = markerBounds.GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
        accuracyCollider = marker.GetComponentInChildren<SphereCollider>();
    }


    bool movingMarker = false;
    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE
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
            ballSpeed = speedSlider.value * (maxSpeed - minSpeed) + minSpeed;

            Vector3 maxBounds = accuracyCollider.bounds.max;
            Vector3 minBounds = accuracyCollider.bounds.min;

            pitchPoint.x = Random.Range(minBounds.x, maxBounds.x);
            pitchPoint.z = Random.Range(minBounds.z, maxBounds.z);
            ballThrowDirection = (pitchPoint - transform.position).normalized;

            listenMode = ListenMode.OFF;
            OnInputsReceived?.Invoke();
        }
#elif UNITY_ANDROID
#if !UNITY_EDITOR
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if(listenMode == ListenMode.LINE_LENGTH)
            {
                if(touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit))
                    {
                        if(hit.collider == boundaryCollider)
                        {
                            movingMarker = true;
                        }
                    }
                }else if(touch.phase == TouchPhase.Moved && movingMarker)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider == boundaryCollider)
                        {
                            Vector3 newPos = hit.point;
                            newPos.y = 0;
                            if (boundaryCollider.bounds.Contains(newPos))
                            {
                                marker.transform.position = newPos;
                            }
                        }
                    }
                }else if(touch.phase == TouchPhase.Ended)
                {
                    listenMode++;
                    UpdateHelpContent();
                }
            }else if(listenMode == ListenMode.ACCURACY || listenMode == ListenMode.SPEED)
            {
                if(touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    listenMode++;
                    UpdateHelpContent();
                }
            }
        }
#else
        if (listenMode == ListenMode.LINE_LENGTH)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == boundaryCollider)
                    {
                        movingMarker = true;
                    }
                }
            }
            else if (Input.GetMouseButton(0) && movingMarker)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == boundaryCollider)
                    {
                        Vector3 newPos = hit.point;
                        newPos.y = 0;
                        if (boundaryCollider.bounds.Contains(newPos))
                        {
                            marker.transform.position = newPos;
                        }
                    }
                }

            }
            else if (Input.GetMouseButtonUp(0) && movingMarker)
            {
                listenMode++;
                UpdateHelpContent();
            }
        }
        else if (listenMode == ListenMode.ACCURACY || listenMode == ListenMode.SPEED)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                listenMode++;
                UpdateHelpContent();
            }
        }

#endif

        if (listenMode == ListenMode.ACCURACY)
        {
            float deltaScale = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale);
            marker.transform.localScale = new Vector3(minScale + deltaScale, 1, minScale + deltaScale);
        }
        else if (listenMode == ListenMode.SPEED)
        {
            speedSlider.value = Mathf.PingPong(Time.time * sliderSpeed, 1);
        }

        if (listenMode == ListenMode.NONE)
        {
            Vector3 pitchPoint = marker.transform.position;
            ballSpeed = speedSlider.value * (maxSpeed - minSpeed) + minSpeed;

            Vector3 maxBounds = accuracyCollider.bounds.max;
            Vector3 minBounds = accuracyCollider.bounds.min;

            pitchPoint.x = Random.Range(minBounds.x, maxBounds.x);
            pitchPoint.z = Random.Range(minBounds.z, maxBounds.z);
            ballThrowDirection = (pitchPoint - transform.position).normalized;

            listenMode = ListenMode.OFF;
            OnInputsReceived?.Invoke();
        }
#endif
    }


    Vector3 ballThrowDirection;
    float ballSpeed;
    public void Play()
    {
        rigidBody.useGravity = true;
        rigidBody.AddForce(ballThrowDirection * ballSpeed, ForceMode.Impulse);
    }

    public void Reset()
    {
        listenMode = ListenMode.LINE_LENGTH;
        marker.transform.localScale = new Vector3((minScale + maxScale) / 2, 1, (minScale + maxScale) / 2);
        marker.transform.localPosition = Vector3.zero;
        speedSlider.value = 0;

        rigidBody.Sleep();
        rigidBody.useGravity = false;
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
