using System.Collections.Generic;

namespace MonsterQuest
{
    public static class RuleValueExtensions
    {
        public static T[] Resolve<T>(this IEnumerable<ArrayValue<T>> values)
        {
            return ArrayValue<T>.Resolve(values);
        }

        public static Cover Resolve(this IEnumerable<CoverValue> values)
        {
            return CoverValue.Resolve(values);
        }

        public static DamageAlteration Resolve(this IEnumerable<DamageAmountAlterationValue> values)
        {
            return DamageAmountAlterationValue.Resolve(values);
        }

        public static int Resolve(this IEnumerable<IntegerValue> values, IntegerValue.ResolutionStrategy resolutionStrategy = IntegerValue.ResolutionStrategy.Maximum)
        {
            return IntegerValue.Resolve(values, resolutionStrategy);
        }

        public static T[] Resolve<T>(this IEnumerable<MultipleValue<T>> values)
        {
            return MultipleValue<T>.Resolve(values);
        }

        public static T Resolve<T>(this IEnumerable<SingleValue<T>> values)
        {
            return SingleValue<T>.Resolve(values);
        }
    }
}