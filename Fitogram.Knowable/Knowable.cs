using System;

namespace Fitogram.Knowable
{
    public interface IKnowable
    {
        bool IsKnown { get; }
        object Value { get; set; }
    }

    public struct Knowable<T> : IKnowable where T : System.Enum
    {
        // private string _value;
        private object _value;

        public bool IsKnown => Enum.IsDefined(typeof(T), _value);

        public T Value
        {
            // get => this.IsKnown
            //     ? (T)Enum.Parse(typeof(T), _value)
            //     : throw new Exception("Enum not known.");
            get
            {
                if (!IsKnown) throw new Exception("Enum not known.");

                return _value switch
                {
                    int => (T) Enum.ToObject(typeof(T), _value),
                    string => (T) Enum.Parse(typeof(T), _value.ToString()),
                    _ => throw new Exception("Type of enum not known.")
                };

                // switch (_value)
                // {
                //     case int value:
                //         return (T)Enum.ToObject(typeof(T) , value);
                //         break;
                //     case string:
                //         return (T) Enum.Parse(typeof(T), _value.ToString());
                //         break;
                // }
            }
            set => _value = value.ToString();
        }

        object IKnowable.Value
        {
            get => Value;
            set => _value = value;
        }

        public static implicit operator Knowable<T>(string value)
        {
            return new Knowable<T> { _value = value, };
        }

        public static implicit operator Knowable<T>(int value)
        {
            return new Knowable<T> { _value = value, };
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