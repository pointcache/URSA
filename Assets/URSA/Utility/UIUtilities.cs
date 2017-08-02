using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public static class UIUtilities {

    public static void AddButtonAction(this UnityEvent ev, System.Action action) {
        ev.AddListener(() => { action(); });
    }

    public static void SetCanvasGroupInteractions(this GameObject go, bool state) {
        var cg = go.GetComponent<CanvasGroup>();
        cg.interactable = state;
        cg.blocksRaycasts = state;
    }

}
