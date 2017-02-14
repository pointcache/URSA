using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;
using System.Linq;
using SmartConsole;

[Serializable]
public class rVar {
}

public interface IrVar {
    object getValue();
    void setValue(object val);
    void registerInConsole(string name, string description);
    void unregisterInConsole(string name);
}

[Serializable]
public class rVar<T> : rVar, IrVar {
    [SerializeField]
    [HideInInspector]
    T value;
    [fsProperty]
    public T Value
    {
        get { return value; }
        set {
            evaluate(value);
            OnChanged(value);
            this.value = value;
        }
    }
    public rVar() {
    }
    public rVar(T initval) {
        Value = initval;
    }
    public event Action<T> OnChanged = delegate { };
    //Used in child classes for raising specific conditional events.
    protected Action<T> evaluate = delegate { };
    public override string ToString() {
        return value.ToString();
    }

    public object getValue() {
        return Value;
    }

    public void setValueDirectly(object val) {
        value = (T)val;
    }

    public void setValue(object val) {
        Value = (T)val;
    }

    public void registerInConsole(string name, string description) {
        SmartConsole.SmartConsole.RegisterVariable(this, name, description);
    }

    public void unregisterInConsole(string name) {
        SmartConsole.SmartConsole.UnregisterVariable(name);
    }

    public rVar<T> SetFromString(string value) {
        Value = ((T)System.Convert.ChangeType(value, typeof(T)));
        return this;
    }

    public rVar<T> Subscribe(Action<T> action) {
        OnChanged += action;
        return this;
    }

    public rVar<T> SubAndUpdate(Action<T> action) {
        OnChanged += action;
        Update();
        return this;
    }
    /// <summary>
    /// Connected var receives OnChanged events from the one it is connected to
    /// </summary>
    /// <param name="other"></param>
    public void Connect(rVar<T> to) {
        to.OnChanged += connectedCall;
        value = to.Value;
    }

    public void Disconnect(rVar<T> from) {
        from.OnChanged -= connectedCall;
    }

    void connectedCall(T val) {
        Value = val;
    }

    /// <summary>
    /// Sets the value to itself raising OnChanged
    /// </summary>
    public rVar<T> Update() {
        Value = Value;
        return this;
    }
}

[Serializable]
public class r_int : rVar<int> {
    public r_int() : base() { }
    public r_int(int initialValue) : base(initialValue) { }
    public static implicit operator int(r_int var) {
        return var.Value;
    }
}
[Serializable]
public class r_float : rVar<float> {
    public r_float() : base() { }
    public r_float(float initialValue) : base(initialValue) { }
    public static implicit operator float(r_float var) { return var.Value; }
}

[Serializable]
public class r_string : rVar<string> {
    public r_string() : base() { }
    public r_string(string initialValue) : base(initialValue) { }
    public static implicit operator string(r_string var) {
        return var.Value;
    }

}
[Serializable]
public class r_double : rVar<double> {
    public r_double() : base() { }
    public r_double(double initialValue) : base(initialValue) { }
    public static implicit operator double(r_double var) {
        return var.Value;
    }
}
[Serializable]
public class r_bool : rVar<bool> {
    public r_bool() : base() { InitOnTrue(); }
    public r_bool(bool initialValue) : base(initialValue) {
        InitOnTrue();
    }

    void InitOnTrue() {
        evaluate += x => { if (x == true) OnTrue(); };
    }


    public static implicit operator bool(r_bool var) {
        return var.Value;
    }
    public event Action OnTrue = delegate { };
}

[Serializable]
public class r_KeyCode : rVar<KeyCode> {
    public r_KeyCode() : base() { }
    public r_KeyCode(KeyCode initialValue) : base(initialValue) { }
    public static implicit operator KeyCode(r_KeyCode var) {
        return var.Value;
    }
}

[Serializable]
public class r_Color : rVar<Color> {
    public r_Color() : base() { }
    public r_Color(Color initialValue) : base(initialValue) { }
    public static implicit operator Color(r_Color var) {
        return var.Value;
    }
}

[Serializable]
public class r_Vector3 : rVar<Vector3> {
    public r_Vector3() : base() { }
    public r_Vector3(Vector3 initialValue) : base(initialValue) { }
    public static implicit operator Vector3(r_Vector3 var) {
        return var.Value;
    }
}
[Serializable]
public class r_Vector2 : rVar<Vector2> {
    public r_Vector2() : base() { }
    public r_Vector2(Vector2 initialValue) : base(initialValue) { }
    public static implicit operator Vector2(r_Vector2 var) {
        return var.Value;
    }
}
[Serializable]
public class r_Quaternion : rVar<Quaternion> {
    public r_Quaternion() : base() { }
    public r_Quaternion(Quaternion initialValue) : base(initialValue) { }
    public static implicit operator Quaternion(r_Quaternion var) {
        return var.Value;
    }
}
[Serializable]
public class r_GameObject : rVar<GameObject> {
    public r_GameObject() : base() { }
    public r_GameObject(GameObject initialValue) : base(initialValue) { }
    public static implicit operator GameObject(r_GameObject var) {
        return var.Value;
    }
}
[Serializable]
public class r_uObject : rVar<UnityEngine.Object> {
    public r_uObject() : base() { }
    public r_uObject(UnityEngine.Object initialValue) : base(initialValue) { }
    public static implicit operator UnityEngine.Object(r_uObject var) {
        return var.Value;
    }
}

