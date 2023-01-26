using System.Collections.Generic;

namespace MonsterQuest
{
    public static class TargetTypeHelper
    {
        public static string GetDescription(this TargetType targetType)
        {
            if ((targetType & TargetType.All) == TargetType.All) return "target";
            if (targetType == TargetType.None) return "nothing";

            List<string> targets = new();
            if ((targetType & TargetType.Creature) == TargetType.Creature) targets.Add("creature");
            if ((targetType & TargetType.Object) == TargetType.Object) targets.Add("object");
            if ((targetType & TargetType.Location) == TargetType.Location) targets.Add("location");

            return string.Join(" or ", targets);
        }
    }
}
