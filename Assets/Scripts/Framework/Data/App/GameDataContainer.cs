using Elder.Framework.Data.Interfaces;
using MessagePack;
using System;
using System.Collections.Generic;

namespace Elder.Framework.Data.App
{
    // MessagePack 직렬화를 위한 어트리뷰트
    [MessagePackObject(AllowPrivate = true)]
    public class GameDataContainer
    {
        // 예시: 몬스터 데이터, 아이템 데이터 등을 담을 Dictionary (Key는 Type으로 관리)
        [IgnoreMember]
        private readonly Dictionary<Type, object> _dataTables = new();

        // 파싱이 완료된 후 DataProvider에서 호출하여 내부 Dictionary를 세팅하는 초기화 함수
        public void Initialize()
        {
            // TODO: 실제 데이터 클래스들이 생기면 여기에 매핑 로직을 추가합니다.
            // 예: _dataTables[typeof(MonsterData)] = MonsterDataList;
        }

        public T GetData<T>(int id) where T : class, IDataRecord
        {
            if (_dataTables.TryGetValue(typeof(T), out var table))
            {
                var typedTable = table as Dictionary<int, T>;
                if (typedTable != null && typedTable.TryGetValue(id, out var data))
                    return data;
            }
            return null;
        }

        public IReadOnlyList<T> GetAllData<T>() where T : class, IDataRecord
        {
            if (_dataTables.TryGetValue(typeof(T), out var table))
            {
                var typedTable = table as Dictionary<int, T>;
                return new List<T>(typedTable.Values);
            }
            return Array.Empty<T>();
        }
    }
}