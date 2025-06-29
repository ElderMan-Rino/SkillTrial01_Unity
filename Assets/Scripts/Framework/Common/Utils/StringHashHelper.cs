using Unity.Entities;

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
                int* layout    = (int*)blobPtr;
                int  offsetPtr = layout[0]; // BlobArray<byte>.m_OffsetPtr
                int  byteCount = layout[1]; // BlobArray<byte>.m_Length (null terminator 포함)

                int length = byteCount - 1;
                if (length <= 0) return 0;

                // 실제 데이터 주소: m_OffsetPtr 필드의 주소 기준 + offset
                // BlobString 내부는 UTF-8 byte 배열 — byte* 로 읽어야 string 오버로드(UTF-16 char)와 ASCII 범위에서 동일 값
                byte* dataPtr = (byte*)((byte*)layout + offsetPtr);

                unchecked
                {
                    int hash = FNV_OFFSET_BASIS;
                    for (int i = 0; i < length; i++)
                    {
                        hash ^= dataPtr[i]; // byte 단위 — ASCII 범위에서 char 오버로드와 동일
                        hash *= FNV_PRIME;
                    }
                    return hash;
                }
            }
        }
    }
}