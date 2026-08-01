using UnityEngine;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.Managers
{
    /// <summary>Lihat docs/02_TDD/GameManagers.md dan docs/01_GDD/26_UI.md.</summary>
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private GameObject _hud;
        [SerializeField] private GameObject _menu;
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private GameObject _notificationPrefab;
        [SerializeField] private Transform _notificationContainer;

        public void ShowHUD() => _hud?.SetActive(true);
        public void HideHUD() => _hud?.SetActive(false);
        public void ShowMenu() => _menu?.SetActive(true);
        public void HideMenu() => _menu?.SetActive(false);

        public void ShowDialogue(DialogueData data)
        {
            _dialoguePanel?.SetActive(true);
            // TODO: isi UI dialog (nama, teks, pilihan) dari `data`.
            // Lihat docs/01_GDD/15_Dialogue.md.
        }

        public void HideDialogue() => _dialoguePanel?.SetActive(false);

        public void ShowNotification(string message)
        {
            if (_notificationPrefab == null || _notificationContainer == null)
            {
                Debug.Log($"[Notification] {message}");
                return;
            }
            var go = Instantiate(_notificationPrefab, _notificationContainer);
            // TODO: set teks notifikasi via komponen TMP_Text pada prefab.
        }
    }

    [System.Serializable]
    public class DialogueData
    {
        public string speakerName;
        [TextArea] public string text;
        public string[] choices;
    }
}
