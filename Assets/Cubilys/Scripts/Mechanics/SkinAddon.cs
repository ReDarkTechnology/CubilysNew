using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Cubilys
{
    public class SkinAddon : MonoBehaviour
    {
        public PlayerMovement movement;
        public SkinItem[] skins;
        public int currentSkin;

        public Transform globalParent;
        public List<GameObject> spawnedSkin;
        void Start()
        {
            movement = movement == null ? FindObjectOfType<PlayerMovement>() : movement;
            movement.onLineTap.AddListener(OnLineTap);
            var c = PlayerPrefs.GetInt("CurrentSkin", 0);
            ApplySkin(c);
        }

        private void Update()
        {
            var s = skins[currentSkin];
            if(movement.isStarted)
            {
                if(movement.isMoving)
                {
                    if (s.alwaysSpawned != null)
                    {
                        var i = Instantiate(s.alwaysSpawned, globalParent);
                        i.transform.position = movement.transform.position;
                        if (s.destroyAfter > 0) Destroy(i, s.destroyAfter);
                        spawnedSkin.Add(i);
                    }
                }
            }
        }

        public void OnLineTap()
        {
            var s = skins[currentSkin];
            if (s.spawnWhenTap != null)
            {
                var i = Instantiate(s.spawnWhenTap, globalParent);
                i.transform.position = movement.transform.position;
                if (s.destroyTapAfter > 0) Destroy(i, s.destroyTapAfter);
                spawnedSkin.Add(i);
            }
        }

        public void ApplySkin(int index)
        {
            foreach(var o in spawnedSkin)
            {
                if (o != null) Destroy(o);
            }
            spawnedSkin.Clear();
            var s = skins[currentSkin];
            if (s.coreObject != null)
            {
                var o = Instantiate(s.coreObject, movement.transform);
                spawnedSkin.Add(o);
            }
        }
    }
}
