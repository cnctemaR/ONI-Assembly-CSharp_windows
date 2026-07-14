using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DlcManager
{
	public static List<string> RELEASED_VERSIONS
	{
		get
		{
			if (DlcManager.released == null)
			{
				DlcManager.released = new List<string>(DlcManager.RELEASE_ORDER);
				DlcManager.released.AddRange(DlcManager.DLC_PACKS.Keys);
			}
			return DlcManager.released;
		}
	}

	public static void ClearCachedValues()
	{
		DlcManager.dlcPurchasedCache = new Dictionary<string, bool>();
		DlcManager.dlcSubscribedCache = new Dictionary<string, bool>();
		DlcManager.dlcAssetsCache = new Dictionary<string, bool>();
	}

	public static bool IsVanillaId(string dlcId)
	{
		return dlcId == null || dlcId == "";
	}

	public static bool IsDlcId(string dlcId)
	{
		return !DlcManager.IsVanillaId(dlcId) && (dlcId == "EXPANSION1_ID" || DlcManager.DLC_PACKS.ContainsKey(dlcId));
	}

	public static bool IsUnknownDlc(string dlcId)
	{
		return !DlcManager.IsVanillaId(dlcId) && !DlcManager.IsDlcId(dlcId);
	}

	public static string GetDlcTitle(string dlcId)
	{
		StringKey stringKey = new StringKey(dlcId);
		if (dlcId == "EXPANSION1_ID")
		{
			stringKey = DlcManager.EXPANSION1_INFO.dlcTitle;
		}
		DlcManager.DlcInfo dlcInfo;
		if (DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			stringKey = dlcInfo.dlcTitle;
		}
		else if (DlcManager.IsUnknownDlc(dlcId))
		{
			stringKey = new StringKey("STRINGS.UI.UNKNOWN_DLC.NAME");
		}
		return string.Concat(new string[]
		{
			"<i><color=#",
			DlcManager.GetDlcBannerColor(dlcId).ToHexString(),
			">",
			Strings.Get(stringKey),
			"</color></i>"
		});
	}

	public static string GetDlcTitleNoFormatting(string dlcId)
	{
		StringKey stringKey = new StringKey(dlcId);
		if (dlcId == "EXPANSION1_ID")
		{
			stringKey = DlcManager.EXPANSION1_INFO.dlcTitle;
		}
		DlcManager.DlcInfo dlcInfo;
		if (DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			stringKey = dlcInfo.dlcTitle;
		}
		return Strings.Get(stringKey);
	}

	public static string GetDlcSmallLogo(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return DlcManager.EXPANSION1_INFO.smallLogo;
		}
		if (DlcManager.IsUnknownDlc(dlcId))
		{
			return "mode_segue";
		}
		DlcManager.DlcInfo dlcInfo;
		if (!DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			return "";
		}
		return dlcInfo.smallLogo;
	}

	public static string GetDlcBanner(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return DlcManager.EXPANSION1_INFO.banner;
		}
		DlcManager.DlcInfo dlcInfo;
		if (DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			return dlcInfo.banner;
		}
		DebugUtil.DevLogError("No banner exists for " + dlcId);
		return "unknown";
	}

	public static string GetDlcLargeLogo(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return DlcManager.EXPANSION1_INFO.largeLogo;
		}
		DlcManager.DlcInfo dlcInfo;
		if (!DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			return "";
		}
		return dlcInfo.largeLogo;
	}

	public static string GetDlcBannerSprite(string dlcId)
	{
		if (dlcId == null || dlcId == "" || dlcId == "")
		{
			return "research_dlc_banner";
		}
		if (dlcId.Contains("COSMETIC"))
		{
			return "cosmetics_banner";
		}
		return "research_dlc_banner";
	}

	public static bool CanPurchase(string DLCID)
	{
		DlcManager.DlcInfo dlcInfo;
		return DlcManager.DLC_PACKS.TryGetValue(DLCID, out dlcInfo) && dlcInfo.canPurchase;
	}

	public static bool CanWishlist(string DLCID)
	{
		DlcManager.DlcInfo dlcInfo;
		return DlcManager.DLC_PACKS.TryGetValue(DLCID, out dlcInfo) && dlcInfo.canWishlist;
	}

	public static Color GetDlcBannerColor(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return DlcManager.EXPANSION1_INFO.bannerColor;
		}
		DlcManager.DlcInfo dlcInfo;
		if (!DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			return Color.magenta;
		}
		return dlcInfo.bannerColor;
	}

	public static string GetMostSignificantDlc(string[] requiredDlcIds)
	{
		if (requiredDlcIds == null || requiredDlcIds.Length == 0)
		{
			return null;
		}
		return requiredDlcIds[requiredDlcIds.Length - 1];
	}

	public static bool DlcListContains(string[] list, string dlcId)
	{
		return list != null && list.Contains(dlcId);
	}

	public static string GetMostSignificantDlc(IHasDlcRestrictions restrictions)
	{
		return DlcManager.GetMostSignificantDlc(restrictions.GetRequiredDlcIds());
	}

	public static string GetContentDirectoryName(string dlcId)
	{
		if (dlcId != null && dlcId.Length == 0)
		{
			return "";
		}
		if (dlcId == "EXPANSION1_ID")
		{
			return "expansion1";
		}
		DlcManager.DlcInfo dlcInfo;
		if (DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			return dlcInfo.directory;
		}
		global::Debug.LogError("No content directory name exists for " + dlcId);
		return null;
	}

	public static string GetDlcIdFromContentDirectory(string contentDirectory)
	{
		if (contentDirectory != null && contentDirectory.Length == 0)
		{
			return "";
		}
		if (!(contentDirectory == "expansion1"))
		{
			foreach (KeyValuePair<string, DlcManager.DlcInfo> keyValuePair in DlcManager.DLC_PACKS)
			{
				if (keyValuePair.Value.directory == contentDirectory)
				{
					return keyValuePair.Key;
				}
			}
			global::Debug.LogError("No dlcId matches content directory '" + contentDirectory + "'");
			return null;
		}
		return "EXPANSION1_ID";
	}

	public static void ToggleDLC(string id)
	{
		DebugUtil.Assert(id == "" || id == "EXPANSION1_ID", "Toggling DLC is only valid for vanilla or expansion1");
		DlcManager.SetContentSettingEnabled(id, !DlcManager.IsContentSettingEnabled(id));
	}

	public static bool ShouldLoadDLCAssets(string dlcId)
	{
		return DlcManager.CheckPlatformSubscription(dlcId);
	}

	public static bool IsContentSubscribed(string dlcId)
	{
		return DlcManager.CheckPlatformSubscription(dlcId) && DlcManager.IsContentSettingEnabled(dlcId);
	}

	public static bool IsAllContentSubscribed(List<string> dlcIds)
	{
		if (dlcIds == null)
		{
			return true;
		}
		using (List<string>.Enumerator enumerator = dlcIds.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!DlcManager.IsContentSubscribed(enumerator.Current))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool IsAllContentSubscribed(string[] dlcIds)
	{
		if (dlcIds == null)
		{
			return true;
		}
		for (int i = 0; i < dlcIds.Length; i++)
		{
			if (!DlcManager.IsContentSubscribed(dlcIds[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsAnyContentSubscribed(string[] dlcIds)
	{
		if (dlcIds == null)
		{
			return false;
		}
		for (int i = 0; i < dlcIds.Length; i++)
		{
			if (DlcManager.IsContentSubscribed(dlcIds[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsCorrectDlcSubscribed(string[] required, string[] forbidden)
	{
		return DlcManager.IsAllContentSubscribed(required) && !DlcManager.IsAnyContentSubscribed(forbidden);
	}

	public static bool IsCorrectDlcSubscribed(IHasDlcRestrictions restrictions)
	{
		return (restrictions.GetAnyRequiredDlcIds() == null || DlcManager.IsAnyContentSubscribed(restrictions.GetAnyRequiredDlcIds())) && DlcManager.IsAllContentSubscribed(restrictions.GetRequiredDlcIds()) && !DlcManager.IsAnyContentSubscribed(restrictions.GetForbiddenDlcIds());
	}

	public static void ConvertAvailableToRequireAndForbidden(string[] dlcIds, out string[] requiredDlcIds, out string[] forbiddenDlcIds)
	{
		requiredDlcIds = null;
		forbiddenDlcIds = null;
		if (dlcIds == null)
		{
			return;
		}
		if (dlcIds.SequenceEqual<string>(DlcManager.AVAILABLE_DLC_3))
		{
			requiredDlcIds = DlcManager.DLC3;
			return;
		}
		if (dlcIds.SequenceEqual<string>(DlcManager.AVAILABLE_EXPANSION1_ONLY))
		{
			requiredDlcIds = DlcManager.EXPANSION1;
			return;
		}
		if (dlcIds.SequenceEqual<string>(DlcManager.AVAILABLE_VANILLA_ONLY))
		{
			forbiddenDlcIds = DlcManager.EXPANSION1;
			return;
		}
		if (dlcIds.SequenceEqual<string>(DlcManager.AVAILABLE_DLC_2))
		{
			requiredDlcIds = DlcManager.DLC2;
			return;
		}
		if (dlcIds.SequenceEqual<string>(DlcManager.AVAILABLE_ALL_VERSIONS))
		{
			requiredDlcIds = null;
			forbiddenDlcIds = null;
			return;
		}
		DebugUtil.DevLogError("ConvertAvailableToRequireAndForbidden received a list it did not recognize. dlcIds='" + (',' + dlcIds) + "'");
	}

	private static bool CheckPlatformSubscription(string dlcId)
	{
		if (DlcManager.IsVanillaId(dlcId))
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
		flag = flag && DlcManager.CheckForDLCFileInstallation(dlcId);
		return flag;
	}

	private static bool CheckForExpansionFileExistence()
	{
		string text = Path.Combine(Application.streamingAssetsPath, "expansion1_bundle");
		bool flag = false;
		try
		{
			flag = File.Exists(text);
		}
		catch (Exception ex)
		{
			global::Debug.Log("[DlcManager] Error at reading file. CheckPlatformSubscription() - " + ex.Message);
		}
		return flag;
	}

	private static bool CheckForDLCFileInstallation(string dlcId)
	{
		bool flag = false;
		if (DlcManager.dlcAssetsCache.TryGetValue(dlcId, out flag))
		{
			return flag;
		}
		if (DlcManager.IsVanillaId(dlcId))
		{
			flag = true;
		}
		else if (dlcId == "EXPANSION1_ID")
		{
			flag = DlcManager.CheckForExpansionFileExistence();
		}
		DlcManager.DlcInfo dlcInfo;
		if (DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			string text = Path.Combine(Application.streamingAssetsPath, dlcInfo.bundleName);
			try
			{
				flag = File.Exists(text);
			}
			catch (Exception ex)
			{
				global::Debug.Log("[DlcManager] Error at reading file. CheckPlatformSubscription() - " + ex.Message);
			}
		}
		DlcManager.dlcAssetsCache.Add(dlcId, flag);
		return flag;
	}

	private static bool IsContentSettingEnabled(string dlcId)
	{
		if (DlcManager.IsVanillaId(dlcId))
		{
			return true;
		}
		if (!DlcManager.CheckPlatformSubscription(dlcId))
		{
			return false;
		}
		if (!DlcManager.dlcDotEnabled.ContainsKey(dlcId))
		{
			DlcManager.dlcDotEnabled[dlcId] = dlcId + ".ENABLED";
		}
		return KPlayerPrefs.GetInt(DlcManager.dlcDotEnabled[dlcId], 1) == 1;
	}

	public static bool IsContentOwned(string dlcId)
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
		for (int i = DlcManager.RELEASED_VERSIONS.Count - 1; i >= 0; i--)
		{
			string text = DlcManager.RELEASED_VERSIONS[i];
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
		for (int i = DlcManager.RELEASED_VERSIONS.Count - 1; i >= 0; i--)
		{
			string text = DlcManager.RELEASED_VERSIONS[i];
			if (!DlcManager.IsVanillaId(text) && DlcManager.IsContentSubscribed(text))
			{
				list.Add(text);
			}
		}
		return list;
	}

	public static string GetContentLetter(string dlcId)
	{
		if (dlcId != null && dlcId.Length == 0)
		{
			return "V";
		}
		if (dlcId == "EXPANSION1_ID")
		{
			return "S";
		}
		DlcManager.DlcInfo dlcInfo;
		if (DlcManager.DLC_PACKS.TryGetValue(dlcId, out dlcInfo))
		{
			return dlcInfo.versionLetter;
		}
		global::Debug.LogError("No content letter exists for " + dlcId);
		return null;
	}

	public static string GetActiveContentLetters()
	{
		string text = "";
		if (DlcManager.IsExpansion1Active())
		{
			text += DlcManager.GetContentLetter("EXPANSION1_ID");
		}
		else
		{
			text += DlcManager.GetContentLetter("");
		}
		foreach (KeyValuePair<string, DlcManager.DlcInfo> keyValuePair in DlcManager.DLC_PACKS)
		{
			string id = keyValuePair.Value.id;
			if (DlcManager.IsContentSubscribed(id))
			{
				text += DlcManager.GetContentLetter(id);
			}
		}
		return text;
	}

	public static string GetSubscribedContentLetters()
	{
		string text = "";
		if (DlcManager.IsExpansion1Active())
		{
			text += DlcManager.GetContentLetter("EXPANSION1_ID");
		}
		else
		{
			text += DlcManager.GetContentLetter("");
		}
		foreach (KeyValuePair<string, DlcManager.DlcInfo> keyValuePair in DlcManager.DLC_PACKS)
		{
			string id = keyValuePair.Value.id;
			if (DlcManager.IsContentSettingEnabled(id))
			{
				text += DlcManager.GetContentLetter(id);
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

	private static bool AreDlcListsIdentical(string[] list1, string[] list2)
	{
		bool flag = list1 == null || list1.Length == 0;
		bool flag2 = list2 == null || list2.Length == 0;
		return (flag && flag2) || (!flag && !flag2 && list1.SequenceEqual<string>(list2));
	}

	public static bool AreRestrictionsIdentical(IHasDlcRestrictions a, IHasDlcRestrictions b)
	{
		if (a.IsNullOrDestroyed() || b.IsNullOrDestroyed())
		{
			DebugUtil.LogErrorArgs(new object[] { "Trying to compare restrictions on null objects" });
		}
		return DlcManager.AreDlcListsIdentical(a.GetRequiredDlcIds(), b.GetRequiredDlcIds()) && DlcManager.AreDlcListsIdentical(a.GetForbiddenDlcIds(), b.GetForbiddenDlcIds());
	}

	public static string GetContentBundleName(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return "expansion1_bundle";
		}
		if (DlcManager.DLC_PACKS.ContainsKey(dlcId))
		{
			return DlcManager.DLC_PACKS[dlcId].bundleName;
		}
		DebugUtil.DevLogError("No bundle exists for " + dlcId);
		return dlcId;
	}

	public static bool IsPureVanilla()
	{
		return !DlcManager.IsExpansion1Active();
	}

	public static bool IsExpansion1Active()
	{
		return DlcManager.IsContentSubscribed("EXPANSION1_ID");
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

	[Obsolete("Use IsCorrectDlcSubscribed instead")]
	public static bool IsDlcListValidForCurrentContent(string[] dlcIds)
	{
		if (dlcIds == null || dlcIds.Length == 0)
		{
			return true;
		}
		if (DlcManager.GetHighestActiveDlcId() == "" && dlcIds.Contains(""))
		{
			return true;
		}
		foreach (string text in dlcIds)
		{
			if (!(text == "") && DlcManager.IsContentSubscribed(text))
			{
				return true;
			}
		}
		return false;
	}

	[Obsolete("Use DlcManager.IsContentSubscribed to check if content is loaded or SaveLoader.Instance.IsDLCActiveForCurrentSave to check if the content is available in a save. This method will be removed in the future.")]
	public static bool IsContentActive(string dlcId)
	{
		DebugUtil.LogWarningArgs(new object[] { "A mod is calling IsContentActive which is obsolete and needs to be fixed." });
		if (dlcId == "" || dlcId == "EXPANSION1_ID")
		{
			return DlcManager.IsContentSubscribed(dlcId);
		}
		DebugUtil.LogErrorArgs(new object[] { "IsContentActive was called with a newer DLC which is not allowed." });
		return false;
	}

	[Obsolete]
	public static bool IsExpansion1Installed()
	{
		return DlcManager.CheckPlatformSubscription("EXPANSION1_ID");
	}

	[Obsolete]
	public static bool IsVanillaId(string[] dlcIds)
	{
		return dlcIds == null || (dlcIds.Length == 1 && dlcIds[0] == "");
	}

	[Obsolete]
	public static bool IsValidForVanilla(string[] dlcIds)
	{
		return dlcIds == null || Array.IndexOf<string>(dlcIds, "") != -1;
	}

	[Obsolete]
	public static bool IsExpansion1Id(string dlcId)
	{
		return dlcId == "EXPANSION1_ID";
	}

	[Obsolete]
	private static string GetInstalledDlcId()
	{
		if (DlcManager.CheckPlatformSubscription("EXPANSION1_ID"))
		{
			return "EXPANSION1_ID";
		}
		return "";
	}

	[Obsolete("Use IsAllContentSubscribed")]
	public static bool HasAllContentSubscribed(List<string> dlcIds)
	{
		return DlcManager.IsAllContentSubscribed(dlcIds);
	}

	[Obsolete("Use IsAllContentSubscribed")]
	public static bool HasAllContentSubscribed(string[] dlcIds)
	{
		return DlcManager.IsAllContentSubscribed(dlcIds);
	}

	[Obsolete("Use IsAnyContentSubscribed")]
	public static bool HasAnyContentSubscribed(string[] dlcIds)
	{
		return DlcManager.IsAnyContentSubscribed(dlcIds);
	}

	[Obsolete("This should no longer be used.")]
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

	[ThreadStatic]
	private static readonly bool IsMainThread = true;

	public const string VANILLA_ID = "";

	public const string EXPANSION1_ID = "EXPANSION1_ID";

	public const string DLC2_ID = "DLC2_ID";

	public const string DLC3_ID = "DLC3_ID";

	public const string DLC4_ID = "DLC4_ID";

	public const string DLC5_ID = "DLC5_ID";

	public const string COSMETIC1_ID = "COSMETIC1_ID";

	public static readonly string[] EXPANSION1 = new string[] { "EXPANSION1_ID" };

	public static readonly string[] DLC2 = new string[] { "DLC2_ID" };

	public static readonly string[] DLC3 = new string[] { "DLC3_ID" };

	public static readonly string[] DLC4 = new string[] { "DLC4_ID" };

	public static readonly string[] DLC5 = new string[] { "DLC5_ID" };

	public const string EXPANSION1_VERIFICATION_FILE_NAME = "expansion1_bundle";

	public const string VANILLA_DIRECTORY = "";

	public const string EXPANSION1_DIRECTORY = "expansion1";

	public static readonly string[] COSMETIC1 = new string[] { "COSMETIC1_ID" };

	public static List<string> CONTENT_ONLY_DLC_IDS = new List<string> { "COSMETIC1_ID" };

	public static List<string> RELEASE_ORDER = new List<string> { "", "EXPANSION1_ID" };

	public static DlcManager.DlcInfo EXPANSION1_INFO = new DlcManager.DlcInfo("EXPANSION1_ID", "S", new Color(1f, 0.79607844f, 0.003921569f), "https://store.steampowered.com/app/1452490/Oxygen_Not_Included__Spaced_Out/", "https://store.epicgames.com/en-US/p/oxygen-not-included--spaced-out", "https://www.wegame.com.cn/store/2001539/", false, null, 144, true, true, true);

	public static Dictionary<string, DlcManager.DlcInfo> DLC_PACKS = new Dictionary<string, DlcManager.DlcInfo>
	{
		{
			"DLC2_ID",
			new DlcManager.DlcInfo("DLC2_ID", "C", new Color(0.003921569f, 0.73333335f, 1f), "https://store.steampowered.com/app/2952300/Oxygen_Not_Included_The_Frosty_Planet_Pack/", "https://store.epicgames.com/p/oxygen-not-included-oxygen-not-included-the-frosty-planet-pack-915ba1", "https://www.wegame.com.cn/store/2002196/", false, null, 144, true, true, true)
		},
		{
			"DLC3_ID",
			new DlcManager.DlcInfo("DLC3_ID", "R", new Color(0.79607844f, 0.3882353f, 0.95686275f), "https://store.steampowered.com/app/3302470/Oxygen_Not_Included_The_Bionic_Booster_Pack/", "https://store.epicgames.com/p/oxygen-not-included-oxygen-not-included-the-bionic-booster-pack-3ba9e9", "https://www.wegame.com.cn/store/2002347", false, null, 144, true, true, true)
		},
		{
			"DLC4_ID",
			new DlcManager.DlcInfo("DLC4_ID", "P", new Color(0.3137255f, 0.6745098f, 0.31764707f), "https://store.steampowered.com/app/3655420/Oxygen_Not_Included_The_Prehistoric_Planet_Pack/", "https://store.epicgames.com/p/oxygen-not-included-oxygen-not-included-the-prehistoric-planet-pack-c14f10", "https://www.wegame.com.cn/store/2002496", false, null, 172, true, true, true)
		},
		{
			"DLC5_ID",
			new DlcManager.DlcInfo("DLC5_ID", "A", new Color(0.09803922f, 0.63529414f, 0.5803922f), "https://store.steampowered.com/app/4310080/Oxygen_Not_Included_The_Aquatic_Planet_Pack/", "https://store.epicgames.com/p/oxygen-not-included-oxygen-not-included-the-aquatic-planet-pack-cd3a3b", "https://www.wegame.com.cn/store/2002763", false, null, 210, true, true, false)
		},
		{
			"COSMETIC1_ID",
			new DlcManager.DlcInfo("COSMETIC1_ID", "N", new Color(0.43529412f, 0.43529412f, 0.75686276f), "https://store.steampowered.com/app/4157740/Oxygen_Not_Included_Neutronium_Cosmetics_Pack/", "https://store.epicgames.com/en-US/p/oxygen-not-included-oxygen-not-included-neutronium-cosmetics-pack-d9e8af", "https://www.wegame.com.cn/store/2002628", true, "cosmetics", 144, true, true, true)
		}
	};

	private static List<string> released = null;

	private static Dictionary<string, bool> dlcPurchasedCache = new Dictionary<string, bool>();

	private static Dictionary<string, bool> dlcSubscribedCache = new Dictionary<string, bool>();

	private static Dictionary<string, bool> dlcAssetsCache = new Dictionary<string, bool>();

	private static Dictionary<string, string> dlcDotEnabled = new Dictionary<string, string>();

	[Obsolete("Use required forbidden instead of available")]
	public static readonly string[] AVAILABLE_VANILLA_ONLY = new string[] { "" };

	[Obsolete("Use required forbidden instead of available")]
	public static readonly string[] AVAILABLE_EXPANSION1_ONLY = new string[] { "EXPANSION1_ID" };

	[Obsolete("Use required forbidden instead of available")]
	public static readonly string[] AVAILABLE_DLC_2 = new string[] { "DLC2_ID" };

	[Obsolete("Use required forbidden instead of available")]
	public static readonly string[] AVAILABLE_DLC_3 = new string[] { "DLC3_ID" };

	[Obsolete("Use required forbidden instead of available")]
	public static readonly string[] AVAILABLE_ALL_VERSIONS = new string[] { "", "EXPANSION1_ID" };

	public struct DlcInfo
	{
		public DlcInfo(string dlcName, string versionLetter, Color bannerColor, string steamUrl, string epicUrl, string railUrl, bool isCosmetic = false, string bannerOverride = null, int mainMenuLogoWidth = 144, bool isReleased = true, bool canWishlist = true, bool canPurchase = true)
		{
			string text = dlcName.Replace("_ID", "").ToLowerInvariant();
			this.id = dlcName;
			this.versionLetter = versionLetter;
			this.bannerColor = bannerColor;
			this.isCosmetic = isCosmetic;
			this.storeUrl = steamUrl;
			this.mainMenuLogoWidth = mainMenuLogoWidth;
			this.banner = bannerOverride ?? (text + "_banner");
			this.bundleName = text + "_bundle";
			this.directory = text;
			this.smallLogo = text + "_mini_logo";
			this.largeLogo = text + "_logo";
			this.dlcTitle = new StringKey("STRINGS.UI." + text.ToUpper() + ".NAME");
			this.canWishlist = canWishlist;
			this.canPurchase = canPurchase;
		}

		public string id;

		public string bundleName;

		public string versionLetter;

		public string directory;

		public string storeUrl;

		public string smallLogo;

		public string largeLogo;

		public string banner;

		public Color bannerColor;

		public StringKey dlcTitle;

		public bool isCosmetic;

		public int mainMenuLogoWidth;

		public bool canWishlist;

		public bool canPurchase;
	}
}
