using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Cubilys.Utilities
{
    public static class Vector3Util
    {
        public static Vector3 Singularite(Vector3 from)
        {
            Quaternion rot = Quaternion.Euler(from);

            Vector3 result = rot * Vector3.forward;
            return result;
        }
        
        public static Vector3[] GetLine(Vector3 from, Vector3 to, Vector3 minimumScale)
        {
            Vector3 centeredPosition = CenterOfVectors(new[] { from, to });
            Vector3 scaledLine = from - to;
            Vector3 fixedScaledLine = new Vector3(II(Mathf.Abs(scaledLine.x), minimumScale.x), II(Mathf.Abs(scaledLine.y), minimumScale.y), II(Mathf.Abs(scaledLine.z), minimumScale.z));
            return new[] { centeredPosition, fixedScaledLine };
        }
        
        private static float II(float val, float limit)
        {
            if (val < limit)
            {
                val = limit;
            }
            return val;
        }

        private static Vector3 CenterOfVectors(IReadOnlyCollection<Vector3> vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Count == 0)
            {
                return sum;
            }

            sum = vectors.Aggregate(sum, (current, vec) => current + vec);
            return sum / vectors.Count;
        }
    }
}