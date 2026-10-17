using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Elder.Framework.Core.Helpers
{
    public sealed class UniTaskThrottleHelper
    {
        private readonly SemaphoreSlim _semaphore;

        public UniTaskThrottleHelper(int maxConcurrency)
        {
            _semaphore = new SemaphoreSlim(maxConcurrency); // [HEAP] 생성 1회
        }

        public async UniTask RunAsync(int count, Func<int, UniTask> taskFactory)
        {
            var tasks = new UniTask[count]; // [HEAP] 호출 1회당 배열 1개

            for (var i = 0; i < count; i++)
                tasks[i] = RunOneAsync(i, taskFactory);

            await UniTask.WhenAll(tasks);
        }

        private async UniTask RunOneAsync(int index, Func<int, UniTask> taskFactory)
        {
            await _semaphore.WaitAsync();
            try
            {
                await taskFactory(index);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}