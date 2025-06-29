namespace Elder.Framework.Common.Utils
{
    public static class StringHashHelper 
    {
        private const int FNV_OFFSET_BASIS = unchecked((int)2166136261);
        private const int FNV_PRIME = 16777619;

        public static int ToStableHash(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            unchecked
            {
                int hash = FNV_OFFSET_BASIS;

                for (int i = 0; i < text.Length; i++)
                {
                    char character = text[i];

                    hash = hash ^ character;
                    hash = hash * FNV_PRIME;
                }

                return hash;
            }
        }
    }
}