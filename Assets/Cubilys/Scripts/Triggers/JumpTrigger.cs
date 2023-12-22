using System;
using UnityEngine;
using Cubilys.Easings;

namespace Cubilys.Triggers
{
    public class JumpTrigger : CysTrigger
    {
        public float height = 100;

        public override void OnEnter(Collider other)
        {
            var rig = other.GetComponent<Rigidbody>();
            rig.AddForce(new Vector3(0, height * rig.mass, 0));
        }
    }
}