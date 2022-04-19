using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameCounter : MonoBehaviour
{
    public Text Counter;
    public void SetDefault()
    {
        Counter.text = "0";
    }
    public abstract void UpdateCounter();
}
