using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
public class DebugGuiEditableProperty : MonoBehaviour
{
    public Text Name, value;
    public InputField inputfield;
    ComponentData comp;
    FieldInfo field;
    PropertyInfo prop;
    object rvar;
    public type _type;
    bool initialized;
    public enum type
    {
        field, prop, rvar
    }

    public void Initialize(ComponentData _comp, FieldInfo _field, PropertyInfo _prop, object _rvar, string _Name)
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
            comp = _comp;
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
            comp = _comp;
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

        initialized = true;
    }
    private void Update()
    {
        if (!initialized)
            return;
        if (_type == type.field)
        {
            value.text = field.GetValue(comp).ToString();

        }
        else
            if (_type == type.rvar)
        {
            value.text = prop.GetValue(rvar, null).ToString();
        }


    }
    public void SetValue(string input)
    {
        if (input == String.Empty)
            return;
        if (_type == type.field)
        {
            Type t = field.FieldType;
            field.SetValue(comp, Convert.ChangeType(input, t));
        }
        else
            if(_type == type.rvar)
        {
            Type t = prop.PropertyType;
            prop.SetValue(rvar, Convert.ChangeType(input, t), null);
        }
    }
}
