using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*namespace Cubilys
{
    public class ExtCore : MonoBehaviour
    {
        // FEATURE TODO : Adding hierarchy
        // FEATURE TODO : Updating TriggerCollection's inspector
        // SUGGESTION TODO : ADDING ARPHROS SUPPORT (might modify Arphros too)

        private static ExtCore p_instance;
        public static ExtCore instance
        {
            get
            {
                p_instance = CysUtility.GetStaticInstance(p_instance);
                return p_instance;
            }
        }

        // Statics
        public static EditorPlayState playState
        {
            get
            {
                return p_state;
            }
            set
            {
                p_state = value;
                instance.shownPlayState = value;
                if (instance.onPlaymodeChanged != null) instance.onPlaymodeChanged.Invoke(p_state);
            }
        }
        public static EditorPlayState p_state = EditorPlayState.Stopped;

        public EditorPlayState shownPlayState = EditorPlayState.Stopped;
        public Action<EditorPlayState> onPlaymodeChanged;
        public static bool isOnlyPlaymode
        {
            get
            {
                return instance.isPlaymodeOnly;
            }
            set
            {
                instance.isPlaymodeOnly = value;
            }
        }

        // MonoBehaviour
        public bool isPlaymodeOnly;
        public Camera editorCamera;
        public CameraMovement playmodeCamera;

        public LevelManager levelManager;
        public PlayerMovement lineMovement;
        public ExtAudioManager audioManager;

        public Transform globalParent;

        public MeshRenderer[] replaceRequestRenderers;

        public Action<GameObject> OnObjectUpdate;
        public Action OnActionUpdate;
        public Action OnClearObject;
        public Action<ExtFieldInspect> OnInspectorChanged;

        public GameObject selectorInstance;

        public Material defaultMaterial;

        public GameObject[] configurations;

        // UI
        //public UIAnimationControl[] animationControls;
        public Image playButtonImage;
        public Image[] pauseButtonImages;

        List<Transform> wasSelectedObjects = new List<Transform>();

        public GameObject[] onlyAndroidUI;
        public GameObject[] onlyDesktopUI;

        //Mesh Collection
        public Mesh[] meshes;
        public static Mesh GetMesh(int index)
        {
            return instance.meshes.Length > index ? instance.meshes[index] : null;
        }

        public static int GetMeshIndex(Mesh mesh)
        {
            for(int i = 0; i < instance.meshes.Length; i++)
            {
                if (mesh == instance.meshes[i]) return i;
            }
            return -1;
        }

        void Start()
        {
            playState = EditorPlayState.Stopped;
            lineMovement.Start();
            p_instance = this;
            //playmodeCamera.gameObject.SetActive(false);
            foreach(var r in replaceRequestRenderers) CysUtility.ReplaceWithNewMaterials(r);

#if !UNITY_EDITOR && UNITY_ANDROID
            foreach (var ui in onlyAndroidUI)
            {
                ui.SetActive(true);
            }
            foreach(var ui in onlyDesktopUI)
            {
                ui.SetActive(false);
            }
#endif
            if (isPlaymodeOnly)
            {
                PlayGame();
            }
            //EnetProfile.SetProfileView(false);
        }

        public static void Reset()
        {
            CysObject.objects.Clear();
            CysObject.lastID = 0;
        }

        public CysObject[] GetObjects()
        {
            return globalParent.GetComponentsInChildren<CysObject>();
        }

        public static CysObject GetObject(int id)
        {
        	if(id == 1) return instance.lineMovement.GetComponent<CysObject>();
            return CysObject.objects.ContainsKey(id) ? CysObject.objects[id] : null;
        }

        public static int AddObject(CysObject obj)
        {
            RemoveObject(obj);
            CysObject.lastID++;
            CysObject.objects.Add(CysObject.lastID, obj);
            return CysObject.lastID;
        }

        public static int AddObject(CysObject obj, int id)
        {
            RemoveObject(obj);
            if (CysObject.lastID < id) CysObject.lastID = id;
            if(CysObject.objects.ContainsKey(id))
            {
                CysObject.lastID++;
                id = CysObject.lastID;
            }
            CysObject.objects.Add(id, obj);
            return id;
        }

        public static void RemoveObject(CysObject obj)
        {
            if (CysObject.objects.ContainsKey(obj.instanceID))
            {
                if (CysObject.objects[obj.instanceID] == obj)
                {
                    CysObject.objects.Remove(obj.instanceID);
                }
            }
        }

        public CysObject CreateObject(GameObject config)
        {
            var obj = config.Instantiate(globalParent);
            HitRaycast(obj.transform);

            var rend = obj.GetComponent<MeshRenderer>();
            if (rend != null) CysUtility.ReplaceWithNewMaterials(rend);

            var scr = obj.AddOrGetComponent<CysObject>();
            scr.instanceID = -1;
            scr.applyNewID = true;
            scr.Initialize();

            return scr;
        }

        public TriggerCollection CreateTrigger(TriggerCollection.TrigType type)
        {
            var obj = CreateObject(triggerInstance);
            var comp = obj.GetComponent<TriggerCollection>();
            comp.TriggerTypes = type;
            comp.SetupColor();
            return comp;
        }

        public void HitRaycast(Transform transf)
        {
            // Try raycasting
            RaycastHit hit;
            var isHitting = Physics.Raycast(editorCamera.transform.position, editorCamera.transform.forward, out hit);

            if (isHitting)
            {
                transf.position = hit.point + new Vector3(0, (transf.localScale.y / 2), 0);
            }
            else
            {
                transf.position = editorCamera.transform.position + (editorCamera.transform.forward * 20);
            }
        }

        public void CreateObject(int index)
        {
            CreateObject(configurations[index]);
        }

        public void CreateTrigger(int index)
        {
            CreateTrigger(types[index]);
        }

        public LiwProject tempProject;
        public TemporaryStates tempLineState = new TemporaryStates();
        public TemporaryStates tempCameraState = new TemporaryStates();
        bool wasPlaying;

        public void PlayGame()
        {
            if (playState == EditorPlayState.Stopped)
            {
                tempProject = ExtProjectManager.instance.GetProject();
                if (tempLineState == null) tempLineState = new TemporaryStates();
                if (tempCameraState == null) tempCameraState = new TemporaryStates();
                tempLineState.SaveState(lineMovement, lineMovement.GetType(), typeof(MonoBehaviour));
                tempCameraState.SaveState(playmodeCamera, playmodeCamera.GetType(), typeof(MonoBehaviour));

                SetPauseIcon(false);
                SetPlayIcon(true);
                SetIsPlaying(true);
                foreach (var trigger in FindObjectsOfType<LineWorldsMod.ModTrigger>())
                {
                    trigger.OnGameStart();
                }

                playmodeCamera.ForceCameraWithCurrent();
            }
            else if (playState == EditorPlayState.Paused)
            {
                LeanTween.resumeAll();
                if (wasPlaying) lineMovement.source.Play();
                SetPauseIcon(false);
                SetPlayIcon(true);
                SetIsPlaying(true);
            }
            playState = EditorPlayState.Playing;
        }

        public void PauseGame()
        {
            if (playState == EditorPlayState.Playing)
            {
                LeanTween.pauseAll();
                wasPlaying = lineMovement.source.isPlaying;
                lineMovement.source.Pause();
                SetIsPlaying(false);
                SetPauseIcon(true);
                SetPlayIcon(false);
            }
            playState = EditorPlayState.Paused;
        }

        public void StopGame()
        {
            if (playState != EditorPlayState.Stopped)
            {
                SetIsPlaying(false);
                foreach (var trigger in FindObjectsOfType<LineWorldsMod.ModTrigger>())
                {
                    trigger.OnGameStop();
                }
                LeanTween.cancelAll();
                tempCameraState.ApplyToSource(playmodeCamera);
                ExtProjectManager.instance.ApplyProjectToScene(tempProject, true);

                SetPauseIcon(false);
                SetPlayIcon(false);
                lineMovement.source.Stop();
                lineMovement.source.volume = 1;
                lineMovement.DestroyAllTail();
                lineMovement.isStarted = false;
                tempLineState.ApplyToSource(lineMovement);
            }
            playState = EditorPlayState.Stopped;
        }

        public void SetIsPlaying(bool to)
        {
            if(editorCamera != null) editorCamera.gameObject.SetActive(!to);
            //playmodeCamera.gameObject.SetActive(to || isPlaymodeOnly);
            playmodeCamera.enabled = to || isPlaymodeOnly;
            playmodeCamera.mainCamera.enabled = to || isPlaymodeOnly;
            // Debug.Log("SetIsPlaying passed camera check");

            if (!isPlaymodeOnly)
            {
                if (to)
                {
                    wasSelectedObjects = new List<Transform>(ExtSelection.instance.transformSelection);
                    ExtSelection.instance.ClearTargets();
                }
                else
                {
                    ExtSelection.instance.transformSelection = wasSelectedObjects;
                }
            }

            lineMovement.enabled = to;
            var rigid = lineMovement.GetComponent<Rigidbody>();
            rigid.useGravity = to;
            rigid.constraints = to ? RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ : RigidbodyConstraints.FreezeAll;
            rigid.isKinematic = !to;
            rigid.velocity = Vector3.zero;
            foreach(var trigger in FindObjectsOfType<TriggerCollection>())
            {
                trigger.enabled = to;
                var col = trigger.GetComponent<Collider>();
                col.enabled = false;
                trigger.GetComponent<MeshRenderer>().enabled = !to;
                col.enabled = true;
                if (to) trigger.FindObject();
            }

            foreach (var trigger in FindObjectsOfType<LineWorldsMod.ModTrigger>())
            {
                trigger.enabled = to;
                var col = trigger.GetComponent<Collider>();
                col.enabled = false;
                trigger.GetComponent<MeshRenderer>().enabled = !to;
                col.enabled = true;
                if (to) trigger.FindObject();
            }
            SetAnimationControls(!to);
        }

        public void SetPlayIcon(bool to)
        {
            if(playButtonImage != null) playButtonImage.color = to ? new Color(0, 1, 0, 1) : Color.white;
        }

        public void SetPauseIcon(bool to)
        {
            foreach(var i in pauseButtonImages)
            {
                playButtonImage.color = to ? new Color(0.6f, 0.6f, 0.6f) : Color.white;
            }
        }

        public void SetAnimationControls(bool to)
        {
            foreach(var ctrl in animationControls)
            {
                if (to)
                    ctrl.MaybeOpen();
                else
                    ctrl.MaybeClose();
            }
        }

        private void OnDestroy()
        {
            Reset();
            p_instance = null;
        }
    }
    
    [System.Serializable]
    public class ObjectConfiguration
    {
    	public string name;
        public GameObject instance;
        public float farDistance = 10;
    }

    public enum EditorPlayState
    {
        Playing,
        Paused,
        Stopped
    }
}*/