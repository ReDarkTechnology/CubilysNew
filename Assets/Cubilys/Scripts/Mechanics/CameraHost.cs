using UnityEngine;
using System.Collections;

namespace Cubilys
{
    public class CameraHost : MonoBehaviour
    {
        public Camera mainCamera;
        [AllowSavingState]
        public Transform line;

        public virtual Camera GetMainCamera()
        {
            return mainCamera;
        }

        public virtual Transform GetTarget()
        {
            return GetTarget();
        }

        public virtual void StayInPosition()
        {
            GameObject currPos = new GameObject();
            currPos.transform.position = line.position;
            currPos.transform.rotation = transform.rotation;
            line = currPos.transform;
        }
    }
}
