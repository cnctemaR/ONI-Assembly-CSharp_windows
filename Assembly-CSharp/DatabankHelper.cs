using System;
using STRINGS;
using UnityEngine;

public abstract class DatabankHelper
{
	public static string ID
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return "OrbitalResearchDatabank";
			}
			return "ResearchDatabank";
		}
	}

	public static Tag TAG
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return OrbitalResearchDatabankConfig.TAG;
			}
			return ResearchDatabankConfig.TAG;
		}
	}

	public static string RESEARCH_NAME
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return RESEARCH.TYPES.ORBITAL.NAME;
			}
			return RESEARCH.TYPES.GAMMA.NAME;
		}
	}

	public static string RESEARCH_CODEXID
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return "RESEARCHDLC1";
			}
			return "RESEARCH";
		}
	}

	public static string NAME
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.NAME;
			}
			return ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.NAME;
		}
	}

	public static string NAME_PLURAL
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.NAME_PLURAL;
			}
			return ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.NAME_PLURAL;
		}
	}

	public static string DESC
	{
		get
		{
			if (DlcManager.IsExpansion1Active())
			{
				return ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.DESC;
			}
			return ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.DESC;
		}
	}

	public static Sprite SPRITE
	{
		get
		{
			return Assets.GetSprite("ui_databank");
		}
	}

	public const string CODEXID = "Databank";
}
