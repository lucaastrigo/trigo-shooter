using System;
using UnityEngine;

namespace FancyFolders.Editor.Serialization
{
    [Serializable]
    internal struct SerializableNullableColor
    {
        [SerializeField, UsedImplicitly] private Color _value;
        [SerializeField, UsedImplicitly] private bool _hasValue;

        public Color? Value
        {
            get
            {
                if (_hasValue)
                    return _value;
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _value = value.Value;
                    _hasValue = true;
                }
                else
                {
                    _value = default(Color);
                    _hasValue = false;
                }
            }
        }

        public bool HasValue
        {
            get { return _hasValue; }
        }

        public static implicit operator Color?(SerializableNullableColor sn)
        {
            return sn.Value;
        }

        public static implicit operator SerializableNullableColor(Color? nt)
        {
            return new SerializableNullableColor { Value = nt };
        }
    }
}
