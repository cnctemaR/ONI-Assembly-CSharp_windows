using System;
using System.Collections.Generic;
using UnityEngine;

public class DlcManager
{
	public static bool IsVanillaId(string dlcId)
	{
		return dlcId == null || dlcId == "";
	}

	public static bool IsVanillaId(string[] dlcIds)
	{
		return dlcIds == null || (dlcIds.Length == 1 && dlcIds[0] == "");
	}

	public static bool IsValidForVanilla(string[] dlcIds)
	{
		return dlcIds == null || Array.IndexOf<string>(dlcIds, "") != -1;
	}

	public static bool IsExpansion1Id(string dlcId)
	{
		return dlcId == "EXPANSION1_ID";
	}

	public static string GetContentBundleName(string dlcId)
	{
		if (dlcId != null && dlcId == "EXPANSION1_ID")
		{
			return "expansion1_bundle";
		}
		global::Debug.LogError("No bundle exists for " + dlcId);
		return null;
	}

	public static string GetContentDirectoryName(string dlcId)
	{
		if (dlcId != null)
		{
			if (dlcId != null && dlcId.Length == 0)
			{
				return "";
			}
			if (dlcId == "EXPANSION1_ID")
			{
				return "expansion1";
			}
		}
		global::Debug.LogError("No content directory name exists for " + dlcId);
		return null;
	}

	public static string GetDlcIdFromContentDirectory(string contentDirectory)
	{
		if (contentDirectory != null)
		{
			if (contentDirectory != null && contentDirectory.Length == 0)
			{
				return "";
			}
			if (contentDirectory == "expansion1")
			{
				return "EXPANSION1_ID";
			}
		}
		global::Debug.LogError("No dlcId matches content directory " + contentDirectory);
		return null;
	}

	public static void ToggleDLC(string id)
	{
		DlcManager.SetContentSettingEnabled(id, !DlcManager.IsContentSettingEnabled(id));
	}

	public static bool IsContentActive(string dlcId)
	{
		return DlcManager.CheckPlatformSubscription(dlcId) && DlcManager.IsContentSettingEnabled(dlcId);
	}

	public static bool IsDlcListValidForCurrentContent(string[] dlcIds)
	{
		if (DlcManager.GetHighestActiveDlcId() == "")
		{
			return Array.IndexOf<string>(dlcIds, "") != -1;
		}
		foreach (string text in dlcIds)
		{
			if (!(text == "") && DlcManager.IsContentActive(text))
			{
				return true;
			}
		}
		return false;
	}

	public static string GetHighestActiveDlcId()
	{
		for (int i = DlcManager.RELEASE_ORDER.Count - 1; i >= 0; i--)
		{
			string text = DlcManager.RELEASE_ORDER[i];
			if (DlcManager.CheckPlatformSubscription(text) && DlcManager.IsContentSettingEnabled(text))
			{
				return text;
			}
		}
		return "";
	}

	private static string GetInstalledDlcId()
	{
		if (DlcManager.CheckPlatformSubscription("EXPANSION1_ID"))
		{
			return "EXPANSION1_ID";
		}
		return "";
	}

	private static bool CheckPlatformSubscription(string dlcId)
	{
		if (dlcId == null || dlcId == "")
		{
			return true;
		}
		if (Application.isEditor && (!DlcManager.IsMainThread || !Application.isPlaying))
		{
			return true;
		}
		bool flag;
		if (!DlcManager.dlcSubscribedCache.TryGetValue(dlcId, out flag))
		{
			flag = DistributionPlatform.Inst.IsDLCSubscribed(dlcId);
			DlcManager.dlcSubscribedCache[dlcId] = flag;
		}
		return flag;
	}

	private static bool IsContentSettingEnabled(string dlcId)
	{
		return dlcId == null || dlcId == "" || (DlcManager.CheckPlatformSubscription(dlcId) && KPlayerPrefs.GetInt(dlcId + ".ENABLED", 1) == 1);
	}

	private static bool IsContentOwned(string dlcId)
	{
		if (DlcManager.IsVanillaId(dlcId))
		{
			return true;
		}
		bool flag;
		if (!DlcManager.dlcPurchasedCache.TryGetValue(dlcId, out flag))
		{
			flag = DistributionPlatform.Inst.IsDLCPurchased(dlcId);
			DlcManager.dlcPurchasedCache[dlcId] = flag;
		}
		return flag;
	}

