using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattingBehaviour : MonoBehaviour, ICricketBehaviour
{
    public float minX = -1f, maxX = 1f;
    public float movementSpeed = 5f;
    public Rigidbody Ball;
    public float hitPower = 5f;
    public Toggle loftToggle;
    public GameObject batsmanInteractionPanel;

    [HideInInspector]
    public bool listenToInput = false;

    public string helpHeader;
    [TextArea]
    public string helpContent;

    public InputsReceived OnInputsReceived { get; set; }

    BoxCollider batsmanCollider;
    // Start is called before the first frame update
    void Start()
    {
        batsmanCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!listenToInput)
            return;
#if UNITY_STANDALONE
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && !AreArrowKeysPressed())
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            Vector3 newPos = transform.position + (move * movementSpeed * Time.deltaTime);
            if (newPos.x > maxX) newPos.x = maxX;
            if (newPos.x < minX) newPos.x = minX;
            transform.position = newPos;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (AreArrowKeysPressed() /*&& batsmanCollider.bounds.Contains(Ball.transform.position)*/)
            {
                hitDirection = new Vector3(0,0,0);

                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    hitDirection.y = 0.5f;
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    hitDirection.x = 1;
                }else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    hitDirection.x = -1;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    hitDirection.z = -1;
                }else if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (Mathf.Approximately(hitDirection.y,0) && Mathf.Approximately(hitDirection.x,0))
                    {
                        hitDirection.z = -0.1f;
                    }
                    else { 
                        if(Mathf.Approximately(hitDirection.x, 0))             //LOFTED Backward shot like uppercut
                        {
                            hitDirection.z = 1f;
                            hitDirection.x = -0.4f;
                        }else if(Mathf.Approximately(hitDirection.y,0))
                        {
                            hitDirection.z = 1f;
                        }
                        else
                        {
                            hitDirection.z = 1f;
                        }
                    } 
                }

                OnInputsReceived?.Invoke();
                listenToInput = false;
            }
        }

#elif UNITY_ANDROID
        if (batsmanMoveDir != null)
        {
            Vector3 newPos = transform.position + ((Vector3)batsmanMoveDir * movementSpeed * Time.deltaTime);
            if (newPos.x > maxX) newPos.x = maxX;
            if (newPos.x < minX) newPos.x = minX;
            transform.position = newPos;
        }
#if !UNITY_EDITOR
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                swipeStart = touch.position;
#else
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                swipeStart = Input.mousePosition;
#endif
                swiping = true;
#if !UNITY_EDITOR
            }else if(touch.phase == TouchPhase.Ended && swiping)
            {
                Vector3 swipeEnd = touch.position;
#else
            }else if(Input.GetMouseButtonUp(0) && swiping){
                Vector3 swipeEnd = Input.mousePosition;
#endif
            Vector3 deltaVector = swipeEnd - swipeStart;
                if(deltaVector.magnitude > 1)       //Some threshold
                {
                    hitDirection = new Vector3();
                    hitDirection.x = deltaVector.normalized.x;
                    hitDirection.y = isLofted ? 0.5f : 0;

                /*if(deltaVector.normalized.y < 0)
                {
                    if((hitDirection.x > -0.4f && hitDirection.x < 0.4f) && (deltaVector.normalized.y > -0.5 || !isLofted))
                    {
                        hitDirection.z = -deltaVector.normalized.y;
                    }
                    else
                    {
                        hitDirection.z = deltaVector.normalized.y;
                        hitDirection.x = -0.4f;
                    }
                }*/

                hitDirection.z = deltaVector.normalized.y;

                    OnInputsReceived?.Invoke();
                    listenToInput = false;
                    swiping = false;
                }
            }
#if !UNITY_EDITOR
    }
#endif
#endif
    }

    Vector3 swipeStart;
    bool swiping = false;
    bool isLofted = false;

    Vector3? batsmanMoveDir = null;
    public void OnLeftButtonDown()
    {
        batsmanMoveDir = Vector3.left;
    }

    public void OnRightButtonDown() 
    {
        batsmanMoveDir = Vector3.right;
    }

    public void OnButtonUp()
    {
        batsmanMoveDir = null;
    }
    
    public void ToggleLofted(bool isChecked)
    {
        isLofted = isChecked;
    }

    Vector3 hitDirection;
    bool isPlaying = false;
    public void Play()
    {
        isPlaying = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaying)
        {
            if(other.gameObject == Ball.gameObject)
            {
                Ball.Sleep();
                Ball.AddForce(hitDirection * hitPower, ForceMode.Impulse);
            }
        }
    }

    bool AreArrowKeysPressed()
    {
        return (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow));
    }

     public void Reset()
    {
        listenToInput = false;
        isPlaying = false;
        swiping = false;
        loftToggle.isOn = false;
        isLofted = false;
        Vector3 pos = transform.position;
        pos.x = 0;
        transform.position = pos;
    }

    public void ListenToInput()
    {
        batsmanInteractionPanel.SetActive(true);
        listenToInput = true;
        HelpManager.Instance.UpdateHelpContent(helpHeader, helpContent);
    }

    public void Silence()
    {
        listenToInput = false;
        batsmanInteractionPanel.SetActive(false);
    }

}
