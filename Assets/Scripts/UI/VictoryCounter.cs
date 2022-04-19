using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryCounter : GameCounter
{
    public override void UpdateCounter()
    {
        Counter.text = GameRules.BugsResolvedCount.ToString();
    }
}
