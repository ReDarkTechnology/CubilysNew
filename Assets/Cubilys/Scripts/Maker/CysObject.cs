using UnityEngine;
using System.Collections.Generic;

namespace Cubilys
{
    public class CysObject : MonoBehaviour
    {
        [Tooltip("Leave on -1 to assign a new instance ID on start")]
        public int instanceID;
        public bool isInitialized;
        public CysObjectType objectType = CysObjectType.Default;
        
        void Start()
        {
            if (isInitialized) return;
            Initialize();
        }

        public void Initialize()
        {
            instanceID = AddObject(this, instanceID);
            isInitialized = true;
        }

        // Static Behaviour
        public static Dictionary<int, CysObject> objects = new Dictionary<int, CysObject>();
        public static int lastID;

        public static CysObject GetObject(int id)
        {
            if (objects.ContainsKey(id)) return objects[id];
            return null;
        }

        public static int AddObject(CysObject obj, int id)
        {
            if (!objects.ContainsKey(id) && id > -1)
            {
                objects.Add(id, obj);
                return id;
            }
            lastID++;
            objects.Add(lastID, obj);
            return id;
        }
    }

    public enum CysObjectType
    {
        Default,
        Custom
    }
}