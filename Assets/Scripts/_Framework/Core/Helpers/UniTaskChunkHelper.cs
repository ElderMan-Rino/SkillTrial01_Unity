using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Elder.Framework.Core.Helpers
{
    public static class UniTaskChunkHelper
    {
        public static async UniTask RunInChunksAsync<T>(IReadOnlyList<T> items, int chunkSize, Func<T, CancellationToken, UniTask> action, CancellationToken cancellationToken)
        {
            var chunk = new List<UniTask>(chunkSize);
            for (int i = 0; i < items.Count; i += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                chunk.Clear();
                var end = Mathf.Min(i + chunkSize, items.Count);
                for (int j = i; j < end; ++j)
                    chunk.Add(action(items[j], cancellationToken));

                await UniTask.WhenAll(chunk);
            }
        }
    }
}