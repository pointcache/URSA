using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerStatus : ComponentBase {

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public r_bool PlayerUnlockedAct1 = new r_bool();
        public r_bool PlayerUnlockedAct2 = new r_bool();
        public r_bool PlayerUnlockedAct3 = new r_bool();

        public r_Color PlayerColor = new r_Color();
        public r_int PlayerCurrency = new r_int();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
