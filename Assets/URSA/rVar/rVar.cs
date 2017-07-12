using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;
using System.Linq;

/// <summary>
/// Base for working with generic collections. Typical c# hack
/// </summary>
[Serializable]
public class RVar {
}

//Interface for communicating with uncasted class from other things
public interface IRVar {
    object GetValue();
    void SetValue(object val);
}

[Serializable]
public class RVar<T> : RVar, IRVar {

    [SerializeField]
    [HideInInspector]
    T value;
    /// <summary>
    /// The value is set AFTER the callback
    /// </summary>
    [fsProperty]
    public T Value
    {
        get { return value; }
        set {
            T val = value;
            if (OnPreEvaluate != null)
                val = OnPreEvaluate(value);
            if (evaluate != null)
                evaluate(val);
            this.value = val;
            if (OnChanged != null)
                OnChanged(val);
        }
    }
    public RVar()
            : this(default(T)) {
    }

    public RVar(T initval) {
        Value = initval;
    }
    /// <summary>
    /// Subscribe to receive the value before it is set so you can modify it (clamp for example or whatever)
    /// return the modified value 
    /// </summary>
    public event Func<T, T> OnPreEvaluate;
    /// <summary>
    /// Subscribe to receive notification after the value was internally set.
    /// </summary>
    public event Action<T> OnChanged;
    //Used in child classes for raising specific conditional events.
    protected event Action<T> evaluate;
    public override string ToString() {
        return value.ToString();
    }

    public object GetValue() {
        return Value;
    }

    public void setValueDirectly(object val) {
        value = (T)val;
    }

    public void SetValue(object val) {
        Value = (T)val;
    }

    public RVar<T> SetFromString(string value) {
        Value = ((T)System.Convert.ChangeType(value, typeof(T)));
        return this;
    }

    public RVar<T> Subscribe(Action<T> action) {
        OnChanged += action;
        return this;
    }

    public RVar<T> SubAndUpdate(Action<T> action) {
        OnChanged += action;
        Update();
        return this;
    }

    public void RemoveAllListeners() {
        OnChanged = null;
        OnPreEvaluate = null;
        evaluate = null;
    }

    /// <summary>
    /// Connected var receives OnChanged events from the one it is connected to
    /// </summary>
    /// <param name="other"></param>
    public void Connect(RVar<T> to) {
        to.OnChanged += connectedCall;
        value = to.Value;
    }

    public void Disconnect(RVar<T> from) {
        from.OnChanged -= connectedCall;
    }

    void connectedCall(T val) {
        Value = val;
    }

    /// <summary>
    /// Sets the value to itself raising events
    /// </summary>
    public RVar<T> Update() {
        Value = Value;
        return this;
    }
}

[Serializable]
public class r_int : RVar<int> {
    public r_int() : base() { }
    public r_int(int initialValue) : base(initialValue) { }
    public static implicit operator int(r_int var) {
        return var.Value;
    }
}
[Serializable]
public class r_float : RVar<float> {
    public r_float() : base() { }
    public r_float(float initialValue) : base(initialValue) { }
    public static implicit operator float(r_float var) { return var.Value; }
}

[Serializable]
public class r_string : RVar<string> {
    public r_string() : base() { }
    public r_string(string initialValue) : base(initialValue) { }
    public static implicit operator string(r_string var) {
        return var.Value;
    }

}
[Serializable]
public class r_double : RVar<double> {
    public r_double() : base() { }
    public r_double(double initialValue) : base(initialValue) { }
    public static implicit operator double(r_double var) {
        return var.Value;
    }
}
[Serializable]
public class r_bool : RVar<bool> {
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
public class r_KeyCode : RVar<KeyCode> {
    public r_KeyCode() : base() { }
    public r_KeyCode(KeyCode initialValue) : base(initialValue) { }
    public static implicit operator KeyCode(r_KeyCode var) {
        return var.Value;
    }
}

[Serializable]
public class r_Color : RVar<Color> {
    public r_Color() : base() { }
    public r_Color(Color initialValue) : base(initialValue) { }
    public static implicit operator Color(r_Color var) {
        return var.Value;
    }
}

[Serializable]
public class r_Vector3 : RVar<Vector3> {
    public r_Vector3() : base() { }
    public r_Vector3(Vector3 initialValue) : base(initialValue) { }
    public static implicit operator Vector3(r_Vector3 var) {
        return var.Value;
    }
}
[Serializable]
public class r_Vector2 : RVar<Vector2> {
    public r_Vector2() : base() { }
    public r_Vector2(Vector2 initialValue) : base(initialValue) { }
    public static implicit operator Vector2(r_Vector2 var) {
        return var.Value;
    }
}
[Serializable]
public class r_Quaternion : RVar<Quaternion> {
    public r_Quaternion() : base() { }
    public r_Quaternion(Quaternion initialValue) : base(initialValue) { }
    public static implicit operator Quaternion(r_Quaternion var) {
        return var.Value;
    }
}
[Serializable]
public class r_GameObject : RVar<GameObject> {
    public r_GameObject() : base() { }
    public r_GameObject(GameObject initialValue) : base(initialValue) { }
    public static implicit operator GameObject(r_GameObject var) {
        return var.Value;
    }
}
[Serializable]
public class r_uObject : RVar<UnityEngine.Object> {
    public r_uObject() : base() { }
    public r_uObject(UnityEngine.Object initialValue) : base(initialValue) { }
    public static implicit operator UnityEngine.Object(r_uObject var) {
        return var.Value;
    }
}


[Serializable]
public class r_ShaderColor : RVar<Color> {
    [SerializeField]
    string ShaderWord;

    public r_ShaderColor(string shaderword) : base() {
        ShaderWord = shaderword;
        evaluate += SetShaderColor;
    }
    public r_ShaderColor(string shaderword, Color initialValue) : base(initialValue) {
        ShaderWord = shaderword;
        evaluate += SetShaderColor;
    }
    public static implicit operator Color(r_ShaderColor var) {
        return var.Value;
    }

    void SetShaderColor(Color color) {
        Shader.SetGlobalColor(ShaderWord, color);
    }
}

[Serializable]
public class r_ShaderFloat : RVar<float> {
    [SerializeField]
    string ShaderWord;

    public r_ShaderFloat(string shaderword) : base() {
        ShaderWord = shaderword;
        evaluate += SetShaderFloat;
    }
    public r_ShaderFloat(string shaderword, float initialValue) : base(initialValue) {
        ShaderWord = shaderword;
        evaluate += SetShaderFloat;
    }
    public static implicit operator float(r_ShaderFloat var) {
        return var.Value;
    }

    void SetShaderFloat(float value) {
        Shader.SetGlobalFloat(ShaderWord, value);
    }
}

