using UnityEngine;

namespace ArcadiaOnline.Utils
{
    public static class MathHelper
    {
        public static float RoundToDecimal(float value, int decimals)
        {
            float factor = Mathf.Pow(10, decimals);
            return Mathf.Round(value * factor) / factor;
        }

        public static bool RollChance(float percentChance)
        {
            return Random.value * 100f < percentChance;
        }

        public static Vector3 RandomPointInRadius(Vector3 center, float radius)
        {
            Vector2 point = Random.insideUnitCircle * radius;
            return center + new Vector3(point.x, 0f, point.y);
        }
    }
}
