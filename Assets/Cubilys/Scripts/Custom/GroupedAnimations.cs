using UnityEngine;
using System.Collections.Generic;
using Cubilys.Easings;

namespace Cubilys.Custom
{
    public class GroupedAnimations : MonoBehaviour
    {
        public Transform parent;
        public List<Transform> childs = new List<Transform>();
        [AllowSavingState]
        public int currentIndex;

        [Header("Tween Variables")]
        public Vector3 offset = new Vector3(0, 5, 0);
        public float time = 0.5f;
        public TweenType type = TweenType.OutCubic;

        void Start()
        {
            if (childs.Count < 1)
            {
                var ch = parent.GetComponentsInChildren<Transform>();
                foreach (var aa in ch)
                {
                    if (aa != parent) childs.Add(aa);
                }
            }
        }

        public void CallTween()
        {
            if(currentIndex < childs.Count)
            {
                var c = childs[currentIndex];
                TweenTool.TweenVector3(c.transform.position, c.transform.position + offset, time).SetEase(type).SetOnUpdate(
                    val => c.transform.position = (Vector3)val);
                currentIndex++;
            }
        }
    }
}
