using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    public static GameRules Instance { get; private set; }

    public static int Seed { get; private set; }
    public static int Score { get; private set; }
    public static int ResolveCoast { get; private set; }
    public static int BugsCount { get; private set; }
    public static int BugsResolvedCount { get; private set; }

    public static int BestRank { get; private set; }
    public static int BestScore { get; private set; }

    public SoundSettings GameStartSound;
    public SoundSettings AmbienceSound;
    public SoundSettings MusicSound;

    private AudioSource ambienceSource;
    private AudioSource musicSource;

    public Transform[] CharacterSpawners;

    private const int _scorePerRank = 20;
    private const int _targetRank = 50;
    private const int _specialsCount = 30;

    private static string[] _ranks = new string[]
    {
        "Intern", "Junior", "Middle", "Senior", "Lead"
    };

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        RestartLevel(false);
    }

    public static void ChangeScore (int score)
    {
        Score += score;
    }

    public static void ChangeResolveCoast(int resolveCoast)
    {
        ResolveCoast += resolveCoast;
        GameCounters.Instance.ResolveCoastCounter.UpdateCounter();
    }

    public static void ChangeBugsCounter (int count)
    {
        BugsCount += count;
        GameCounters.Instance.BugsCounter.UpdateCounter();
    }

    public static void ChangeBugsResolvedCounter(int count)
    {
        BugsResolvedCount += count;
        GameCounters.Instance.VictoryCounter.UpdateCounter();
    }

    public static float GetGameProgress()
    {
        return Mathf.Min(1f, (float)GetRank() / (float)_targetRank);
    }

    public static float GetRankProgress ()
    {
        float scorePerRank = GameRules._scorePerRank;
        return Mathf.Repeat(Score, scorePerRank) / scorePerRank;
    }

    public static int GetRank ()
    {
        return Mathf.CeilToInt(Score / _scorePerRank);
    }

    public static float GetRankProgress(int score)
    {
        float scorePerRank = GameRules._scorePerRank;
        return Mathf.Repeat(score, scorePerRank) / scorePerRank;
    }

    public static int GetRank(int score)
    {
        return Mathf.CeilToInt(score / _scorePerRank);
    }

    public static string GetRankTitle ()
    {
        int rank = GetRank();

        if (rank == 0)
            return Lexicon.GetDef("Enum.Rank.Bob");

        if (rank >= _targetRank)
            return Lexicon.GetDef("Enum.Rank.TheDeveloper");

        float levelsPerRank = Mathf.Ceil(_targetRank / _ranks.Length);
        string rankTitle = _ranks[Mathf.FloorToInt(rank / levelsPerRank)];
        rankTitle = Lexicon.GetDef("Enum.Rank." + rankTitle).ToLower();

        int specialIndex = Mathf.FloorToInt(((Seed + rank) % _specialsCount));
        string specialSpell = Lexicon.GetDef("Enum.Special.Spell" + specialIndex);

        int gradeIndex = Mathf.FloorToInt(rank % 10);
        string grade = Lexicon.GetDef("Enum.Grade.Grade" + gradeIndex);

        return grade + " " + rankTitle + " " + specialSpell;
    }

    public static void FearAllBugs()
    {
        Bug[] bugs = FindObjectsOfType<Bug>(true);
        for (int i = 0; i < bugs.Length; i++)
        {
            Vector2 direction = bugs[i].transform.position - Character.Active.transform.position;
            direction = Vector2.ClampMagnitude(direction, 1f) * 2f;
            bugs[i].PushImpulse(direction);
        }
    }

    public static void KillAllBugs ()
    {
        Bug[] bugs = FindObjectsOfType<Bug>(true);
        for(int i = 0; i < bugs.Length; i++)
        {
            bugs[i].Die();
        }
    }

    public static void ChangeSeed ()
    {
        Seed = Random.Range(0, int.MaxValue / 2);
    }

    public static void BakeResults ()
    {
        int rank = GetRank();

        if (rank > BestRank)
        {
            BestRank = rank;
        }

        if (Score > BestScore)
        {
            BestScore = Score;
        }
    }

    public static void RestartLevel (bool killCharacter)
    {
        ChangeSeed();

        Score = 0;
        ResolveCoast = 0;
        KillAllBugs();

        if (killCharacter)
            Character.Active.DieAndRestart(true);

        GameCounters.SetDefault();
        BugsSpawner.Instance.IsActive = true;

        SoundManager.Play(Instance.GameStartSound);

        if(Instance.ambienceSource == null)
        {
            Instance.ambienceSource = SoundManager.Play(Instance.AmbienceSound);
            Instance.ambienceSource.Play();
        }

        if (Instance.musicSource == null)
        {
            Instance.musicSource = SoundManager.Play(Instance.MusicSound);
            Instance.musicSource.Play();
        }
    }
    
    public static float GetProgress()
    {
        float t = GetGameProgress();
        float x1 = 0f;
        float x2 = 0.05f;
        float x3 = 1f;
        float progress = Mathf.Pow(1 - t, 2) * x1 + 2 * (1 - t) * t * x2 + Mathf.Pow(t, 2) * x3;
        return Mathf.Clamp(progress, 0f, 1f);
    }
}
