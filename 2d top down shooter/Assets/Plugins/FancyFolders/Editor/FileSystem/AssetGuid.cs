using System;
using UnityEngine;

namespace FancyFolders.Editor.FileSystem
{
    [Serializable]
    public struct AssetGuid
        : IEquatable<AssetGuid>
    {
        [SerializeField, UsedImplicitly] private string _guid;
        public string Guid => _guid;

        public AssetGuid([NotNull] string guid)
        {
            _guid = guid ?? throw new ArgumentNullException(nameof(guid));
        }

        public override string ToString()
        {
            return Guid;
        }

        public bool Equals(AssetGuid other)
        {
            return string.Equals(_guid, other._guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is AssetGuid other && Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode (justification: this field is only non-readonly for serialization purposes)
            return (_guid != null ? _guid.GetHashCode() : 0);
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        public static bool operator ==(AssetGuid left, AssetGuid right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetGuid left, AssetGuid right)
        {
            return !left.Equals(right);
        }
    }
}
