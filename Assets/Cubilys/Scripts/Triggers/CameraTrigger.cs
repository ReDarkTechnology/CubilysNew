using System;
using UnityEngine;
using Cubilys.Easings;

namespace Cubilys.Triggers
{
    public class CameraTrigger : CysTrigger
    {
        [Header("Variables")]
        public bool setStart;
        public CameraValues startValues = new CameraValues();
        public bool setTween = true;
        public CameraValues tweenValues = new CameraValues();
        public bool changeTargetObject;
        public Transform targetObject;
        
        [Header("Tweening")]
        public TweenType tweenType = TweenType.OutCubic;
        public float tweenTime = 1f;
        public float tweenDelay;

        public override void OnEnter(Collider other)
        {
            if (CameraMovement.current != null)
            {
                if (changeTargetObject)
                {
                    CameraMovement.current.line = targetObject;
                }
                if (setStart)
                {
                    startValues.ApplyValues(CameraMovement.current);
                }
                if (setTween)
                {
                    tweenValues.TweenValues(CameraMovement.current, tweenType, tweenTime, tweenDelay);
                }
            }
            else
            {
                Debug.Log("Camera is not found...");
            }
        }

        /*private void OnTriggerEnter(Collider other)
        {
        }*/
    }

    [Serializable]
    public class CameraValues
    {
        public bool changeRotation = true;
        public Vector3 targetRotation = new Vector3(45f, 60f, 0);

        public bool changeDistance;
        public float targetDistance = 20f;

        public bool changeTimes;
        [Range(0.001f, 10f)]
        public float smoothFactor = 1f;

        /*public bool changeInsideRotation;
        public Vector3 insideCameraRotation;
        public bool changeInsidePosition;
        public Vector3 insideCameraPosition;*/

        public bool changeCameraOffset;
        public Vector3 cameraOffset;

        public static CameraValues GetCameraValues(CameraMovement cam)
        {
            var values = new CameraValues();
            values.TakeValues(cam);
            return values;
        }

        public void TakeValues(CameraMovement cam)
        {
            targetRotation = cam.targetRotation;
            targetDistance = cam.targetDistance;
            smoothFactor = cam.smoothFactor;
            cameraOffset = cam.pivotOffset;
            //insideCameraRotation = cam.mainCamera.transform.localEulerAngles;
        }
        public void ApplyValues(CameraMovement cam)
        {
            if (changeRotation)
            {
                cam.targetRotation = targetRotation;
            }
            if (changeDistance) cam.targetDistance = targetDistance;
            if (changeTimes)
            {
                cam.smoothFactor = smoothFactor;
            }
            //if (changeInsideRotation) cam.localEulerOffset = insideCameraRotation;
            if (changeCameraOffset) cam.pivotOffset = cameraOffset;
        }
        public void TweenValues(CameraMovement cam, TweenType tweenType, float tweenTime, float tweenDelay)
        {
            if (changeRotation)
            {
                var camRotation = cam.targetRotation;
                TweenTool.TweenVector3(camRotation, targetRotation, tweenTime).SetEase(tweenType).SetDelay(tweenDelay).SetOnUpdate(
                    (object result) => cam.targetRotation = (Vector3)result);
            }
            if (changeDistance)
            {
                TweenTool.TweenFloat(cam.targetDistance, targetDistance, tweenTime).SetEase(tweenType).SetDelay(tweenDelay).SetOnUpdate(
                    result => cam.targetDistance = (float)result);
            }
            if (changeTimes)
            {
                TweenTool.TweenFloat(cam.smoothFactor, smoothFactor, tweenTime).SetEase(tweenType).SetDelay(tweenDelay).SetOnUpdate(
                    result => cam.smoothFactor = (float)result);
            }
            /*if (changeInsideRotation)
            {
                LeanTween.value(cam.gameObject, cam.localEulerOffset, insideCameraRotation, tweenTime).setEase(tweenType).setDelay(tweenDelay).setOnUpdate(
                    (Vector3 result) =>
                    {
                        cam.localEulerOffset = result;
                    }
                );
            }
            if (changeInsideRotation)
            {
                LeanTween.value(cam.gameObject, cam.localPositionOffset, insideCameraPosition, tweenTime).setEase(tweenType).setDelay(tweenDelay).setOnUpdate(
                    (Vector3 result) =>
                    {
                        cam.localPositionOffset = result;
                    }
                );
            }*/
            if (changeCameraOffset)
                TweenTool.TweenVector3(cam.pivotOffset, cameraOffset, tweenTime).SetEase(tweenType).SetDelay(tweenDelay).SetOnUpdate(
                    result =>
                    {
                        cam.pivotOffset = (Vector3)result;
                    }
                );
        }
    }
}