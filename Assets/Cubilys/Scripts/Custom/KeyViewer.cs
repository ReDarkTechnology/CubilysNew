using UnityEngine;
using TMPro;
using System;

namespace Cubilys
{
    public class KeyViewer : MonoBehaviour
    {
        public float speed = 1.0f;
        public RendererState[] states = { };
        public int index;

        void Update()
        {
            foreach(var state in states)
                state.renderer.color = Color.Lerp(state.renderer.color, new Color(1f, 1f, 1f, 0f), Time.deltaTime * speed);
        }

        public void OnKeyPress()
        {
            var state = states[index];
            state.count++;
            state.renderer.color = Color.white;
            state.keyCount.text = state.count.ToString();

            index++;
            if (index > states.Length - 1) index = 0;
        }
    }

    [Serializable]
    public class RendererState
    {
        public int count;
        public SpriteRenderer renderer;
        public TextMeshPro keyCount;
    }
}