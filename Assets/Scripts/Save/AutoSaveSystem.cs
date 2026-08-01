using UnityEngine;

namespace ArcadiaOnline.Save
{
    /// <summary>Lihat docs/02_TDD/SaveArchitecture.md - Auto Save (default 5 menit).</summary>
    public class AutoSaveSystem : MonoBehaviour
    {
        [SerializeField] private float _autoSaveInterval = 300f;
        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _autoSaveInterval)
            {
                SaveManager.Instance?.AutoSave();
                _timer = 0f;
            }
        }
    }
}
