using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace RFG
{
  /// <summary>
  /// Container class for a float variable with a clamped range. Also handy for minimum/maximum checks to variable parameters.
  /// Can easily be expanded or modified to add features.
  /// </summary>
  [System.Serializable]
  public class FloatRange : ISerializable
  {
    // These attributes are needed for Inspector support in Unity3.x
    // If using 4.x than a PropertyDrawer can be created instead.
    [HideInInspector]
    [SerializeField]
    private float _currValue;
    [SerializeField]
    private float _min;
    [SerializeField]
    private float _max;

    #region Public Properties
    public float minimumValue
    {
      get { return _min; }
      set
      {
        if (value > maximumValue)
        {
          float temp = _max;
          _max = value;
          _min = temp;
          _currValue = temp;
        }
        else
        {
          if (value <= _currValue)
            _min = value;
          else
          {
            _min = value;
            _currValue = value;
          }
        }
      }
    }
    public float maximumValue
    {
      get { return _max; }
      set
      {
        if (value < minimumValue)
        {
          float temp = _min;
          _min = value;
          _max = temp;
          _currValue = temp;
        }
        else
        {
          if (value >= _currValue)
            _max = value;
          else
          {
            _max = value;
            _currValue = value;
          }
        }
      }
    }
    public float Value
    {
      get { return _currValue; }
      set
      {
        if (value <= minimumValue)
          _currValue = minimumValue;
        else if (value >= maximumValue)
          _currValue = maximumValue;
        else
          _currValue = value;
      }
    }
    #endregion

    #region constructors
    public FloatRange()
    {
      _min = 0f;
      _max = 1f;
      Value = 0f;
    }
    public FloatRange(Int16 value)
    {
      _min = -32767;
      _max = 32767;
      Value = value;
    }
    public FloatRange(Int32 value)
    {
      _min = -2147483647;
      _max = 2147483647;
      Value = value;
    }
    public FloatRange(Int64 value)
    {
      _min = -9223372036854775807;
      _max = 9223372036854775807;
      Value = value;
    }
    public FloatRange(float value)
    {
      _min = -9223372036854775807;
      _max = 9223372036854775807;
      Value = value;
    }
    public FloatRange(FloatRange floatRange)
    {
      this.minimumValue = floatRange.minimumValue;
      this.maximumValue = floatRange.maximumValue;
      this.Value = floatRange.Value;
    }
    public FloatRange(float minimum, float maximum)
    {
      if (maximum < minimum)
      {
        _min = maximum;
        _max = minimum;
      }
      else
      {
        _min = minimum;
        _max = maximum;
      }
      Value = minimum;
    }
    public FloatRange(float minimum, float maximum, float initialValue)
    {
      if (maximum < minimum)
      {
        _min = maximum;
        _max = minimum;
      }
      else
      {
        _min = minimum;
        _max = maximum;
      }
      Value = initialValue;
    }
    public FloatRange(SerializationInfo info, StreamingContext context)
    {
      float minimum = info.GetSingle("MinimumValue");
      float maximum = info.GetSingle("MaximumValue");
      if (maximum < minimum)
      {
        _min = maximum;
        _max = minimum;
      }
      else
      {
        _min = minimum;
        _max = maximum;
      }
      Value = info.GetSingle("CurrentValue");
    }
    #endregion

    #region ToString Conversions
    public override string ToString()
    {
      return _currValue.ToString();
    }
    public string ToLongString()
    {
      return "Min: " + minimumValue + ", Max: " + maximumValue + ", Current: " + _currValue;
    }
    #endregion

    /// <summary>
    /// Check if a value is within range
    /// </summary>
    /// <param name="value">The float to check</param>
    /// <returns>True if between min and max inclusive.</returns>
    public bool Contains(float value)
    {
      if (value >= minimumValue && value <= maximumValue)
        return true;
      else
        return false;
    }
    public bool Contains(Int16 value)
    {
      if (value >= minimumValue && value <= maximumValue)
        return true;
      else
        return false;
    }
    public bool Contains(Int32 value)
    {
      if (value >= minimumValue && value <= maximumValue)
        return true;
      else
        return false;
    }
    public bool Contains(Int64 value)
    {
      if (value >= minimumValue && value <= maximumValue)
        return true;
      else
        return false;
    }

    // Needed for Serialization
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("MinimumValue", minimumValue);
      info.AddValue("MaximumValue", maximumValue);
      info.AddValue("CurrentValue", Value);
    }

    #region Operator Overloads
    // +
    public static FloatRange operator +(FloatRange a1, FloatRange a2)
    {
      FloatRange temp = new FloatRange(a1);
      temp.Value += a2.Value;
      return temp;
    }
    public static FloatRange operator +(FloatRange a1, float a2)
    {
      FloatRange temp = new FloatRange(a1);
      temp.Value += a2;
      return temp;
    }
    public static FloatRange operator +(float a1, FloatRange a2)
    {
      FloatRange temp = new FloatRange(a2);
      temp.Value += a1;
      return temp;
    }
    public static FloatRange operator +(FloatRange a1, int a2)
    {
      FloatRange temp = new FloatRange(a1);
      temp.Value += a2;
      return temp;
    }
    public static FloatRange operator +(int a1, FloatRange a2)
    {
      FloatRange temp = new FloatRange(a2);
      temp.Value += a1;
      return temp;
    }
    // -
    public static FloatRange operator -(FloatRange a1, FloatRange a2)
    {
      FloatRange temp = new FloatRange(a1);
      temp.Value -= a2.Value;
      return temp;
    }
    public static FloatRange operator -(FloatRange a1, float a2)
    {
      FloatRange temp = new FloatRange(a1);
      temp.Value -= a2;
      return temp;
    }
    public static FloatRange operator -(float a1, FloatRange a2)
    {
      FloatRange temp = new FloatRange(a2);
      temp.Value -= a1;
      return temp;
    }
    public static FloatRange operator -(FloatRange a1, int a2)
    {
      FloatRange temp = new FloatRange(a1);
      temp.Value -= a2;
      return temp;
    }
    public static FloatRange operator -(int a1, FloatRange a2)
    {
      FloatRange temp = new FloatRange(a2);
      temp.Value -= a1;
      return temp;
    }
    // ++
    public static FloatRange operator ++(FloatRange floatRange)
    {
      floatRange.Value += 1;
      return floatRange;
    }
    // --
    public static FloatRange operator --(FloatRange floatRange)
    {
      floatRange.Value -= 1;
      return floatRange;
    }
    // <
    public static bool operator <(FloatRange value1, FloatRange value2)
    {
      return value1.Value < value2.Value;
    }
    public static bool operator <(FloatRange value1, float value2)
    {
      return value1.Value < value2;
    }
    public static bool operator <(float value1, FloatRange value2)
    {
      return value1 < value2.Value;
    }
    public static bool operator <(FloatRange value1, Int16 value2)
    {
      return value1.Value < value2;
    }
    public static bool operator <(Int16 value1, FloatRange value2)
    {
      return value1 < value2.Value;
    }
    public static bool operator <(FloatRange value1, Int32 value2)
    {
      return value1.Value < value2;
    }
    public static bool operator <(Int32 value1, FloatRange value2)
    {
      return value1 < value2.Value;
    }
    public static bool operator <(FloatRange value1, Int64 value2)
    {
      return value1.Value < value2;
    }
    public static bool operator <(Int64 value1, FloatRange value2)
    {
      return value1 < value2.Value;
    }
    // >
    public static bool operator >(FloatRange value1, FloatRange value2)
    {
      return value1.Value > value2.Value;
    }
    public static bool operator >(FloatRange value1, float value2)
    {
      return value1.Value > value2;
    }
    public static bool operator >(float value1, FloatRange value2)
    {
      return value1 > value2.Value;
    }
    public static bool operator >(FloatRange value1, Int16 value2)
    {
      return value1.Value > value2;
    }
    public static bool operator >(Int16 value1, FloatRange value2)
    {
      return value1 > value2.Value;
    }
    public static bool operator >(FloatRange value1, Int32 value2)
    {
      return value1.Value > value2;
    }
    public static bool operator >(Int32 value1, FloatRange value2)
    {
      return value1 > value2.Value;
    }
    public static bool operator >(FloatRange value1, Int64 value2)
    {
      return value1.Value > value2;
    }
    public static bool operator >(Int64 value1, FloatRange value2)
    {
      return value1 > value2.Value;
    }
    // >=
    public static bool operator >=(FloatRange value1, FloatRange value2)
    {
      return value1.Value >= value2.Value;
    }
    public static bool operator >=(FloatRange value1, float value2)
    {
      return value1.Value >= value2;
    }
    public static bool operator >=(float value1, FloatRange value2)
    {
      return value1 >= value2.Value;
    }
    public static bool operator >=(FloatRange value1, Int16 value2)
    {
      return value1.Value >= value2;
    }
    public static bool operator >=(Int16 value1, FloatRange value2)
    {
      return value1 >= value2.Value;
    }
    public static bool operator >=(FloatRange value1, Int32 value2)
    {
      return value1.Value >= value2;
    }
    public static bool operator >=(Int32 value1, FloatRange value2)
    {
      return value1 >= value2.Value;
    }
    public static bool operator >=(FloatRange value1, Int64 value2)
    {
      return value1.Value >= value2;
    }
    public static bool operator >=(Int64 value1, FloatRange value2)
    {
      return value1 >= value2.Value;
    }
    // <=
    public static bool operator <=(FloatRange value1, FloatRange value2)
    {
      return value1.Value <= value2.Value;
    }
    public static bool operator <=(FloatRange value1, float value2)
    {
      return value1.Value <= value2;
    }
    public static bool operator <=(float value1, FloatRange value2)
    {
      return value1 <= value2.Value;
    }
    public static bool operator <=(FloatRange value1, Int16 value2)
    {
      return value1.Value <= value2;
    }
    public static bool operator <=(Int16 value1, FloatRange value2)
    {
      return value1 <= value2.Value;
    }
    public static bool operator <=(FloatRange value1, Int32 value2)
    {
      return value1.Value <= value2;
    }
    public static bool operator <=(Int32 value1, FloatRange value2)
    {
      return value1 <= value2.Value;
    }
    public static bool operator <=(FloatRange value1, Int64 value2)
    {
      return value1.Value <= value2;
    }
    public static bool operator <=(Int64 value1, FloatRange value2)
    {
      return value1 <= value2.Value;
    }
    // ==
    public static bool operator ==(FloatRange value1, FloatRange value2)
    {
      return value1.Value == value2.Value;
    }
    public static bool operator ==(FloatRange value1, float value2)
    {
      return value1.Value == value2;
    }
    public static bool operator ==(float value1, FloatRange value2)
    {
      return value1 == value2.Value;
    }
    public static bool operator ==(FloatRange value1, Int16 value2)
    {
      return value1.Value == value2;
    }
    public static bool operator ==(Int16 value1, FloatRange value2)
    {
      return value1 == value2.Value;
    }
    public static bool operator ==(FloatRange value1, Int32 value2)
    {
      return value1.Value == value2;
    }
    public static bool operator ==(Int32 value1, FloatRange value2)
    {
      return value1 == value2.Value;
    }
    public static bool operator ==(FloatRange value1, Int64 value2)
    {
      return value1.Value == value2;
    }
    public static bool operator ==(Int64 value1, FloatRange value2)
    {
      return value1 == value2.Value;
    }
    // !=
    public static bool operator !=(FloatRange value1, FloatRange value2)
    {
      return value1.Value != value2.Value;
    }
    public static bool operator !=(FloatRange value1, float value2)
    {
      return value1.Value != value2;
    }
    public static bool operator !=(float value1, FloatRange value2)
    {
      return value1 != value2.Value;
    }
    public static bool operator !=(FloatRange value1, Int16 value2)
    {
      return value1.Value != value2;
    }
    public static bool operator !=(Int16 value1, FloatRange value2)
    {
      return value1 != value2.Value;
    }
    public static bool operator !=(FloatRange value1, Int32 value2)
    {
      return value1.Value != value2;
    }
    public static bool operator !=(Int32 value1, FloatRange value2)
    {
      return value1 != value2.Value;
    }
    public static bool operator !=(FloatRange value1, Int64 value2)
    {
      return value1.Value != value2;
    }
    public static bool operator !=(Int64 value1, FloatRange value2)
    {
      return value1 != value2.Value;
    }

    public override bool Equals(object obj)
    {
      if (obj == null) return false;
      FloatRange fr = obj as FloatRange;
      if (fr != null)
      {
        return fr.Value == this.Value;
      }
      return false;
    }
    public bool Equals(FloatRange obj)
    {
      if (obj == null) return false;
      return (obj.Value == Value);
    }
    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }
    // conversion operators
    //int
    public static implicit operator FloatRange(Int16 integer)
    {
      FloatRange output = new FloatRange(integer);
      return output;
    }
    public static implicit operator Int16(FloatRange floatRange)
    {
      return (Int16)floatRange.Value;
    }
    public static implicit operator FloatRange(Int32 integer)
    {
      FloatRange output = new FloatRange(integer);
      return output;
    }
    public static implicit operator Int32(FloatRange floatRange)
    {
      return (Int32)floatRange.Value;
    }
    public static implicit operator FloatRange(Int64 integer)
    {
      FloatRange output = new FloatRange(integer);
      return output;
    }
    public static implicit operator Int64(FloatRange floatRange)
    {
      return (Int64)floatRange.Value;
    }
    //float
    public static implicit operator FloatRange(float input)
    {
      FloatRange output = new FloatRange(input);
      return output;
    }
    public static implicit operator float(FloatRange floatRange)
    {
      return floatRange.Value;
    }
    #endregion
  }
}