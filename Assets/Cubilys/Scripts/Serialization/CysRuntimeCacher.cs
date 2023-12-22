using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
    
namespace Cubilys
{
    public class CysCachedGameObject
    {
        public Transform transform;
        public List<CachedComponent> caches = new List<CachedComponent>();

        public GameObject lastGameObject;

        public CysCachedGameObject() {}
        public CysCachedGameObject(GameObject obj)
        {
            lastGameObject = obj;
            transform = new Transform(obj.transform.position, obj.transform.eulerAngles, obj.transform.localScale);
            var components = obj.GetComponents<MonoBehaviour>();
            foreach (var comp in components)
            {
                try
                {
                    var cache = new CachedComponent();
                    cache.cache = new CysRuntimeCache(comp, comp.GetType());
                    cache.componentType = comp.GetType();
                    caches.Add(cache);
                }
                catch
                {
                    Debug.LogWarning("Unable to cache a component in: " + obj.name);
                }
            }
        }

        public void ApplyToGameObject(GameObject obj)
        {
            if (obj != null)
            {
                lastGameObject = obj;
                transform.Apply(obj.transform);
                foreach (var cache in caches)
                {
                    var comp = obj.GetComponent(cache.componentType);
                    if (comp != null) cache.cache.ApplyToSource(comp);
                }
            }
        }

        public class Transform
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;

            public Transform() {}
            public Transform(Vector3 p, Vector3 r, Vector3 s)
            {
                position = p;
                rotation = r;
                scale = s;
            }

            public void Apply(UnityEngine.Transform tr)
            {
                tr.position = position;
                tr.eulerAngles = rotation;
                tr.localScale = scale;
            }
        }

        public class CachedComponent
        {
            public Type componentType;
            public CysRuntimeCache cache;
        }
    }

    public class CysRuntimeCache
    {
        public List<SourceTemp> sources = new List<SourceTemp>();

        public CysRuntimeCache() {}
        public CysRuntimeCache(object source, Type mainType, Type excludedType = null)
        {
            SaveState(source, mainType, excludedType);
        }

        public void SaveState(object source, Type mainType, Type excludeType = null)
        {
            sources.Clear();
            var exProps = excludeType != null ? new List<FieldInfo>(excludeType.GetFields()) : new List<FieldInfo>();
            bool isSerializingAll = mainType.GetCustomAttribute(typeof(SaveAllStateAttribute)) != null;
            foreach (var p in mainType.GetFields())
            {
                bool isValidField = isSerializingAll ? 
                    p.GetCustomAttribute(typeof(IgnoreSavingStateAttribute)) == null : 
                    p.GetCustomAttribute(typeof(AllowSavingStateAttribute)) != null;
                if (isValidField)
                {
                    try
                    {
                        var cls = new SourceTemp(p, source);
                        sources.Add(cls);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(p.Name + " -> " + e.Message);
                    }
                }
            }
        }

        public void ApplyToSource(object source)
        {
            foreach (var s in sources)
            {
                try
                {
                    s.ApplyToSource(source);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Failed to apply: " + s.name + " -> " + e.Message);
                }
            }
        }

        public class SourceTemp
        {
            public bool isField;
            public string name;
            public object value;

            public SourceTemp(PropertyInfo info, object source)
            {
                name = info.Name;
                value = info.GetValue(source);
            }
            
            public SourceTemp(FieldInfo info, object source)
            {
                isField = true;
                name = info.Name;
                value = info.GetValue(source);
            }

            public void ApplyToSource(object source)
            {
                if(isField)
                {
                    source.GetType().GetField(name).SetValue(source, value);
                }
                else
                {
                    source.GetType().GetProperty(name).SetValue(source, value);
                }
            }
        }
    }

    public class IgnoreSavingStateAttribute : Attribute {}
    public class AllowSavingStateAttribute : Attribute {}
    public class SaveAllStateAttribute : Attribute {}
}