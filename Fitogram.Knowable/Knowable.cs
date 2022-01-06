using System;
using System.ComponentModel;

namespace Fitogram.Knowable
{
    public interface IKnowable<T> where T : System.Enum
    {
        bool IsKnown { get; }
        T Value { get; set; }
    }

    public struct Knowable<T> : IKnowable<T> where T : System.Enum
    {
        /// <summary>
        /// The underlying value of the enum, likely to be a string or an int depending on if the string enum converter is also being used.
        /// </summary>
        public object InnerValue { get; private set; }

        public bool IsKnown => InnerValue != null && Enum.IsDefined(typeof(T), InnerValue);

        /// <exception cref="InvalidEnumArgumentException">May throw this an exception if the underlying InnerValue is an unexpected type or cannot be converted to the enum.</exception>
        public T Value
        {
            get
            {
                if (!IsKnown) throw new InvalidEnumArgumentException("Cannot convert inner value to known enum. Check that the enum IsKnown before trying to get the value.");

                return InnerValue switch
                {
                    int => (T) Enum.ToObject(typeof(T), InnerValue),
                    string => (T) Enum.Parse(typeof(T), InnerValue.ToString()),
                    _ => throw new InvalidEnumArgumentException("Type of enum not known.")
                };
            }
            set => InnerValue = value.ToString();
        }

        public static implicit operator Knowable<T>(string value)
        {
            return new Knowable<T> { InnerValue = value, };
        }

        public static implicit operator Knowable<T>(int value)
        {
            return new Knowable<T> { InnerValue = value, };
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
