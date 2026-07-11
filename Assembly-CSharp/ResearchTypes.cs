using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ResearchTypes
{
	public ResearchTypes()
	{
		ResearchType researchType = new ResearchType("alpha", RESEARCH.TYPES.ALPHA.NAME, RESEARCH.TYPES.ALPHA.DESC, Assets.GetSprite("research_type_alpha_icon"), new Color(0.59607846f, 0.6666667f, 0.9137255f), new Recipe.Ingredient[]
		{
			new Recipe.Ingredient("Dirt".ToTag(), 100f)
		}, 600f, "research_center_kanim", new string[] { "ResearchCenter" }, RESEARCH.TYPES.ALPHA.RECIPEDESC);
		this.Types.Add(researchType);
		ResearchType researchType2 = new ResearchType("beta", RESEARCH.TYPES.BETA.NAME, RESEARCH.TYPES.BETA.DESC, Assets.GetSprite("research_type_beta_icon"), new Color(0.6f, 0.38431373f, 0.5686275f), new Recipe.Ingredient[]
		{
			new Recipe.Ingredient("Water".ToTag(), 25f)
		}, 1200f, "research_center_kanim", new string[] { "AdvancedResearchCenter" }, RESEARCH.TYPES.BETA.RECIPEDESC);
		this.Types.Add(researchType2);
		ResearchType researchType3 = new ResearchType("gamma", RESEARCH.TYPES.GAMMA.NAME, RESEARCH.TYPES.GAMMA.DESC, Assets.GetSprite("research_type_gamma_icon"), new Color32(240, 141, 44, byte.MaxValue), null, 2400f, "research_center_kanim", new string[] { "CosmicResearchCenter" }, RESEARCH.TYPES.GAMMA.RECIPEDESC);
		this.Types.Add(researchType3);
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
		public const string ALPHA = "alpha";

		public const string BETA = "beta";

		public const string GAMMA = "gamma";
	}
}
