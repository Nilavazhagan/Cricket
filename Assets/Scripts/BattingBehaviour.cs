using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattingBehaviour : MonoBehaviour
{
    public float minX = -1f, maxX = 1f;
    public float movementSpeed = 5f;
    public GameObject Ball;
    public float hitPower = 5f;

    [HideInInspector]
    public bool listenToInput = false;

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
            if (AreArrowKeysPressed() && batsmanCollider.bounds.Contains(Ball.transform.position))
            {
                Vector3 input = new Vector3
                {
                    x = Input.GetKey(KeyCode.RightArrow) ? 1 : (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0),
                    y = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? 0.5f : 0,
                    z = Input.GetKey(KeyCode.UpArrow) ? -0.5f : (Input.GetKey(KeyCode.DownArrow) ? -1 : 0)
                };

                Ball.GetComponent<Rigidbody>().AddForce(input * hitPower, ForceMode.Impulse);
                //HitDirection hitDir = GetHitDirection(input);

                listenToInput = false;
            }
        }
    }

    bool AreArrowKeysPressed()
    {
        return (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow));
    }

    HitDirection GetHitDirection(Vector2Int input)
    {
        HitDirection[,] directionArray = {
            { HitDirection.DOWN_LEFT, HitDirection.LEFT, HitDirection.UP_LEFT },
            { HitDirection.DOWN, HitDirection.NONE, HitDirection.UP },
            { HitDirection.DOWN_RIGHT, HitDirection.RIGHT, HitDirection.UP_RIGHT }
        };

        try { 
            return directionArray[input.x + 1, input.y + 1];
        }catch(System.Exception e)
        {
            return HitDirection.NONE;
        }
    }

    public void Reset()
    {
        listenToInput = false;
        Vector3 pos = transform.position;
        pos.x = 0;
        transform.position = pos;
    }

    enum HitDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT,
        NONE
    }
}
