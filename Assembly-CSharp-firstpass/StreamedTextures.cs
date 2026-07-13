using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class StreamedTextures
{
	public static Texture2D GetTexture(string tex_name)
	{
		Texture2D texture2D;
		StreamedTextures.TexLookup.TryGetValue(tex_name, out texture2D);
		return texture2D;
	}

	public static void SetBundlesLoaded(bool load)
	{
		StreamedTextures.ShouldBeLoaded = load;
	}

	public static bool AreBundlesLoaded()
	{
		return StreamedTextures.ShouldBeLoaded;
	}

	public static void UpdateRequests()
	{
		for (int i = 0; i < StreamedTextures.ActiveRequests.Count; i++)
		{
			StreamedTextures.LoadRequest loadRequest = StreamedTextures.ActiveRequests[i];
			AssetBundleCreateRequest request = loadRequest.Request;
			if (request.isDone)
			{
				string bundleName = loadRequest.BundleName;
				AssetBundle assetBundle = request.assetBundle;
				global::Debug.Assert(assetBundle != null, "Failed to load bundle: " + bundleName);
				TextureBundle component = assetBundle.LoadAsset<GameObject>(bundleName).GetComponent<TextureBundle>();
				global::Debug.Assert(component != null, "Bundle format mismatch");
				foreach (Texture2D texture2D in component.HiResTextures)
				{
					string text = texture2D.name.Replace("_hires_", "_");
					DebugUtil.DevAssert(!StreamedTextures.TexLookup.ContainsKey(text), "Texture already loaded!", null);
					StreamedTextures.TexLookup[text] = texture2D;
				}
				StreamedTextures.LoadedBundle loadedBundle = default(StreamedTextures.LoadedBundle);
				loadedBundle.Name = bundleName;
				loadedBundle.AssetBundle = assetBundle;
				StreamedTextures.LoadedBundles.Add(loadedBundle);
				StreamedTextures.ActiveRequests.RemoveAt(i);
				i--;
			}
		}
		if (StreamedTextures.ShouldBeLoaded && StreamedTextures.ActiveRequests.Count == 0 && StreamedTextures.LoadedBundles.Count == 0)
		{
			for (int k = 0; k < StreamedTextures.VanillaBundles.Length; k++)
			{
				string text2 = StreamedTextures.VanillaBundles[k];
				string text3 = Path.Combine(Application.streamingAssetsPath, text2);
				StreamedTextures.LoadRequest loadRequest2 = default(StreamedTextures.LoadRequest);
				loadRequest2.BundleName = text2;
				loadRequest2.Request = AssetBundle.LoadFromFileAsync(text3);
				StreamedTextures.ActiveRequests.Add(loadRequest2);
			}
			foreach (KeyValuePair<string, string[]> keyValuePair in StreamedTextures.DlcHiResBundles)
			{
				if (DlcManager.IsContentSubscribed(keyValuePair.Key))
				{
					for (int l = 0; l < keyValuePair.Value.Length; l++)
					{
						string text4 = keyValuePair.Value[l];
						string text5 = Path.Combine(Application.streamingAssetsPath, text4);
						StreamedTextures.LoadRequest loadRequest3 = default(StreamedTextures.LoadRequest);
						loadRequest3.BundleName = text4;
						loadRequest3.Request = AssetBundle.LoadFromFileAsync(text5);
						StreamedTextures.ActiveRequests.Add(loadRequest3);
					}
				}
			}
		}
		if (!StreamedTextures.ShouldBeLoaded && StreamedTextures.LoadedBundles.Count > 0)
		{
			while (StreamedTextures.LoadedBundles.Count > 0)
			{
				StreamedTextures.LoadedBundles[0].AssetBundle.Unload(true);
				StreamedTextures.LoadedBundles.RemoveAt(0);
			}
			StreamedTextures.TexLookup.Clear();
		}
	}

	private static string[] VanillaBundles = new string[] { "hires_base_bundle" };

	private static Dictionary<string, string[]> DlcHiResBundles = new Dictionary<string, string[]>
	{
		{
			"EXPANSION1_ID",
			new string[] { "hires_expansion1_bundle" }
		},
		{
			"DLC2_ID",
			new string[] { "hires_dlc2_bundle" }
		},
		{
			"DLC3_ID",
			new string[] { "hires_dlc3_bundle" }
		},
		{
			"DLC4_ID",
			new string[] { "hires_dlc4_bundle" }
		}
	};

	private static bool ShouldBeLoaded = false;

	private static Dictionary<string, Texture2D> TexLookup = new Dictionary<string, Texture2D>();

	private static List<StreamedTextures.LoadedBundle> LoadedBundles = new List<StreamedTextures.LoadedBundle>();

	private static List<StreamedTextures.LoadRequest> ActiveRequests = new List<StreamedTextures.LoadRequest>();

	private struct LoadedBundle
	{
		public string Name;

		public AssetBundle AssetBundle;
	}

	private struct LoadRequest
	{
		public string BundleName;

		public AssetBundleCreateRequest Request;
	}
}
