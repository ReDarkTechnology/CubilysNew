using UnityEngine;
using System.Collections;

namespace Cubilys.Monetization
{
    public class SkinStoreManager : MonoBehaviour
    {
        public GemSystem gemSystem;

        public SkinStoreItem[] skins;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            var c = PlayerPrefs.GetInt("CurrentSkin", 0);
            TryApplySkin(c);
        }

        public void TryApplySkin(int index)
        {
            MenuManager.skinAddon.currentSkin = index;
            MenuManager.skinAddon.ApplySkin(index);
            PlayerPrefs.SetInt("CurrentSkin", index);
            foreach (var sk in skins)
            {
                sk.CheckSkin();
            }
        }
    }
}