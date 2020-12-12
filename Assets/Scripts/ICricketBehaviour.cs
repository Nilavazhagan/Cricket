using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICricketBehaviour
{
    InputsReceived OnInputsReceived { get; set; }
    void ListenToInput();
    void Silence();
    void Reset();
    void Play();
}

public delegate void InputsReceived();