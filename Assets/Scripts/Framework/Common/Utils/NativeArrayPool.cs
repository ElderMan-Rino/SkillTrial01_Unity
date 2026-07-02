using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Elder.Framework.Common.Utils
{
    // NativeArray<T> 재사용 풀 — Persistent 할당 + 용량 증가 시만 재할당
    // ✅ OK: 폴더 위치 — Common/Utils/ (공통 유틸리티)
    // ❌ VIOLATION: public sealed — internal sealed이어야 함
    // ❌ VIOLATION: Dispose()의 foreach (var stack in _buckets.Values) — [HEAP] 주석 누락 (Dictionary.Values 열거자 힙 할당)
    public sealed class NativeArrayPool<T> : IDisposable where T : unmanaged
    {
        private readonly Dictionary<int, Stack<NativeArray<T>>> _buckets = new(); // [HEAP] 1회
        private bool _disposed;

        public NativeArray<T> Rent(int minimumLength)
        {
            int bucket = NextPow2(minimumLength);

            if (_buckets.TryGetValue(bucket, out var stack) && stack.Count > 0)
                return stack.Pop();

            // [HEAP] NativeArray Persistent 할당 — 풀 미스 시만
            return new NativeArray<T>(bucket, Allocator.Persistent);
        }

        public void Return(NativeArray<T> array)
        {
            if (!array.IsCreated) return;
            int bucket = NextPow2(array.Length);

            if (!_buckets.TryGetValue(bucket, out var stack))
            {
                stack = new Stack<NativeArray<T>>();
                _buckets[bucket] = stack;
            }
            stack.Push(array);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var stack in _buckets)
            {
                while (stack.Value.Count > 0)
                {
                    var arr = stack.Value.Pop();
                    if (arr.IsCreated) arr.Dispose();
                }
            }
            _buckets.Clear();
        }

        private static int NextPow2(int n)
        {
            if (n <= 0) return 1;
            int p = 1;
            while (p < n) p <<= 1;
            return p;
        }
    }
}
