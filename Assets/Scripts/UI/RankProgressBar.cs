using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankProgressBar : MonoBehaviour
{
    public Text RankTitle;
    public Image FillBar;
    
    private float _progress;
    private int _rank;
    private const float lerpSpeed = 5f;

    public void SetDefault ()
    {
        _rank = 0;
        FillBar.fillAmount = 0;
        RankTitle.text = Lexicon.GetDef("UI.RankAndLevel", _rank, GameRules.GetRankTitle());
    }

    public void Update()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals (bool isForced = false)
    {
        int currentRank = GameRules.GetRank(Mathf.FloorToInt(GameRules.Score));

        float progress = GameRules.GetRankProgress(Mathf.FloorToInt(GameRules.Score));
        _progress = Mathf.Lerp(_progress, progress, Time.deltaTime * lerpSpeed);
        FillBar.fillAmount = _progress;

        if (isForced || currentRank != _rank || Time.unscaledTime % 10 == 0)
        {
            _rank = currentRank;
            RankTitle.text = Lexicon.GetDef("UI.RankAndLevel", _rank, GameRules.GetRankTitle());
        }
    }
}
