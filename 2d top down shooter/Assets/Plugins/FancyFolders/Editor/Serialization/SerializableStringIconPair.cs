using System;
using FancyFolders.Editor.Settings.Icons;
using UnityEngine;

namespace FancyFolders.Editor.Serialization
{
    [Serializable]
    internal struct SerializableStringIconPair
        : IComparable<SerializableStringIconPair>
    {
        [SerializeField, UsedImplicitly] public string Name;
        [SerializeField, UsedImplicitly] public Icon Icon;

        public SerializableStringIconPair(string name, Icon icon)
        {
            Name = name;
            Icon = icon;
        }

        public int CompareTo(SerializableStringIconPair other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}
