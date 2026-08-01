using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// Test script untuk cek MonsterInfoUI.
    /// Tekan T untuk test show, H untuk hide.
    /// </summary>
    public class MonsterInfoUITest : MonoBehaviour
    {
        [Header("Test")]
        [SerializeField] private Slider testSlider;
        [SerializeField] private Text testText;

        void Update()
        {
            // Tekan T untuk test show
            if (Input.GetKeyDown(KeyCode.T))
            {
                TestShow();
            }

            // Tekan H untuk hide
            if (Input.GetKeyDown(KeyCode.H))
            {
                TestHide();
            }

            // Tekan +/- untuk test slider
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (testSlider != null)
                {
                    testSlider.value = Mathf.Min(1, testSlider.value + 0.1f);
                    Debug.Log($"[Test] Slider: {testSlider.value}");
                }
            }

            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (testSlider != null)
                {
                    testSlider.value = Mathf.Max(0, testSlider.value - 0.1f);
                    Debug.Log($"[Test] Slider: {testSlider.value}");
                }
            }
        }

        private void TestShow()
        {
            if (MonsterInfoUI.Instance != null)
            {
                // Buat dummy monster untuk test
                GameObject dummy = new GameObject("TestMonster");
                Monster.SimpleMonsterAI monster = dummy.AddComponent<Monster.SimpleMonsterAI>();
                MonsterInfoUI.Instance.ShowMonsterInfo(monster);
                Debug.Log("[Test] Show MonsterInfo");
            }
            else
            {
                Debug.LogError("[Test] MonsterInfoUI.Instance null!");
            }
        }

        private void TestHide()
        {
            if (MonsterInfoUI.Instance != null)
            {
                MonsterInfoUI.Instance.HideMonsterInfo();
                Debug.Log("[Test] Hide MonsterInfo");
            }
        }
    }
}
