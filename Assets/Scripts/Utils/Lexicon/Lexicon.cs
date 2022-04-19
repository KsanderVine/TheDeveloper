using UnityEngine;
using System.Xml;

/* * * * * * * * * * * * * * * * * * * * *
 * Lexicon
 * Version 3.01
 * ------------------------------
 * W: Alexander Vine 
 * Crt: 2013 July 6
 * Upd: 2022 April 18
 * * * * * * * * * * * * * * * * * * * * */

public sealed class Lexicon : MonoBehaviour
{
    public static Lexicon Instance { get; private set; }
    private static XMLZip TranslationZip { get; set; }

    public static string LanguageTitle => TranslationZip.SearchRoot("Data.Language").Value;

    public static SystemLanguage SystemLanguage => Application.systemLanguage;
    public static string SettingsLanguage = defaultLanguage;
    public const string defaultLanguage = "ru";

    [SerializeField]
    private TranslationAsset[] translationAssets;

    [System.Serializable]
    public struct TranslationAsset
    {
        public string LocalizedTitle;
        public string Code => TextAsset.name;
        public TextAsset TextAsset;
    }

    public void Awake()
    {
        Instance = this;
        Translate();
    }

    private static TranslationAsset FindTranslation(string language)
    {
        for (int i = 0; i < Instance.translationAssets.Length; i++)
        {
            if (Instance.translationAssets[i].TextAsset.name == language)
            {
                return Instance.translationAssets[i];
            }
        }
        return FindTranslation(defaultLanguage);
    }

    public static void Translate ()
    {
        System.Diagnostics.Stopwatch totalTime = new System.Diagnostics.Stopwatch();
        totalTime.Start();

        TranslationAsset translation = FindTranslation(SettingsLanguage);
        TextAsset textAsset = translation.TextAsset;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        XMLZip translationResult = null;

        try
        {
            translationResult = XMLConstructor.ExtractDocument(xmlDoc);
        }
        catch (System.InvalidCastException exeption)
        {
            Debug.LogError($"[{nameof(Lexicon)}] XML extraction errors\n{exeption}");
        }

        if (translationResult == null || translationResult.IsEmpty())
        {
            Debug.Log(translationResult.Item + " " + translationResult.ToString());
            Debug.LogError($"[{nameof(Lexicon)}] TranslationAsset is empty");
            return;
        }

        TranslationZip = translationResult;
        
        LXHandler[] handlers = FindObjectsOfType<LXHandler>();
        for (int i = 0; i < handlers.Length; i++)
        {
            handlers[i].Translate();
        }

        totalTime.Stop();
        Debug.Log($"[{nameof(Lexicon)}] Translation done in {totalTime.ElapsedMilliseconds} ms");
    }

    public static string GetDef (string root)
    {
        XMLZip rootXML = TranslationZip.SearchRoot(root);

        if (rootXML == null)
        {
            Debug.Log($"[{nameof(Lexicon)}] Def root not found: " + root);
            return root;
        }
        
        return rootXML.Value;
    }

    public static string GetDef (string root, params object[] args)
    {
        XMLZip rootXML = TranslationZip.SearchRoot(root);

        if (rootXML == null)
        {
            Debug.Log($"[{nameof(Lexicon)}] Def root not found: " + root);
            return root;
        }

        string def = rootXML.Value;

        for (int i = 0; i < args.Length; i++)
        {
            if (def.Contains("{" + i + "}"))
            {
                def = def.Replace("{" + i + "}", args[i].ToString());
            }
        }

        return def;
    }

    public static void SetPrevTranslation()
    {
        int ID = 0;

        for (int i = 0; i < Instance.translationAssets.Length; i++)
        {
            if (Instance.translationAssets[i].Code == SettingsLanguage)
            {
                ID = i;
                break;
            }
        }

        ID--;
        if (ID < 0)
        {
            ID = Instance.translationAssets.Length - 1;
        }

        SettingsLanguage = Instance.translationAssets[ID].Code;
        Translate();
    }

    public static void SetNextTranslation()
    {
        int ID = 0;

        for (int i = 0; i < Instance.translationAssets.Length; i++)
        {
            if (Instance.translationAssets[i].Code == SettingsLanguage)
            {
                ID = i;
                break;
            }
        }

        ID++;

        if (ID >= Instance.translationAssets.Length)
        {
            ID = 0;
        }

        SettingsLanguage = Instance.translationAssets[ID].Code;
        Translate();
    }
}