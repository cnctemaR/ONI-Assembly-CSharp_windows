using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ResearchTypes
{
	public ResearchTypes()
	{
		ResearchType researchType = new ResearchType(ResearchTypes.ID.ALPHA, RESEARCH.TYPES.ALPHA.NAME.text, RESEARCH.TYPES.ALPHA.DESC.text, new Color(0.59607846f, 0.6666667f, 0.9137255f), new Recipe.Ingredient[]
		{
			new Recipe.Ingredient("Dirt".ToTag(), 100f)
		}, 600f, "research_center_kanim", new string[] { "ResearchCenter" }, RESEARCH.TYPES.ALPHA.RECIPEDESC);
		this.Types.Add(researchType);
		ResearchType researchType2 = new ResearchType(ResearchTypes.ID.BETA, RESEARCH.TYPES.BETA.NAME.text, RESEARCH.TYPES.BETA.DESC.text, new Color(0.6f, 0.38431373f, 0.5686275f), new Recipe.Ingredient[]
		{
			new Recipe.Ingredient("Water".ToTag(), 25f)
		}, 1200f, "research_center_kanim", new string[] { "AdvancedResearchCenter" }, RESEARCH.TYPES.BETA.RECIPEDESC);
		this.Types.Add(researchType2);
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
		Debug.LogWarning(string.Format("No research with type id {0} found", id));
		return null;
	}

	public List<ResearchType> Types = new List<ResearchType>();

	public class ID
	{
		public static string ALPHA = "alpha";

		public static string BETA = "beta";
	}
}
