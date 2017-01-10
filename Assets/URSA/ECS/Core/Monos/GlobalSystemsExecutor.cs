using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GlobalSystemsExecutor : MonoBehaviour
{
    void OnEnable()
    {
        sendMessage("orderedOnEnable");
    }

    //void OnDisable()
    //{
    //    reversesendMessage("orderedOnDisable");
    //}

    void FixedUpdate()
    {
       sendMessage("orderedFixedUpdate");
    }

    void Update()
    {
        sendMessage("orderedUpdate");
    }

    void sendMessage(string message)
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            transform.GetChild(i).SendMessage(message, SendMessageOptions.DontRequireReceiver);
        }
    }

   // void reversesendMessage(string message)
   // {
   //     int count = transform.childCount;
   //     for (int i = count; i > 0; i--)
   //     {
   //         transform.GetChild(i).SendMessage(message, SendMessageOptions.DontRequireReceiver);
   //     }
   // }
}


