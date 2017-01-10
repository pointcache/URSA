using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class DebugGuiInspectorExpander : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool drag;
    public RectTransform target;
    public float mousedelta;
    public Vector3 prevmouse;
    private void Update()
    {
        if (!drag)
            return;

        var pos = Input.mousePosition;

        mousedelta = pos.x - prevmouse.x;
        prevmouse = pos ;

        var v = target.sizeDelta;
        v.x += mousedelta;
        target.sizeDelta = v;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        drag = true;
        prevmouse = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        drag = false;
    }
}
