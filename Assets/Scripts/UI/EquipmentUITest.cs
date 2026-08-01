using UnityEngine;

namespace ArcadiaOnline.Equipment
{
    /// <summary>
    /// Test EquipmentUI - Tekan T untuk test buka/Close.
    /// </summary>
    public class EquipmentUITest : MonoBehaviour
    {
        void Update()
        {
            // Tekan T untuk test buka/close
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (EquipmentUI.Instance != null)
                {
                    EquipmentUI.Instance.ToggleEquipment();
                    Debug.Log("[Test] Toggle Equipment");
                }
                else
                {
                    Debug.LogError("[Test] EquipmentUI.Instance null!");
                }
            }

            // Tekan O untuk force open
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (EquipmentUI.Instance != null)
                {
                    EquipmentUI.Instance.OpenEquipment();
                    Debug.Log("[Test] Open Equipment");
                }
            }

            // Tekan P untuk force close
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (EquipmentUI.Instance != null)
                {
                    EquipmentUI.Instance.CloseEquipment();
                    Debug.Log("[Test] Close Equipment");
                }
            }
        }
    }
}
