using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LXHandler : MonoBehaviour
{
    protected UnityEngine.UI.Text container;
    public void OnEnable()
    {
        Translate();
    }
    public void Translate ()
    {
        container = GetComponent<UnityEngine.UI.Text>();
        container.text = Lexicon.GetDef(gameObject.name);
    }
}
