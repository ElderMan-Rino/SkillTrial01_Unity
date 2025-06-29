using UnityEngine;

namespace Elder.Framework.Data.Interfaces
{
    public interface IDataDeserializer 
    {
        T Deserialize<T>(byte[] data);
    }
}