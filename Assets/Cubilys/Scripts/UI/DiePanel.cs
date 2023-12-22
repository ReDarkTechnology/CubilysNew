using UnityEngine;
using UnityEngine.UI;

using Cubilys.Easings;
using TMPro;

namespace Cubilys.UI
{
    public class DiePanel : MonoBehaviour
    {
        [Header("Main Interface")]
        public UIAnimationGroup animationGroup;
        public CanvasGroup mainPanel;
        public TMP_Text levelTitle;
        public TMP_Text levelMusic;
        public Slider musicProgress;
        public TMP_Text musicPercentage;

        public void OnDie()
        {
            gameObject.SetActive(true);
            var p = MenuManager.player.source.time / MenuManager.player.source.clip.length;
            TweenTool.TweenFloat(0, p, 2).SetEase(TweenType.OutCubic).SetOnUpdate(val => {
                float e = (float)val;
                musicProgress.value = e;
                musicPercentage.text = (e * 100).ToString("0") + "%";
            });
            animationGroup.OpenAll();
            FadeTo(1);
        }

        public void OnFinish()
        {
            gameObject.SetActive(true);
            TweenTool.TweenFloat(0, 1, 2).SetEase(TweenType.OutCubic).SetOnUpdate(val => {
                float e = (float)val;
                musicProgress.value = e;
                musicPercentage.text = (e * 100).ToString("0") + "%";
            });
            animationGroup.OpenAll();
            FadeTo(1);
        }

        public void FadeTo(float to)
        {
            // TweenTool.TweenFloat(mainPanel.alpha, to, 0.25f).SetEase(TweenType.Linear).SetOnUpdate(val => mainPanel.alpha = (float)val);
        }

        public void FadeOutAndExit()
        {
            if (MenuManager.manager.isRestarting) return;
            animationGroup.CloseAll();
            // TweenTool.TweenFloat(mainPanel.alpha, 0, 0.25f).SetEase(TweenType.InCubic).SetOnUpdate(val => mainPanel.alpha = (float)val);
            Invoke("DisableNow", 0.25f);
        }

        public void DisableNow()
        {
            gameObject.SetActive(false);
        }
    }
}
