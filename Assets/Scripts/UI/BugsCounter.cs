using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BugsCounter : GameCounter
{
    public override void UpdateCounter()
    {
        Counter.text = GameRules.BugsCount.ToString();
    }
}
