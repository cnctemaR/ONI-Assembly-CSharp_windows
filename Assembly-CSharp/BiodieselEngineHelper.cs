using System;
using STRINGS;

internal static class BiodieselEngineHelper
{
	public static string ID
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return "BiodieselEngineCluster";
			}
			return "BiodieselEngine";
		}
	}

	public static string CODEXID
	{
		get
		{
			return BiodieselEngineHelper.ID.ToUpperInvariant();
		}
	}

	public static string NAME
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return BUILDINGS.PREFABS.BIODIESELENGINECLUSTER.NAME;
			}
			return BUILDINGS.PREFABS.BIODIESELENGINE.NAME;
		}
	}
}
