using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattingBehaviour : MonoBehaviour, ICricketBehaviour
{
    public float minX = -1f, maxX = 1f;
    public float movementSpeed = 5f;
    public GameObject Ball;
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
                hitDirection = new Vector3
                {
                    x = Input.GetKey(KeyCode.RightArrow) ? 1 : (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0),
                    y = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? 0.5f : 0,
                    z = Input.GetKey(KeyCode.UpArrow) ? -0.5f : (Input.GetKey(KeyCode.DownArrow) ? -1 : 0)
                };
                OnInputsReceived?.Invoke();
                //Ball.GetComponent<Rigidbody>().AddForce(input * hitPower, ForceMode.Impulse);
                //HitDirection hitDir = GetHitDirection(input);

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
            if (other.CompareTag("Ball"))
            {
                other.GetComponent<Rigidbody>().AddForce(hitDirection * hitPower, ForceMode.Impulse);
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
