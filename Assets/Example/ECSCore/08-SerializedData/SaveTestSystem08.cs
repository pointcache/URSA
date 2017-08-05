using UnityEngine;
using System;
using System.Collections.Generic;
using URSA;
using URSA.Serialization;
using URSA.Utility;
using UnityEngine.UI;

public class SaveTestSystem08 : ECSSystem {


	internal override void OrderedOnEnable() {
        base.OrderedOnEnable();

        SaveObject so = SaveSystem.CreateSaveObjectFromEntity(Pool<ExampleComponent08>.First.Entity);

        string json = SerializationHelper.Serialize(so, true);

        Pool<UITextComponent>.First.GetComponent<Text>().text = json;
    }
	
	internal override void OrderedUpdate() {
        base.OrderedUpdate();


    }

}