	public static List<string> GetOwnedDLCIds()
	{
		List<string> list = new List<string>();
		for (int i = DlcManager.RELEASE_ORDER.Count - 1; i >= 0; i--)
		{
			string text = DlcManager.RELEASE_ORDER[i];
			if (!DlcManager.IsVanillaId(text) && DlcManager.IsContentOwned(text))
			{
				list.Add(text);
			}
		}
		return list;
	}

	public static List<string> GetActiveDLCIds()
	{
		List<string> list = new List<string>();
		for (int i = DlcManager.RELEASE_ORDER.Count - 1; i >= 0; i--)
		{
			string text = DlcManager.RELEASE_ORDER[i];
			if (!DlcManager.IsVanillaId(text) && DlcManager.IsContentActive(text))
			{
				list.Add(text);
			}
		}
		return list;
	}

	public static string GetContentLetter(string dlcId)
	{
		if (dlcId != null)
		{
			if (dlcId != null && dlcId.Length == 0)
			{
				return "V";
			}
			if (dlcId == "EXPANSION1_ID")
			{
				return "S";
			}
		}
		global::Debug.LogError("No content letter exists for " + dlcId);
		return null;
	}

	public static string GetActiveContentLetters()
	{
		if (DlcManager.IsPureVanilla())
		{
			return DlcManager.GetContentLetter("");
		}
		string text = "";
		for (int i = 0; i < DlcManager.RELEASE_ORDER.Count; i++)
		{
			string text2 = DlcManager.RELEASE_ORDER[i];
			if (!DlcManager.IsVanillaId(text2) && DlcManager.IsContentActive(text2))
			{
				text += DlcManager.GetContentLetter(text2);
			}
		}
		return text;
	}

	private static void SetContentSettingEnabled(string dlcId, bool enabled)
	{
		global::Debug.Assert(dlcId != "", "There is no KPlayerPrefs value for vanilla - it is always enabled");
		bool flag = Application.isEditor || DistributionPlatform.Inst.IsDLCPurchased(dlcId);
		if (enabled && !flag)
		{
			return;
		}
		DlcManager.dlcPurchasedCache.Clear();
		DlcManager.dlcSubscribedCache.Clear();
		KPlayerPrefs.SetInt(dlcId + ".ENABLED", enabled ? 1 : 0);
		if (enabled && !DlcManager.CheckPlatformSubscription(dlcId))
		{
			global::Debug.Log("ToggleDLCSubscription");
			DistributionPlatform.Inst.ToggleDLCSubscription(dlcId);
			return;
		}
		if (App.instance)
		{
			global::Debug.Log("Restart");
			App.instance.Restart();
		}
	}

	public static bool IsPureVanilla()
	{
		return !DlcManager.IsExpansion1Active();
	}

	public static bool IsExpansion1Installed()
	{
		return DlcManager.CheckPlatformSubscription("EXPANSION1_ID");
	}

	public static bool IsExpansion1Active()
	{
		return DlcManager.IsContentActive("EXPANSION1_ID");
	}

	public static bool FeatureRadiationEnabled()
	{
		return DlcManager.IsExpansion1Active();
	}

	public static bool FeaturePlantMutationsEnabled()
	{
		return DlcManager.IsExpansion1Active();
	}

	public static bool FeatureClusterSpaceEnabled()
	{
		return DlcManager.IsExpansion1Active();
	}

	[ThreadStatic]
	public static readonly bool IsMainThread = true;

	public const string VANILLA_ID = "";

	public const string EXPANSION1_ID = "EXPANSION1_ID";

	public const string VANILLA_DIRECTORY = "";

	public const string EXPANSION1_DIRECTORY = "expansion1";

	public static readonly string[] AVAILABLE_VANILLA_ONLY = new string[] { "" };

	public static readonly string[] AVAILABLE_EXPANSION1_ONLY = new string[] { "EXPANSION1_ID" };

	public static readonly string[] AVAILABLE_ALL_VERSIONS = new string[] { "", "EXPANSION1_ID" };

	public static List<string> RELEASE_ORDER = new List<string> { "", "EXPANSION1_ID" };

	private static Dictionary<string, bool> dlcPurchasedCache = new Dictionary<string, bool>();

	private static Dictionary<string, bool> dlcSubscribedCache = new Dictionary<string, bool>();
}
