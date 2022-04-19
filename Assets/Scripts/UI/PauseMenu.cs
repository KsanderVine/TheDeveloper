using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
    }

    public GameObject MainPanel;
    public Image Background;

    public Text LanguageTitle;
    public Text VersionTitle;
    public Text ResolvingCoast;
    public Text BestRank;
    public Text BestScore;

    public float BackgroundAlpha = .8f;
    private bool _isVisible;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isVisible = !_isVisible;
            if (_isVisible)
            {
                Redraw();
            }
        }

        Color color = Background.color;
        float step = Time.unscaledDeltaTime * (_isVisible == true ? 8f : -8f);
        color.a = Mathf.Clamp(color.a + step, 0f, BackgroundAlpha);
        Background.color = color;

        bool isHidden = !(_isVisible == false && color.a == 0);
        Time.timeScale = isHidden == true ? 0 : 1;

        if (MainPanel.activeSelf != isHidden)
        {
            MainPanel.SetActive(isHidden);
        }
    }

    public void OnOption (int optionIndex)
    {
        switch(optionIndex)
        {
            case 1: //Back to game
                _isVisible = false;
                break;
            case 2: //Restart
                GameRules.RestartLevel(true);
                _isVisible = false;
                break;
            case 3: //Exit Game
                Application.Quit();
                break;
            case 4: //Left Lang
                Lexicon.SetPrevTranslation();
                Redraw();
                break;
            case 5: //Right Lang
                Lexicon.SetNextTranslation();
                Redraw();
                break;
        }
    }

    private void Redraw ()
    {
        LanguageTitle.text = Lexicon.LanguageTitle;
        ResolvingCoast.text = Lexicon.GetDef("UI.ResolvingCoast", GameRules.ResolveCoast);
        BestRank.text = Lexicon.GetDef("UI.BestRank", GameRules.BestRank);
        BestScore.text = Lexicon.GetDef("UI.BestScore", GameRules.BestScore);
        VersionTitle.text = Lexicon.GetDef("UI.Version", Application.version);
        GameCounters.Instance.RankProgressBar.UpdateVisuals(true);
    }
}
