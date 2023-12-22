using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

namespace Cubilys
{
    public static class CysUtility
    {
        public static T GetStaticInstance<T>(T cache)
        {
            return EqualityComparer<T>.Default.Equals(cache, default(T)) ? (T)(object)Object.FindObjectOfType(typeof(T)) : cache;
        }

        public static bool DoesObjectHaveComponent<T>(this GameObject obj)
        {
            return obj.GetComponent(typeof(T)) != null;
        }

        public static T AddOrGetComponent<T>(this GameObject obj)
        {
            var comp = obj.GetComponent(typeof(T));
            return comp == null ? (T)(object)obj.AddComponent(typeof(T)) : (T)(object)comp;
        }

        public static object AddOrGetComponent(this GameObject obj, Type type)
        {
            var comp = obj.GetComponent(type);
            return comp ?? obj.AddComponent(type);
        }

        public static void ReplaceWithNewMaterials(MeshRenderer rend)
        {
            var mats = new List<Material>();
            foreach (var mtrl in rend.sharedMaterials)
            {
                if (mtrl != null) mats.Add(new Material(mtrl));
            }
            rend.sharedMaterials = mats.ToArray();
        }

        public static List<Type> availableTypes;

        public static void CheckTypes()
        {
            if(availableTypes == null)
            {
                availableTypes = new List<Type>();
                var unityAssembly = Assembly.Load("UnityEngine");
                availableTypes.AddRange(unityAssembly.GetTypes());
                availableTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());
            }
        }

        public static Type GetType(string name)
        {
            CheckTypes();
            foreach(var type in availableTypes)
            {
                if(type.FullName == name) return type;
            }
            return null;
        }

        public static Type GetTypeWithPartialName(string name)
        {
            CheckTypes();
            foreach(var type in availableTypes)
            {
                if(type.Name == name) return type;
            }
            return null;
        }
    }      
}