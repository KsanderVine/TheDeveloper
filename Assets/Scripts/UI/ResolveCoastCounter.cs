using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ResolveCoastCounter : GameCounter
{
    public override void UpdateCounter()
    {
        Counter.text = string.Format("{0:n}", GameRules.ResolveCoast);
    }
}
