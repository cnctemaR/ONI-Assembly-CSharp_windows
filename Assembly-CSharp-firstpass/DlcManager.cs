using System;
using System.Collections.Generic;

public class DlcManager
{
	public static bool IsAheadOfInstalledDlc(string dlcId)
	{
		return DlcManager.IsAheadOf(dlcId, DlcManager.GetInstalledDlcId());
	}

	public static bool IsVanillaId(string dlcId)
	{
		return dlcId == null || dlcId == "";
	}

	public static string GetContentBundleName(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return "expansion1_bundle";
		}
		Debug.LogError("No bundle exists for " + dlcId);
		return null;
	}

	public static string GetContentDirectoryName(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return "expansion1";
		}
		Debug.LogError("No content directory name exists for " + dlcId);
		return null;
	}

	public static bool IsExpansion1Installed()
	{
		return false;
	}

	public static bool IsContentInstalled(string dlcId)
	{
		if (dlcId == null || (dlcId != null && dlcId.Length == 0))
		{
			return true;
		}
		if (!(dlcId == "EXPANSION1_ID"))
		{
			DebugUtil.LogErrorArgs(new object[] { "Invalid dlcId:", dlcId });
			return false;
		}
		return DlcManager.IsExpansion1Installed();
	}

	public static void SetExpansion1Active(bool active)
	{
		Debug.Assert(false, "Do not call this in the trunk version of the game!");
	}

	public static bool IsExpansion1Active()
	{
		return DlcManager.IsContentActive("EXPANSION1_ID");
	}

	public static bool IsContentActive(string dlcId)
	{
		return !DlcManager.IsAheadOf(dlcId, DlcManager.GetActiveDlcId());
	}

	public static string GetActiveDlcId()
	{
		if (DlcManager.IsExpansion1Installed() && DlcManager.IsContentSettingEnabled("EXPANSION1_ID"))
		{
			return "EXPANSION1_ID";
		}
		return "";
	}

	private static string GetInstalledDlcId()
	{
		if (DlcManager.IsExpansion1Installed())
		{
			return "EXPANSION1_ID";
		}
		return "";
	}

	private static bool IsAheadOf(string dlcIdA, string dlcIdB)
	{
		if (dlcIdA == null)
		{
			dlcIdA = "";
		}
		if (dlcIdB == null)
		{
			dlcIdB = "";
		}
		int num = DlcManager.RELEASE_ORDER.IndexOf(dlcIdA);
		Debug.Assert(num != -1, string.Format("Invalid dlcIdA: {0}", dlcIdA));
		int num2 = DlcManager.RELEASE_ORDER.IndexOf(dlcIdB);
		Debug.Assert(num2 != -1, string.Format("Invalid dlcIdB: {0}", dlcIdB));
		return num > num2;
	}

	private static bool IsContentSettingEnabled(string dlcId)
	{
		Debug.Assert(dlcId != "", "There is no KPlayerPrefs value for vanilla - it is always enabled");
		return false;
	}

	private static void SetContentSettingEnabled(string dlcId, bool enabled)
	{
		Debug.Assert(false, "Don't call this in the trunk version of the game!");
	}

	public const string PACK1_ID = "PACK1";

	public const string VANILLA_ID = "";

	public const string EXPANSION1_ID = "EXPANSION1_ID";

	public static List<string> RELEASE_ORDER = new List<string> { "", "EXPANSION1_ID" };
}
