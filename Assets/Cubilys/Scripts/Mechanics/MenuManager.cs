using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cubilys
{
    public class MenuManager : MonoBehaviour
    {
        public LevelManager[] levels;
        public static PlayerMovement player;
        public static LevelManager manager;
        public static SkinAddon skinAddon;

        public GameObject menuPanel;
        public UI.UIAnimationGroup animationGroup;
        public UI.IndicatorHost indicator;
        public UI.DiePanel diePanel;
        public Monetization.SkinStoreManager skinManager;
        public UI.ColorTheme colorTheme;

        public Text levelTitleText;
        public Text levelMusicText;
        public Sprite selectedSprite;
        public Sprite unselectedSprite;

        public Image[] selectorImages;
        int currentIndex;

        void Start()
        {
            ChangeToIndex(0);
        }

        public void ChangeToIndex(int index)
        {
            currentIndex = index;
            for(int i = 0; i < levels.Length; i++)
            {
                var l = levels[i];
                l.gameObject.SetActive(i == index);
                if(i == index)
                {
                    manager = l;
                    InitNow();
                }
                selectorImages[i].sprite = i == index ? selectedSprite : unselectedSprite;
            }
        }

        public void MoveLevelTowards(bool next)
        {
            if(next)
            {
                if(currentIndex + 1 >= levels.Length)
                {
                    ChangeToIndex(0);
                }
                else
                {
                    ChangeToIndex(currentIndex + 1);
                }
            }
            else
            {
                if (currentIndex - 1 < 0)
                {
                    ChangeToIndex(levels.Length - 1);
                }
                else
                {
                    ChangeToIndex(currentIndex - 1);
                }
            }
        }

        public void InitNow()
        {
            player = manager.player;
            skinAddon = player.GetComponent<SkinAddon>();
            RenderSettings.fogColor = manager.fogColor;
            RenderSettings.skybox = manager.skyboxMaterial;
            skinManager.Initialize();
            diePanel.levelMusic.text = manager.info.musicInfo;
            diePanel.levelTitle.text = manager.info.levelName;
            levelTitleText.text = manager.info.levelName;
            levelMusicText.text = manager.info.musicInfo;
            colorTheme.color = manager.uiColor;
        }

        public void Restart()
        {
            manager.Restart();
        }

        public void OpenPanel()
        {
            indicator.Open();
            menuPanel.SetActive(true);
            animationGroup.OpenAll();
        }

        public void ClosePanel()
        {
            indicator.Close();
            animationGroup.CloseAll();
            CallDelay.Call(0.5f, () => menuPanel.SetActive(false));
        }

        public void OpenScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void CHangeQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }
    }
}