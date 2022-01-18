using System;
using System.ComponentModel;

namespace Fitogram.Knowable
{
    public interface IKnowable
    {
        public object InnerValue { get; }
        bool IsKnown { get; }
        object Value { get; set; }
    }

    public struct Knowable<T> : IKnowable where T : Enum
    {
        private object _innerValue;

        /// <summary>
        /// The underlying value of the enum, likely to be a string or an int depending on if the string enum converter is also being used.
        /// </summary>
        public object InnerValue
        {
            get => _innerValue ?? 0;
            private set => _innerValue = value;
        }

        public bool IsKnown => Enum.IsDefined(typeof(T), InnerValue);

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

        object IKnowable.Value
        {
            get => Value;
            set => InnerValue = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Knowable<T>)
            {
                Knowable<T>? knowable2 = obj as Knowable<T>?;

                return this.Value.Equals(knowable2.Value.Value);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return InnerValue.GetHashCode();
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
