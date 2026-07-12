using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BundledAssetsLoader : KMonoBehaviour
{
	public BundledAssets Expansion1Assets { get; private set; }

	public List<BundledAssets> DlcAssetsList { get; private set; }

	protected override void OnPrefabInit()
	{
		BundledAssetsLoader.instance = this;
		if (DlcManager.IsExpansion1Active())
		{
			global::Debug.Log("Loading Expansion1 assets from bundle");
			AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, DlcManager.GetContentBundleName("EXPANSION1_ID")));
			global::Debug.Assert(assetBundle != null, "Expansion1 is Active but its asset bundle failed to load");
			GameObject gameObject = assetBundle.LoadAsset<GameObject>("Expansion1Assets");
			global::Debug.Assert(gameObject != null, "Could not load the Expansion1Assets prefab");
			this.Expansion1Assets = Util.KInstantiate(gameObject, base.gameObject, null).GetComponent<BundledAssets>();
		}
		this.DlcAssetsList = new List<BundledAssets>(DlcManager.DLC_PACKS.Count);
		foreach (KeyValuePair<string, DlcManager.DlcInfo> keyValuePair in DlcManager.DLC_PACKS)
		{
			if (DlcManager.IsContentSubscribed(keyValuePair.Key))
			{
				global::Debug.Log("Loading DLC " + keyValuePair.Key + " assets from bundle");
				AssetBundle assetBundle2 = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, DlcManager.GetContentBundleName(keyValuePair.Key)));
				global::Debug.Assert(assetBundle2 != null, "DLC " + keyValuePair.Key + " is Active but its asset bundle failed to load");
				GameObject gameObject2 = assetBundle2.LoadAsset<GameObject>(keyValuePair.Value.directory + "Assets");
				global::Debug.Assert(gameObject2 != null, "Could not load the " + keyValuePair.Key + " prefab");
				this.DlcAssetsList.Add(Util.KInstantiate(gameObject2, base.gameObject, null).GetComponent<BundledAssets>());
			}
		}
	}

	public static BundledAssetsLoader instance;
}
