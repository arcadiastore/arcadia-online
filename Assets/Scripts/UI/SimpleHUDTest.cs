using UnityEngine;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// Test SimpleHUD - Tekan H untuk test.
    /// </summary>
    public class SimpleHUDTest : MonoBehaviour
    {
        void Update()
        {
            // Tekan H untuk test
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (SimpleHUD.Instance != null)
                {
                    Debug.Log("[Test] SimpleHUD Instance found!");
                }
                else
                {
                    Debug.LogError("[Test] SimpleHUD Instance null!");
                }
            }

            // Tekan J untuk force create
            if (Input.GetKeyDown(KeyCode.J))
            {
                SimpleHUD hud = FindAnyObjectByType<SimpleHUD>();
                if (hud != null)
                {
                    Debug.Log("[Test] SimpleHUD found, triggering start...");
                    // Force reinitialize
                    hud.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    Debug.LogError("[Test] SimpleHUD not found!");
                }
            }
        }
    }
}
