using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubilys
{
	[ExecuteInEditMode]
	public class CameraMovement : CameraHost
    {
        private static CameraMovement p_instance;
        public static CameraMovement current
        {
            get
            {
                p_instance = CysUtility.GetStaticInstance(p_instance);
                return p_instance;
            }
        }

        [Header("Variables")]
        [AllowSavingState]
        public Vector3 pivotOffset = Vector3.zero;
        [AllowSavingState]
        public Vector3 targetRotation = new Vector3 (45f, 60f, 0);
        [AllowSavingState]
        public float targetDistance = 20f;
        [AllowSavingState]
        [Range(0.001f, 10f)]
        public float smoothFactor = 1f;

        [AllowSavingState]
        public Vector3 localPositionOffset;
        [AllowSavingState]
        public Vector3 localEulerOffset;

		[Header("Shake")]
        [AllowSavingState]
        public float shakeDuration = 0f;
        [AllowSavingState]
        public float shakeAmount = 0.7f;
        [AllowSavingState]
        private float decreaseFactor = 1.0f;

		[Header("Misc")]
		public bool simulateInEditor = true;
		void Start()
		{
			if (mainCamera == null)
				return;
			if (line != null) {
				transform.position = line.position + pivotOffset;
			} else {
				transform.position = Vector3.zero + pivotOffset;
			}
			mainCamera.transform.localPosition = new Vector3(0, 0, -targetDistance);
			base.transform.eulerAngles = targetRotation;
		}

        private void OnEnable()
        {
            p_instance = this;
        }

        void Update()
		{
			if (mainCamera == null)
				return;
			if(Application.isPlaying){
				Process (false);
			}else{
				if(simulateInEditor){
					Process (true);
				}
			}
		}
		public void Process(bool quick)
		{
			Vector3 targetDist = new Vector3(0, 0, -targetDistance) + localPositionOffset;
			base.transform.eulerAngles = targetRotation;

			if (quick) {
				if (line != null) {
					transform.position = line.position + pivotOffset;
				} else {
					transform.position = Vector3.zero + pivotOffset;
				}
			} else {
				if (line != null) {
					transform.position = Vector3.Slerp (transform.position, line.position + pivotOffset, smoothFactor * Time.deltaTime);
				} else {
					transform.position = Vector3.Slerp (transform.position, Vector3.zero + pivotOffset, smoothFactor * Time.deltaTime);
				}
			}

			if (shakeDuration > 0) {
				mainCamera.transform.localPosition = targetDist + Random.onUnitSphere * shakeAmount;
				shakeDuration -= decreaseFactor * Time.deltaTime;
			} else {
				mainCamera.transform.localPosition = targetDist;
				shakeDuration = 0f;
            }
            mainCamera.transform.localEulerAngles = localEulerOffset;
        }

        public override Transform GetTarget()
        {
            return line;
        }
    }
}