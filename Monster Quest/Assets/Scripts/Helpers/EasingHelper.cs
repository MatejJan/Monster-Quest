using UnityEngine;

namespace MonsterQuest
{
    public static class EasingHelper
    {
        public static float Ease(float parameter, Easing easing)
        {
            return easing switch
            {
                Easing.EaseIn => 1 - Mathf.Cos(parameter * Mathf.PI / 2),
                Easing.EaseOut => Mathf.Sin(parameter * Mathf.PI / 2),
                Easing.EaseInOut => 0.5f - Mathf.Cos(parameter * Mathf.PI) / 2,
                _ => parameter
            };
        }

        public static float EaseIn(float parameter)
        {
            return Ease(parameter, Easing.EaseIn);
        }

        public static float EaseOut(float parameter)
        {
            return Ease(parameter, Easing.EaseOut);
        }

        public static float EaseInOur(float parameter)
        {
            return Ease(parameter, Easing.EaseInOut);
        }
    }
}
