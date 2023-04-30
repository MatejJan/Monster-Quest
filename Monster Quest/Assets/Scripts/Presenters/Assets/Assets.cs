using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace MonsterQuest.Presenters
{
    public static class Assets
    {
        private static readonly List<BodyAsset> _bodyAssets = new();

        public static IEnumerable<BodyAsset> bodyAssets => _bodyAssets;

        public static IEnumerator Initialize()
        {
            yield return Addressables.InitializeAsync();

            // Load all assets.
            yield return LoadAssets(_bodyAssets);
        }

        public static BodyAsset GetBodyAsset(string name)
        {
            return _bodyAssets.First(bodyAsset => bodyAsset.name == name);
        }

        private static IEnumerator LoadAssets<TObject>(List<TObject> list)
        {
            yield return Addressables.LoadAssetsAsync<TObject>("assets", list.Add);
        }
    }
}
