using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using Cubilys.Easings;

namespace Cubilys
{
	public class LevelManager : MonoBehaviour 
	{
		[Header("Audio")]
		public string clipPath;
		public AudioClip clip;
		public AudioImporter importer;
		public UnityEvent onClipLoaded;

		[Header("Line")]
		public PlayerMovement player;
        public GameObject playerHolder;
		public KeyCode backKey = KeyCode.Escape;
		public CysCachedGameObject playerCache;

		[Header("Camera")]
		public CameraHost mainCamera;
		public CysCachedGameObject mainCameraCache;

        [Header("Scene")]
        public MenuManager menu;
        public Material skyboxMaterial;
        public Color uiColor = Color.white;
        public Color fogColor = Color.black;
        public LevelInfo info;
        public UnityEvent onRestarted;
        public Transform[] resettableObjects;
        List<CysCachedGameObject> cachedObjects = new List<CysCachedGameObject>();

        // Privates
        [HideInInspector] public bool isRestarting;
        float previousDensity = 0.015f;

		void Start()
		{
			if (File.Exists (clipPath)) {
				LoadAudio (Storage.GetPathLocal(clipPath), (val) => 
				FindObjectOfType<PlayerMovement> ().source.clip = clip
				);
			}

			playerCache = new CysCachedGameObject(player.gameObject);
			mainCameraCache = new CysCachedGameObject(mainCamera.gameObject);
            foreach(var res in resettableObjects)
            {
                RecursiveSaving(res);
            }
		}

        public void RecursiveSaving(Transform tr)
        {
            cachedObjects.Add(new CysCachedGameObject(tr.gameObject));
            foreach(var trc in tr.GetComponentsInChildren<Transform>())
            {
                if(tr != trc) RecursiveSaving(trc);
            }
        }

        public void Restart()
        {
            if (isRestarting) return;

            isRestarting = true;
            previousDensity = RenderSettings.fogDensity;
            TweenTool.TweenFloat(previousDensity, 1f, 0.25f).SetEase(TweenType.InCubic).SetOnUpdate(val => RenderSettings.fogDensity = (float)val);
            Invoke("RestartAndFadeOut", 0.25f);
        }

        public void RestartAndFadeOut()
        {
            RestartForce(false);
            TweenTool.TweenFloat(1f, previousDensity, 0.25f).SetEase(TweenType.InCubic).SetOnUpdate(val => RenderSettings.fogDensity = (float)val);
            CallDelay.Call(0.25f, () => {
                isRestarting = false;
            });
        }
        
        public void RestartForce()
        {
            RestartForce(true);
        }

        public void RestartForce(bool setFalse)
        {
            isRestarting = true;
            TweenTool.tweenables.Clear();
            LeanTween.cancelAll();
            playerCache.ApplyToGameObject(player.gameObject);
            mainCameraCache.ApplyToGameObject(mainCamera.gameObject);
            player.DestroyTails();
            player.source.Stop();
            foreach (var cache in cachedObjects)
            {
                cache.ApplyToGameObject(cache.lastGameObject);
            }
            isRestarting = !setFalse;
            menu.OpenPanel();
            player.source.volume = 1;
            onRestarted.Invoke();
        }

        void SetLater()
        {
            isRestarting = false;
        }

        public void LoadAudio(string path, Action<AudioClip> onLoad)
		{
			StartCoroutine (LoadAudioAsync(path, onLoad));
		}

		IEnumerator LoadAudioAsync(string path, Action<AudioClip> onLoad)
		{
            if (Path.GetExtension(path).ToLower().Contains("mp3"))
            {
                importer.Import(path);

                while (!importer.isInitialized && !importer.isError)
                    yield return null;

                if (importer.isError)
                    Debug.LogError(importer.error);

                clip = importer.audioClip;
                if (onLoad != null) onLoad.Invoke(clip);
                onClipLoaded.Invoke();
            }
            else
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success && www.result != UnityWebRequest.Result.InProgress)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(www);
                        if (onLoad != null) onLoad.Invoke(clip);
                        onClipLoaded.Invoke();
                    }
                }
            }
		}

		private float[] ConvertByteToFloat(byte[] array)
		{
			float[] floatArr = new float[array.Length / 4];
			for (int i = 0; i < floatArr.Length; i++)
			{
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(array, i * 4, 4);
				}
				floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
			}
			return floatArr;
		}
	}

    [System.Serializable]
    public class LevelInfo
    {
        public string levelName;
        public string musicInfo;
    }
}