namespace ItchyOwl.General
{
    /// <summary>
    /// Simplified integer version of Vector2.
    /// </summary>
    [System.Serializable]
    public struct IntPair
    {
        public IntPair(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;

        public bool IsGreater(IntPair pair)
        {
            return x > pair.x && y > pair.y;
        }

        public bool IsGreaterOrEqual(IntPair pair)
        {
            return x >= pair.x && y >= pair.y;
        }

        public bool IsLess(IntPair pair)
        {
            return x < pair.x && y < pair.y;
        }

        public bool IsLessOrEqual(IntPair pair)
        {
            return x <= pair.x && y <= pair.y;
        }

        public bool IsEqual(IntPair pair)
        {
            return x == pair.x && y == pair.y;
        }

        public IntPair Add(IntPair pair)
        {
            return new IntPair(x + pair.x, y + pair.y);
        }

        public IntPair Reduce(IntPair pair)
        {
            return new IntPair(x - pair.x, y - pair.y);
        }

        public static IntPair Up { get { return new IntPair(0, 1); } }
        public static IntPair Down { get { return new IntPair(0, -1); } }
        public static IntPair Left { get { return new IntPair(-1, 0); } }
        public static IntPair Right { get { return new IntPair(1, 0); } }
        public static IntPair Zero { get { return new IntPair(0, 0); } }
        public static IntPair One { get { return new IntPair(1, 1); } }
    }

}