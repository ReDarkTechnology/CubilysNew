using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace Cubilys.Monetization
{
    public class SkinStoreItem : MonoBehaviour
    {
        public GemSystem gemCore;
        public SkinStoreManager skinManager;
        public int skinIndex;
        public int requiredGems = 550;

        public bool isDefault = false;
        public bool isBought;

        public Text buttonText;

        private void Start()
        {
            isBought = bool.Parse(PlayerPrefs.GetString("IsSkinBought - " + skinIndex, "False")) || isDefault;
            CheckSkin();
        }

        public void CheckSkin()
        {
            if (!isDefault)
            {
                if (isBought)
                {
                    CheckAppliedSkin();
                }
                else
                {
                    buttonText.text = "Buy: " + requiredGems.ToString();
                }
            }
            else
            {
                CheckAppliedSkin();
            }
        }

        public void CheckAppliedSkin()
        {
            var c = PlayerPrefs.GetInt("CurrentSkin", 0);
            buttonText.text = c == skinIndex ? "Equipped" : "Equip";
        }

        public void TryApplyingSkin()
        {
            if (isBought)
            {
                ApplySkin();
            }
            else
            {
                if (gemCore.TryToBuy(requiredGems))
                {
                    PlayerPrefs.SetString("IsSkinBought - " + skinIndex, "True");
                    isBought = true;
                    ApplySkin();
                    CheckAppliedSkin();
                }
            }
        }

        public void ApplySkin()
        {
            skinManager.TryApplySkin(skinIndex);
        }
    }
}
