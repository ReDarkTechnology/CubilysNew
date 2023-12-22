using UnityEngine;
using System.Collections.Generic;

namespace Cubilys.UI
{
    public class UIAnimationGroup : MonoBehaviour
    {
        public bool isOpened = true;
        public List<IndicatorHost> hosts = new List<IndicatorHost>();
        public void Toggle()
        {
            if (isOpened) CloseAll(); else OpenAll();
            isOpened = !isOpened;
        }

        public void OpenAll()
        {
            foreach (var host in hosts)
            {
                host.Open();
            }
        }

        public void CloseAll()
        {
            foreach(var host in hosts)
            {
                host.Close();
            }
        }
    }
}