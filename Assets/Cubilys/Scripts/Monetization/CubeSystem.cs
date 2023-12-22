using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
// using UnityEngine.Advertisements;

using System;
using System.Collections.Generic;

namespace Cubilys.Monetization
{
    public class CubeSystem : MonoBehaviour
    {
        public static CubeSystem instance;
        public int cubeLeft;
        public float countdown;
        public bool isLoading;

        public TMP_Text leftText;
        public TMP_Text countText;

        public PlayerMovement player;
        Easings.Tweenable tw;

        public GameObject adLoadingPanel;
        public GameStore storeCenter;

        void Start()
        {
            instance = this;
            cubeLeft = PlayerPrefs.GetInt("CubeLeft", 15);
            var last = PlayerPrefs.GetString("LastCountdown", null);
            if(!string.IsNullOrEmpty(last))
            {
                var prev = DateTime.Parse(last);
                var now = DateTime.UtcNow;
                var cubes = (now.Ticks - prev.Ticks) / 300000000f;
                var increment = cubes < 1 ? 0 : Convert.ToInt32(cubes);
                cubeLeft = Mathf.Clamp(cubeLeft + increment, 0, 15);
                PlayerPrefs.SetInt("CubeLeft", cubeLeft);
            }
        }

        void Update()
        {
            if (cubeLeft < 15)
            {
                if (!isLoading)
                {
                    countdown = 30;
                    isLoading = true;
                }
                countdown -= Time.deltaTime;
                countText.text = countdown.ToString("0");
                if (countdown <= 0)
                {
                    isLoading = false;
                    countText.text = "0";
                    cubeLeft = Mathf.Clamp(cubeLeft + 1, 0, 15);
                    countdown = 30;
                    PlayerPrefs.SetInt("CubeLeft", cubeLeft);
                    PlayerPrefs.SetString("LastCountdown", DateTime.UtcNow.ToLongTimeString());
                }
            }
            else
            {
                countText.text = "";
            }
            leftText.text = cubeLeft + "/15";
        }

        public void TryStarting()
        {
            if(cubeLeft > 0)
            {
                cubeLeft--;
                player.preventStarting = false;
            }
            else
            {
                player.preventStarting = true;
                if (tw != null)
                {
                    Easings.TweenTool.tweenables.Remove(tw);
                    tw.finished = true;
                }
                tw = Easings.TweenTool.TweenColor(Color.red, Color.white, 1f).SetEase(Easings.TweenType.OutCubic).SetOnUpdate(
                    val => leftText.color = (Color)val);
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetString("LastCountdown", DateTime.UtcNow.ToLongTimeString());
        }

        /*public void FillWithAd()
        {
            var action = default(Action<ShowResult>);
            action += val =>
            {
                if(val == ShowResult.Failed)
                {
                    Debug.Log("Showing ad failed...");
                }
                else if(val == ShowResult.Skipped)
                {
                    Debug.Log("The ad is skipped, no reward!");
                }
                else if(val == ShowResult.Finished)
                {
                    cubeLeft = 30;
                    PlayerPrefs.SetInt("CubeLeft", cubeLeft);
                }
                adLoadingPanel.SetActive(false);
            };
            var request = new AdRequest("Rewarded_Android", action);
            storeCenter.ShowAd(request);
            adLoadingPanel.SetActive(true);
        }*/
    }
}
