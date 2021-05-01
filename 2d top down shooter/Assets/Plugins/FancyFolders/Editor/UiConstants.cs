using UnityEngine;

namespace FancyFolders.Editor
{
    /// <summary>
    /// Various important constants extracted from the Unity UI, these may change per Unity version
    /// </summary>
    internal static class UiConstants
    {
        public static class RightColumn
        {
            public static class Indentation
            {
                /// <summary>
                /// Indentation of a basic item in the right column
                /// </summary>
                public const int BasicItem = 13;

                /// <summary>
                /// Indentation of a sub item in the right column
                /// </summary>
                public const int SubItem = 28;
            }

            public static class Icon
            {
                public const int Width = 20;

                public static readonly Vector2 Offset = new Vector2(1, 0);

                public const float ExtraHeight = 0;
            }

            /// <summary>
            /// How much to offset text from the base box position
            /// </summary>
            public static readonly Vector2 TextOffset = new Vector2(0, 1f);
        }

        public static class LeftColumn
        {
            /// <summary>
            /// Indentation in left column is `Base + x * Step` where x is an integer number of indentation steps
            /// </summary>
            public static class Indentation
            {
                /// <summary>
                /// Base indentation of all items
                /// </summary>
                public const int Base = 16;

                /// <summary>
                /// How much to add to base per level of indentation
                /// </summary>
                public const int Step = 14;
            }

            public static class Icon
            {
                public const int Width = 16;

                public static readonly Vector2 Offset = new Vector2(0, -1);

                public const float ExtraHeight = 1;
            }

            /// <summary>
            /// How much to offset text from the base box position
            /// </summary>
            public static readonly Vector2 TextOffset = new Vector2(0, 0.5f);
        }
    }
}
