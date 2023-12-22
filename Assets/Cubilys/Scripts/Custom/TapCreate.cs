using UnityEngine;

namespace Cubilys.Custom
{
    public class TapCreate : MonoBehaviour
    {
        public PlayerMovement player;

        public GameObject instance;
        public OffsetType offsetType = OffsetType.Forward;
        public Vector3 fixedOffset = new Vector3(1, 0, 1);
        public float far = 2f;
        public float customY = 0;

        public Transform parent;

        public void OnPlayerTapped()
        {
            var inst = Instantiate(instance, parent);
            inst.transform.position = player.transform.position + (offsetType == OffsetType.Forward ? (player.transform.forward * far) : fixedOffset);
            inst.transform.position = new Vector3(inst.transform.position.x, customY, inst.transform.position.z);
        }
    }

    public enum OffsetType
    {
        Forward,
        Fixed
    }
}