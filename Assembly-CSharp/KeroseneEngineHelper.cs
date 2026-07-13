using System;
using STRINGS;

internal static class KeroseneEngineHelper
{
	public static string ID
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return "KeroseneEngineCluster";
			}
			return "KeroseneEngine";
		}
	}

	public static string CODEXID
	{
		get
		{
			return KeroseneEngineHelper.ID.ToUpperInvariant();
		}
	}

	public static string NAME
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return BUILDINGS.PREFABS.KEROSENEENGINECLUSTER.NAME;
			}
			return BUILDINGS.PREFABS.KEROSENEENGINE.NAME;
		}
	}
}
