using System;
using System.IO;
using UnityEngine;

public class BundledAssetsLoader : KMonoBehaviour
{
	public BundledAssets Expansion1Assets { get; private set; }

	protected override void OnPrefabInit()
	{
		BundledAssetsLoader.instance = this;
		global::Debug.Log("Expansion1: " + DlcManager.IsExpansion1Active().ToString());
		if (DlcManager.IsExpansion1Active())
		{
			global::Debug.Log("Loading Expansion1 assets from bundle");
			AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, DlcManager.GetContentBundleName("EXPANSION1_ID")));
			global::Debug.Assert(assetBundle != null, "Expansion1 is Active but its asset bundle failed to load");
			GameObject gameObject = assetBundle.LoadAsset<GameObject>("Expansion1Assets");
			global::Debug.Assert(gameObject != null, "Could not load the Expansion1Assets prefab");
			this.Expansion1Assets = Util.KInstantiate(gameObject, base.gameObject, null).GetComponent<BundledAssets>();
		}
	}

	public static BundledAssetsLoader instance;
}
