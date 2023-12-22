using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OsuToCubilys;
using Cubilys.Utilities;

namespace Cubilys.Midi
{
    public class CubilysParser : MonoBehaviour
    {
        [Header("Components")]
        public PlayerMovement player;
        public AudioSource source;

        [Header("Autoplay")]
        public bool autoplayLineWithMidi;
        public bool forceLineAdjustment;
        public int timingIndex;

        [Header("Beatmap")]
        public bool readBeatmapBeforeGenerating = true;
        public string beatmapFilePath;
        public int selectedPoint = 0;
        public List<TimingPoints> points = new List<TimingPoints>();
        public float[] timingsFound { get => points[selectedPoint].timings; set => points[selectedPoint].timings = value; }
        public List<Transform> autoPoints { get => points[selectedPoint].points; set => points[selectedPoint].points = value; }

        [Header("Adjustments")]
        public bool reverseTurn;
        public Vector3 positionOffset;
        public float offset = 0;
        public float generationScale = 1f;

        [HideInInspector] public int lc;

        private TimingData _timingData;

        private void Start()
        {
            if (player == null) player = FindObjectOfType<PlayerMovement>(true);
            ReadFromFile();
        }

        private void Update()
        {
            if (!player.isStarted)
                return;

            if (!autoplayLineWithMidi)
                return;

            if (timingIndex >= timingsFound.Length)
                return;

            if (!(source.time + Time.deltaTime - offset > timingsFound[timingIndex]))
                return;

            if (forceLineAdjustment)
                player.AdjustLine(autoPoints[timingIndex].position);

            player.TurnLine();
            timingIndex++;
        }

        private void ReadFromFile()
        {
            if (!File.Exists(beatmapFilePath)) return;

            _timingData = JsonUtility.FromJson<TimingData>(File.ReadAllText(beatmapFilePath));
            GetDataFromBeatmap();
        }

        private void GetDataFromBeatmap()
        {
            ICollection<HitPoint> notes = _timingData.hitPoints;
            timingsFound = RemoveEqualTimings(notes.Select(note => note.time)).ToArray();
            Array.Sort(timingsFound);
        }

        private static IEnumerable<float> RemoveEqualTimings(IEnumerable<float> origin)
        {
            var newTimings = new List<float>();
            foreach (float num in origin)
            {
                if (!newTimings.Contains(num))
                    newTimings.Add(num);
            }
            return newTimings.ToArray();
        }

        #region Algorithm
        [Header("Results")]
        public List<Result> results = new List<Result>();

        [Serializable]
        public class Result
        {
            [Header("Direction")]
            public Vector3 eulerAngle;
            public Vector3 direction;
            [Header("Point")]
            public Vector3 startPoint;
            public Vector3 endPoint;
            [Header("Result")]
            public Vector3 pos;
            public Vector3 scale;
        }

        [ContextMenu("Clear")]
        public void ClearLines()
        {
            Transform tailParent = transform.Find("TailParent");
            if (tailParent != null)
            {
                DestroyImmediate(tailParent.gameObject);
            }

            Transform centerTailParent = transform.Find("CenterTailParent");
            if (centerTailParent != null)
            {
                DestroyImmediate(centerTailParent.gameObject);
            }
            autoPoints.Clear();
        }

        [ContextMenu("Generate")]
        public void GenerateLines()
        {
            if (readBeatmapBeforeGenerating)
            {
                if (timingsFound.Length <= 0)
                    ReadFromFile();
            }
            float[] timings = timingsFound;
            float speed = player.speed * generationScale;

            if (player == null)
                return;

            Transform tailParent = transform.Find("TailParent");
            if (tailParent == null)
            {
                tailParent = new GameObject().transform;
                tailParent.SetParent(transform);
                tailParent.gameObject.name = "TailParent";
            }

            Transform centerTailParent = transform.Find("CenterTailParent");
            if (centerTailParent == null)
            {
                centerTailParent = new GameObject().transform;
                centerTailParent.SetParent(transform);
                centerTailParent.gameObject.name = "CenterTailParent";
            }

            autoPoints.Clear();
            results.Clear();
            int turn = lc;
            Vector3 lastPosition = player.transform.position + positionOffset;
            var startTail = DrawCube(lastPosition, player.transform.localScale);
            startTail.transform.SetParent(centerTailParent);
            for (int i = 0; i < timings.Length; i++)
            {
                float timing;
                float nextTiming = timings[i];
                if (i - 1 < 0)
                    timing = 0;
                else
                    timing = timings[i - 1];

                float diff = nextTiming - timing;
                if (i == 0) diff += offset;
                float size = speed * Mathf.Abs(diff);

                bool to = turn % 2 != 0;
                to = reverseTurn ? !to : to;
                Vector3 angle = to ? player.directions[0] : player.directions[1];
                Vector3 direction = Vector3Util.Singularite(angle);

                Vector3[] result = Vector3Util.GetLine(lastPosition, lastPosition + size * direction, player.transform.localScale);
                Result res = new Result()
                {
                    startPoint = lastPosition,
                    eulerAngle = angle,
                    direction = direction,
                    endPoint = lastPosition + (size * direction),
                    pos = result[0],
                    scale = result[1]
                };
                results.Add(res);

                GameObject tail = DrawCube(result[0], result[1]);
                tail.transform.SetParent(tailParent);
                lastPosition = res.endPoint;

                GameObject centerTail = DrawCube(lastPosition, player.transform.localScale);
                centerTail.transform.SetParent(centerTailParent);
                turn++;

                autoPoints.Add(centerTail.transform);
            }
        }

        private static GameObject DrawCube(Vector3 pos, Vector3 scale, Transform parent = null)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.GetComponent<BoxCollider>().isTrigger = true;
            obj.transform.position = pos;
            obj.transform.localScale = scale;

            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }

            return obj;
        }

        [ContextMenu("Regenerate")]
        public void ClearAndGenerate()
        {
            ClearLines();
            GenerateLines();
        }
        #endregion
    }

    [Serializable]
    public class TimingPoints
    {
        public float[] timings;
        public List<Transform> points = new List<Transform>();
    }
}