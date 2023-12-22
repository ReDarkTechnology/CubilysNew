using UnityEngine;

namespace Cubilys
{
    public class CysTrigger : MonoBehaviour
    {
        public string targetTrigger = "Player";

        public virtual void OnStart()
        {

        }

        public virtual void OnEnter(Collider other)
        {

        }

        public virtual void OnUndo()
        {

        }

        int revivePoint;
        CheckpointManager manager;
        void Start()
        {
            manager = FindObjectOfType<CheckpointManager>();
            if(manager != null) manager.OnUndo += UndoTrigger;
            var rend = GetComponent<MeshRenderer>();
            if (rend != null) rend.enabled = false;
            OnStart();
        }

        void UndoTrigger(int point)
        {
            if (revivePoint == point) OnUndo();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == targetTrigger)
            {
                revivePoint = manager.revivePoint;
                OnEnter(other);
            }
        }

        public static PlayerMovement GetLine(Collider other)
        {
            return other.GetComponent<PlayerMovement>();
        }
    }
}