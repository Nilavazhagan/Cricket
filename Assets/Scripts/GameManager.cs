using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            throw new System.Exception("Attempted to create a second GameManager Instance");
        }
        else
        {
            Instance = this;
        }
    }

    //public ICricketBehaviour battingBehaviour, bowlingBehaviour;          //UNABLE to use ICricketBehaviour as it is not shown in inspector
    public BattingBehaviour battingBehaviour;
    public BowlingBehaviour bowlingBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        bowlingBehaviour.OnInputsReceived += BowlerReady;
        battingBehaviour.OnInputsReceived += BatsmanReady;

        Restart();
    }

    void BowlerReady()
    {
        bowlingBehaviour.Silence();
        battingBehaviour.ListenToInput();
    }

    void BatsmanReady()
    {
        bowlingBehaviour.Silence();
        battingBehaviour.Silence();

        bowlingBehaviour.Play();
        battingBehaviour.Play();

        Invoke("Restart", 3f);
    }

    void Restart()
    {
        bowlingBehaviour.Reset();
        battingBehaviour.Reset();

        bowlingBehaviour.ListenToInput();
        battingBehaviour.Silence();
    }
}
