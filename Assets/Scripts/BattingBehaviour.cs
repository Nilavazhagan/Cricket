using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattingBehaviour : MonoBehaviour, ICricketBehaviour
{
    public float minX = -1f, maxX = 1f;
    public float movementSpeed = 5f;
    public Rigidbody Ball;
    public float hitPower = 5f;

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
        Vector3 pos = transform.position;
        pos.x = 0;
        transform.position = pos;
    }

    public void ListenToInput()
    {
        listenToInput = true;
        HelpManager.Instance.UpdateHelpContent(helpHeader, helpContent);
    }

    public void Silence()
    {
        listenToInput = false;
    }

}
