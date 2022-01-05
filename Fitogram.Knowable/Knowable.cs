using System;
using System.ComponentModel;

namespace Fitogram.Knowable
{
    public interface IKnowable
    {
        bool IsKnown { get; }
        object Value { get; set; }
    }

    public struct Knowable<T> : IKnowable where T : System.Enum
    {
        private object _innerValue;

        /// <summary>
        /// The underlying value of the enum, likely to be a string or an int depending on if the string enum converter is also being used.
        /// </summary>
        public object InnerValue => _innerValue;

        public bool IsKnown => _innerValue != null && Enum.IsDefined(typeof(T), _innerValue);

        /// <exception cref="InvalidEnumArgumentException">May throw this an exception if the underlying InnerValue is an unexpected type or cannot be converted to the enum.</exception>
        public T Value
        {
            get
            {
                if (!IsKnown) throw new InvalidEnumArgumentException("Cannot convert inner value to known enum. Check that the enum IsKnown before trying to get the value.");

                return _innerValue switch
                {
                    int => (T) Enum.ToObject(typeof(T), _innerValue),
                    string => (T) Enum.Parse(typeof(T), _innerValue.ToString()),
                    _ => throw new InvalidEnumArgumentException("Type of enum not known.")
                };
            }
            set => _innerValue = value.ToString();
        }

        object IKnowable.Value
        {
            get => Value;
            set => _innerValue = value;
        }

        public static implicit operator Knowable<T>(string value)
        {
            return new Knowable<T> { _innerValue = value, };
        }

        public static implicit operator Knowable<T>(int value)
        {
            return new Knowable<T> { _innerValue = value, };
        }

        public static implicit operator Knowable<T>(T value)
        {
            return new Knowable<T> { Value = value, };
        }

        public static explicit operator T(Knowable<T> optional)
        {
            return optional.Value;
        }
    }
}
