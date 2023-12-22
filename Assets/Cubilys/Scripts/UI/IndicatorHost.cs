using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Cubilys.Easings;

namespace Cubilys.UI
{
    public class IndicatorHost : MonoBehaviour
    {
        public Transform tr;
        public Transform close;
        public Transform open;
        public float time = 0.5f;
        public bool isOpened;

        public UnityEvent onClosed;
        public UnityEvent onOpened;
        Tweenable cur;
        public void Close()
        {
            if (cur != null) cur.finished = true;
            onClosed.Invoke();
            cur = TweenTool.TweenVector3(tr.transform.position, close.transform.position, time).SetEase(TweenType.OutCubic).SetOnUpdate(
                val => tr.transform.position = (Vector3)val);
        }

        public void Open()
        {
            if (cur != null) cur.finished = true;
            onOpened.Invoke();
            cur = TweenTool.TweenVector3(tr.transform.position, open.transform.position, time).SetEase(TweenType.OutCubic).SetOnUpdate(
                val => tr.transform.position = (Vector3)val);
        }
    }
}