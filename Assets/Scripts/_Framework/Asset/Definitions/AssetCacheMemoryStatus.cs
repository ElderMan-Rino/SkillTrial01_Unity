namespace Elder.Framework.Asset.Definitions
{
    public readonly struct AssetCacheMemoryStatus
    {
        public int CachedKeyCount { get; }
        public int RegisteredLabelCount { get; }
        public long TotalAllocatedBytes { get; }

        public AssetCacheMemoryStatus(int cachedKeyCount, int registeredLabelCount, long totalAllocatedBytes)
        {
            CachedKeyCount = cachedKeyCount;
            RegisteredLabelCount = registeredLabelCount;
            TotalAllocatedBytes = totalAllocatedBytes;
        }
    }
}
