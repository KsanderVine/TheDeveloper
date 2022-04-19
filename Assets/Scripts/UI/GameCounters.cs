using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCounters : MonoBehaviour
{
    public static GameCounters Instance;
    public void Awake()
    {
        Instance = this;
    }

    public RankProgressBar RankProgressBar;
    public BugsCounter BugsCounter;
    public ResolveCoastCounter ResolveCoastCounter;
    public VictoryCounter VictoryCounter;

    public static void SetDefault ()
    {
        Instance.RankProgressBar.SetDefault();
        Instance.BugsCounter.SetDefault();
        Instance.ResolveCoastCounter.SetDefault();
        Instance.VictoryCounter.SetDefault();
    }
}
