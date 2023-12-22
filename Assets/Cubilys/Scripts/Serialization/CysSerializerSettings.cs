using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Cubilys
{
    [CreateAssetMenu(fileName = "SerializerSettings", menuName = "Cubilys/Serializer Settings", order = 1)]
    public class CysSerializerSettings : ScriptableObject
    {
        public FieldSerializerMatch[] fieldMatches = new []{
            new FieldSerializerMatch("System.String", "Cubilys.ConverterCollection.StringConverter"),
            new FieldSerializerMatch("System.Single", "Cubilys.ConverterCollection.FloatConverter"),
            new FieldSerializerMatch("System.Int32", "Cubilys.ConverterCollection.IntConverter"),
            new FieldSerializerMatch("System.Enum", "Cubilys.ConverterCollection.EnumConverter")
        };
        public ComponentSerializerMatch[] componentMatches = new []{
            new ComponentSerializerMatch("UnityEngine.Transform", null, new []{
                "position", "rotation", "localScale"
            })
        };
    }

    [System.Serializable]
    public class FieldSerializerMatch
    {
        public string serializableType;
        public string serializerType;

        public FieldSerializerMatch()
        {

        }

        public FieldSerializerMatch(string type, string serializer)
        {
            serializableType = type;
            serializerType = serializer;
        }
    }

    [System.Serializable]
    public class ComponentSerializerMatch
    {
        public string serializableType;
        public bool serializeWithSerializer;

        public string serializerType;
        public string[] targetFields;

        public ComponentSerializerMatch()
        {

        }

        public ComponentSerializerMatch(string type, string serializer, string[] fields = null)
        {
            serializableType = type;
            serializeWithSerializer = !string.IsNullOrWhiteSpace(serializer);
            if(serializeWithSerializer)
                targetFields = fields;
            else
                serializerType = serializer;
        }
    }
}
