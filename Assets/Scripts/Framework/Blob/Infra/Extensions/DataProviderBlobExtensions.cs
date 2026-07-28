using Elder.Framework.Blob.Interfaces;
using Unity.Entities;

namespace Elder.Framework.Blob.Infra.Extensions
{
    public static class DataProviderBlobExtensions
    {
        public static bool TryGetBlobData<T>(this IDataProvider provider, out T data) where T : unmanaged
        {
            if (!provider.TryGetBlobReference<T>(out var reference))
            {
                data = default;
                return false;
            }

            data = reference.Value;
            return true;
        }
    }
}
