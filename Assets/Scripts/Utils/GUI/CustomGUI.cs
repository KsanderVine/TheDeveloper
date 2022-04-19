using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CustomGUI : MonoBehaviour
{
    public static CustomGUI gui;
    public void OnEnable () { gui = this; }

    public GUISkin customSkin;
}
