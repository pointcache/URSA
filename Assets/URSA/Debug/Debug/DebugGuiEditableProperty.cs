using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
public class DebugGuiEditableProperty : MonoBehaviour
{
    public Text Name, value;
    public InputField inputfield;
    object target;
    FieldInfo field;
    PropertyInfo prop;
    object rvar;
    public type _type;
    bool initialized;
    public enum type
    {
        field, prop, rvar
    }

    public void Initialize(object _target, FieldInfo _field, PropertyInfo _prop, object _rvar, string _Name)
    {
        if (_rvar != null)
            _type = type.rvar;
        else
            if (_prop != null)
            _type = type.prop;
        else
            _type = type.field;

        Name.text = _Name;

        if (_type == type.field)
        {
            target = _target;
            field = _field;
            if (field.FieldType == typeof(float))
                inputfield.characterValidation = InputField.CharacterValidation.Decimal;
            else
                if (field.FieldType == typeof(int))
                inputfield.characterValidation = InputField.CharacterValidation.Integer;
        }
        else
        if (_type == type.prop)
        {
            target = _target;
            prop = _prop;
            if (prop.PropertyType == typeof(float))
                inputfield.characterValidation = InputField.CharacterValidation.Decimal;
            else
                if (prop.PropertyType == typeof(int))
                inputfield.characterValidation = InputField.CharacterValidation.Integer;
        }
        else
        if (_type == type.rvar)
        {
            rvar = _rvar;
            prop = _prop;
            if (prop.PropertyType == typeof(float))
                inputfield.characterValidation = InputField.CharacterValidation.Decimal;
            else
                if (prop.PropertyType == typeof(int))
                inputfield.characterValidation = InputField.CharacterValidation.Integer;
        }
        inputfield.text = getvalue();

        initialized = true;
    }
    private void Update()
    {
        if (!initialized)
            return;
        if (_type == type.field)
        {
            value.text = field.GetValue(target).ToString();

        }
        else
            if (_type == type.rvar)
        {
            value.text = prop.GetValue(rvar, null).ToString();
        }


    }

    string getvalue() {
        if (_type == type.field) {
            return field.GetValue(target).ToString();

        } else
            if (_type == type.rvar) {
            return  prop.GetValue(rvar, null).ToString();
        } else
            return "";
    }
    public void SetValue(string input)
    {
        if (input == String.Empty)
            return;
        if (_type == type.field)
        {
            Type t = field.FieldType;
            field.SetValue(target, Convert.ChangeType(input, t));
        }
        else
            if(_type == type.rvar)
        {
            Type t = prop.PropertyType;
            prop.SetValue(rvar, Convert.ChangeType(input, t), null);
        }
    }
}
