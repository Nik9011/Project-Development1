using Invector;
using UnityEngine;
using UnityEngine.UI;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("GrapplingRope UI", iconName = "misIconRed")]
    public class mvGrapplingRopeUI : vMonoBehaviour
    {
        [SerializeField] GameObject background;
        [SerializeField] Text amountText;
        bool isActivated;

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Awake()
        {
            isActivated = background.activeSelf;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void SetActive(bool active)
        {
            if (isActivated != active)
            {
                isActivated = active;
                background.SetActive(active);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void UpdateAmount(int currentAmount, int maxAmount)
        {
            amountText.text = currentAmount.ToString() + "/" + maxAmount.ToString();
        }
    }
}