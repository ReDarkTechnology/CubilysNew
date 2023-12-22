using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cubilys.UI
{
    [ExecuteInEditMode]
    public class ColorTheme : MonoBehaviour
    {
        private Color prevColor = Color.white;
        public Color color = Color.white;
        public Image[] images;
        public RawImage[] rawImages;
        public Text[] texts;

        private void Update()
        {
            if(prevColor != color)
            {
                foreach (var img in images) img.color = color;
                foreach (var img in rawImages) img.color = color;
                foreach (var text in texts) text.color = color;
                prevColor = color;
            }
        }
    }
}