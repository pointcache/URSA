using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;

public class PlayerProfile13 : ECSComponent {

	

    public Data data;
    [Serializable]
    public class Data : SerializedData
    {
        public string PlayerName;
        public float PlayerMoney;
        [Range(10,59)]
        public int PlayerIQ;
    }
}
