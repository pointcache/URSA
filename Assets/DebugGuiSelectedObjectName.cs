using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DebugGuiSelectedObjectName : MonoBehaviour {


    private Text _text;
    public Text text { get { if (!_text) _text = GetComponent<Text>(); return _text; } }


    private void OnEnable() {
        DebugSystem.selectedName.OnChanged -= setname;
        DebugSystem.selectedName.OnChanged += setname;
    }

    void setname(string str) {
        text.text = str;
    }
}
