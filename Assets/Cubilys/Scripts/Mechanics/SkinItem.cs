using UnityEngine;
using System.Collections;

namespace Cubilys
{
    [CreateAssetMenu(fileName = "SkinItem", menuName = "Cubilys/Skin", order = 0)]
    public class SkinItem : ScriptableObject
    {
        public string skinName;
        public GameObject coreObject;
        public GameObject alwaysSpawned;
        public float destroyAfter = -1;
        public GameObject spawnWhenTap;
        public float destroyTapAfter = -1;
    }
}
