using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ResearchTypes
{
	public ResearchTypes()
	{
		ResearchType researchType = new ResearchType("basic", RESEARCH.TYPES.ALPHA.NAME, RESEARCH.TYPES.ALPHA.DESC, Assets.GetSprite("research_type_alpha_icon"), new Color(0.59607846f, 0.6666667f, 0.9137255f), new Recipe.Ingredient[]
		{
			new Recipe.Ingredient("Dirt".ToTag(), 100f)
		}, 600f, "research_center_kanim", new string[] { "ResearchCenter" }, RESEARCH.TYPES.ALPHA.RECIPEDESC);
		this.Types.Add(researchType);
		ResearchType researchType2 = new ResearchType("advanced", RESEARCH.TYPES.BETA.NAME, RESEARCH.TYPES.BETA.DESC, Assets.GetSprite("research_type_beta_icon"), new Color(0.6f, 0.38431373f, 0.5686275f), new Recipe.Ingredient[]
		{
			new Recipe.Ingredient("Water".ToTag(), 25f)
		}, 1200f, "research_center_kanim", new string[] { "AdvancedResearchCenter" }, RESEARCH.TYPES.BETA.RECIPEDESC);
		this.Types.Add(researchType2);
		ResearchType researchType3 = new ResearchType("space", RESEARCH.TYPES.GAMMA.NAME, RESEARCH.TYPES.GAMMA.DESC, Assets.GetSprite("research_type_gamma_icon"), new Color32(240, 141, 44, byte.MaxValue), null, 2400f, "research_center_kanim", new string[] { "CosmicResearchCenter" }, RESEARCH.TYPES.GAMMA.RECIPEDESC);
		this.Types.Add(researchType3);
		ResearchType researchType4 = new ResearchType("nuclear", RESEARCH.TYPES.DELTA.NAME, RESEARCH.TYPES.DELTA.DESC, Assets.GetSprite("research_type_delta_icon"), new Color32(231, 210, 17, byte.MaxValue), null, 2400f, "research_center_kanim", new string[] { "NuclearResearchCenter" }, RESEARCH.TYPES.DELTA.RECIPEDESC);
		this.Types.Add(researchType4);
		ResearchType researchType5 = new ResearchType("orbital", RESEARCH.TYPES.ORBITAL.NAME, RESEARCH.TYPES.ORBITAL.DESC, Assets.GetSprite("research_type_orbital_icon"), new Color32(240, 141, 44, byte.MaxValue), null, 2400f, "research_center_kanim", new string[] { "OrbitalResearchCenter", "DLC1CosmicResearchCenter" }, RESEARCH.TYPES.ORBITAL.RECIPEDESC);
		this.Types.Add(researchType5);
	}

	public ResearchType GetResearchType(string id)
	{
		foreach (ResearchType researchType in this.Types)
		{
			if (id == researchType.id)
			{
				return researchType;
			}
		}
		global::Debug.LogWarning(string.Format("No research with type id {0} found", id));
		return null;
	}

	public List<ResearchType> Types = new List<ResearchType>();

	public class ID
	{
		public const string BASIC = "basic";

		public const string ADVANCED = "advanced";

		public const string SPACE = "space";

		public const string NUCLEAR = "nuclear";

		public const string ORBITAL = "orbital";
	}
}
