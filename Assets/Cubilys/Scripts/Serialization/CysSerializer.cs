using System;
using System.Reflection;
using UnityEngine;

namespace Cubilys
{
    public class CysSerializer : MonoBehaviour
    {
        // Singleton
        public static CysSerializer Instance { get; private set; }
        public static CysSerializerSettings Settings {
            get
            {
                return Instance == null ? new CysSerializerSettings() : Instance.serializerSettings;
            }
        }
        public CysSerializerSettings serializerSettings;

        void Start()
        {
            Instance = this;
        }

        // Serializer Functions
        public static string GetSerializedObject(GameObject obj)
        {
            return null;
        }

        public static string GetSerializedComponent(Component obj)
        {
            return null;
        }
    }

    public class FieldConverter
    {
        public virtual string type
        {
            get
            {
                return null;
            }
        }

        public virtual string GetJsonValue(object obj)
        {
            return null;
        }

        public virtual object GetValidObject(string json)
        {
            return null;
        }
    }

    public static class ConverterCollection
    {
        public class StringConverter : FieldConverter
        {
            public override string type => "System.String";
            public override string GetJsonValue(object obj)
            {
                string result = JsonUtility.ToJson(new Wrapper { s = (string)obj });
                return result.Remove(result.Length - 1, 1).Remove(0, 5);
            }
            public override object GetValidObject(string json)
            {
                return JsonUtility.FromJson<Wrapper>("{\"s\":" + json +"}").s;
            }

            [Serializable]
            public class Wrapper
            {
                public string s;
            }
        }

        public class FloatConverter : FieldConverter
        {
            public override string type => "System.Single";
            public override string GetJsonValue(object obj)
            {
                string result = JsonUtility.ToJson(new Wrapper { n = (float)obj });
                return result.Remove(result.Length - 1, 1).Remove(0, 5);
            }
            public override object GetValidObject(string json)
            {
                return JsonUtility.FromJson<Wrapper>("{\"n\":" + json +"}").n;
            }

            [Serializable]
            public class Wrapper
            {
                public float n;
            }
        }

        public class IntConverter : FieldConverter
        {
            public override string type => "System.Int32";
            public override string GetJsonValue(object obj)
            {
                string result = JsonUtility.ToJson(new Wrapper { n = (int)obj });
                return result.Remove(result.Length - 1, 1).Remove(0, 5);
            }
            public override object GetValidObject(string json)
            {
                return JsonUtility.FromJson<Wrapper>("{\"n\":" + json +"}").n;
            }

            [Serializable]
            public class Wrapper
            {
                public int n;
            }
        }

        public class MaterialConverter : FieldConverter
        {
            public override string type => "UnityEngine.Material";
            public override string GetJsonValue(object obj)
            {
                var mat = (Material)obj;
                return JsonUtility.ToJson(new Wrapper() { color = mat.color });
            }
            public override object GetValidObject(string json)
            {
                var wr = JsonUtility.FromJson<Wrapper>(json);
                return null;
            }

            [Serializable]
            public class Wrapper
            {
                public Color color;
            }
        }

        public class EnumConverter : FieldConverter
        {
            public override string type => "System.Enum";
            public override string GetJsonValue(object obj)
            {
                return JsonUtility.ToJson(new Wrapper { type = obj.GetType().FullName, name = ((Enum)obj).ToString() });
            }
            public override object GetValidObject(string json)
            {
                var cls = JsonUtility.FromJson<Wrapper>(json);
                return Enum.Parse(Type.GetType(cls.type), cls.name);
            }

            [Serializable]
            public class Wrapper
            {
                public string type;
                public string name;
            }
        }

        public class GameObjectConverter : FieldConverter
        {
            public override string type => "UnityEngine.GameObject";
            public override string GetJsonValue(object obj)
            {
                try
                {
                    if (obj != null)
                    {
                        var ext = ((GameObject)obj).GetComponent<CysObject>().instanceID;
                        return JsonUtility.ToJson(new Wrapper { instanceID = ext });
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrapper { instanceID = -1 });
                    }
                }
                catch
                {
                    return JsonUtility.ToJson(new Wrapper { instanceID = -1 });
                }
            }
            public override object GetValidObject(string json)
            {
                try
                {
                    var cls = JsonUtility.FromJson<Wrapper>(json);
                    if (cls.instanceID > -1)
                    {
                        return CysObject.GetObject(cls.instanceID).gameObject;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }

            [Serializable]
            public class Wrapper
            {
                public int instanceID;
            }
        }

        public class UnityObjectConverter : FieldConverter
        {
            public override string type => "UnityEngine.Object";
            public override string GetJsonValue(object obj)
            {
                if (obj != null)
                {
                    var ext = ((MonoBehaviour)obj).GetComponent<CysObject>().instanceID;
                    return JsonUtility.ToJson(new Wrapper { targetType = obj.GetType().FullName, instanceID = ext });
                }
                else
                {
                    return JsonUtility.ToJson(new Wrapper { targetType = null, instanceID = -1 });
                }
            }
            public override object GetValidObject(string json)
            {
                var cls = JsonUtility.FromJson<Wrapper>(json);
                var type = CysUtility.GetType(cls.targetType);
                if (cls.instanceID > -1)
                {
                    return type == null ? null : CysObject.GetObject(cls.instanceID).GetComponent(type);
                }
                else
                {
                    return null;
                }
            }

            [Serializable]
            public class Wrapper
            {
                public string targetType;
                public int instanceID;
            }
        }
    }
}