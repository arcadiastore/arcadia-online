using System.Collections.Generic;
using UnityEngine;

namespace ArcadiaOnline.Core
{
    /// <summary>
    /// Generic object pool untuk projectile, VFX, dan monster respawn.
    /// Lihat docs/02_TDD/Architecture.md - Object Pool.
    /// </summary>
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly T _prefab;
        private readonly Transform _parent;

        public ObjectPool(T prefab, int prewarmCount = 0, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < prewarmCount; i++)
            {
                T obj = Object.Instantiate(_prefab, _parent);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}
