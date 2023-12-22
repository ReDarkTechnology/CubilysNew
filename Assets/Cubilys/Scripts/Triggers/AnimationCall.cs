using UnityEngine;

namespace Cubilys.Triggers
{
    public class AnimationCall : CysTrigger
    {
        public Custom.GroupedAnimations source;

        public override void OnEnter(Collider other)
        {
            source.CallTween();
            base.OnEnter(other);
        }

        public override void OnUndo()
        {
            base.OnUndo();
        }
    }
}
