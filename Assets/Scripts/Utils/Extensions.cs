using UnityEngine;

namespace ArcadiaOnline.Utils
{
    public static class Extensions
    {
        public static float DistanceToXZ(this Transform a, Transform b)
        {
            Vector3 pa = new Vector3(a.position.x, 0f, a.position.z);
            Vector3 pb = new Vector3(b.position.x, 0f, b.position.z);
            return Vector3.Distance(pa, pb);
        }

        public static bool IsInLayerMask(this GameObject obj, LayerMask mask)
        {
            return (mask.value & (1 << obj.layer)) != 0;
        }
    }
}
