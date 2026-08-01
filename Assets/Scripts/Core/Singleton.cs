using UnityEngine;

namespace ArcadiaOnline.Core
{
    /// <summary>
    /// Base singleton class untuk semua Manager di Arcadia Online.
    /// Lihat docs/02_TDD/ScriptArchitecture.md - Manager Pattern.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                }
                return _instance;
            }
        }

        [SerializeField] private bool _dontDestroyOnLoad = true;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
