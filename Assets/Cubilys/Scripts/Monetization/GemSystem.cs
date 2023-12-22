using UnityEngine;
using TMPro;

namespace Cubilys.Monetization
{
    public class GemSystem : MonoBehaviour
    {
        public int gems = 550;
        public TMP_Text gemCount;

        Easings.Tweenable tw;

        private void Start()
        {
            gems = PlayerPrefs.GetInt("GemsLeft", 550);
        }

        private void Update()
        {
            gemCount.text = gems.ToString();
        }

        public bool TryToBuy(int requiredGems, bool warnIfInsufficient = true)
        {
            if (gems >= requiredGems)
            {
                AdjustGems(-requiredGems);
                return true;
            }
            if(warnIfInsufficient)
            {
                if (tw != null)
                {
                    Easings.TweenTool.tweenables.Remove(tw);
                    tw.finished = true;
                }
                tw = Easings.TweenTool.TweenColor(Color.red, Color.white, 1f).SetEase(Easings.TweenType.OutCubic).SetOnUpdate(
                    val => gemCount.color = (Color)val);
            }
            return false;
        }

        public void AdjustGems(int increment)
        {
            gems += increment;
            PlayerPrefs.SetInt("GemsLeft", gems);
        }
    }
}
