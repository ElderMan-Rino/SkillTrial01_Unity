using Unity.Entities;
using UnityEngine;

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
                    hash ^= text[i];
                    hash *= FNV_PRIME;
                }

                return hash;
            }
        }

        public static unsafe int ToStableHash(ref BlobString blobString)
        {
            fixed (BlobString* blobPtr = &blobString)
            {
                int* layout = (int*)blobPtr;
                int offsetPtr = layout[0]; // m_OffsetPtr
                int length = layout[1]; // m_Length (null 포함)

                // null terminator 제외
                int byteLength = Mathf.Max(0, length - 1);
                if (byteLength == 0) return 0;

                // 실제 데이터 주소 = &m_OffsetPtr + m_OffsetPtr
                byte* dataPtr = (byte*)layout + offsetPtr;

                unchecked
                {
                    int hash = FNV_OFFSET_BASIS;
                    for (int i = 0; i < byteLength; i++)
                    {
                        hash ^= dataPtr[i];
                        hash *= FNV_PRIME;
                    }
                    return hash;
                }
            }
        }
    }
}