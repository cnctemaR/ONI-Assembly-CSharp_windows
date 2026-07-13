using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using UnityEngine;

public class DevToolFuzzy : DevTool
{
	public DevToolFuzzy()
	{
		this.mostRecentEditTime = Time.unscaledTime;
	}

	private void RecipesUi(StringBuilder sb, string id, List<SearchUtil.NameDescCache> recipes)
	{
		int num = 0;
		foreach (SearchUtil.NameDescCache nameDescCache in recipes)
		{
			if (nameDescCache.Score > num)
			{
				num = nameDescCache.Score;
			}
		}
		if (!this.IsPassingScore(num))
		{
			return;
		}
		sb.Clear();
		sb.AppendFormat("[{0}] Recipes##{1}", num, id);
		if (ImGui.CollapsingHeader(sb.ToString()))
		{
			ImGui.Indent();
			foreach (SearchUtil.NameDescCache nameDescCache2 in recipes)
			{
				if (this.IsPassingScore(nameDescCache2.Score))
				{
					sb.Clear();
					sb.AppendFormat("{0}##{1}", DevToolFuzzy.FormatScoreDisplay(nameDescCache2.Score, nameDescCache2.name.text), id);
					if (ImGui.CollapsingHeader(sb.ToString()))
					{
						this.DisplayIfScorePasses(nameDescCache2);
					}
				}
			}
			ImGui.Unindent();
		}
	}

	private void TechItemUi(StringBuilder sb, string id, SearchUtil.TechItemCache techItem, SearchUtil.TechCache parentTech = null)
	{
		if (!this.IsPassingScore(techItem.Score))
		{
			return;
		}
		sb.Clear();
		sb.AppendFormat("{0}##TechItem{1}", DevToolFuzzy.FormatScoreDisplay(techItem.Score, techItem.nameDescSearchTerms.nameDesc.name.text), id);
		string text = sb.ToString();
		if (ImGui.CollapsingHeader(text))
		{
			ImGui.Indent();
			if (parentTech != null)
			{
				ImGui.LabelText("Parent Tech", parentTech.tech.nameDesc.name.text);
			}
			this.DisplayIfScorePasses(techItem.nameDescSearchTerms);
			this.RecipesUi(sb, text, techItem.recipes);
			ImGui.Unindent();
		}
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (ImGui.InputText("Search Text", ref this.searchText, 30U))
		{
			this.refresh = true;
			this.mostRecentEditTime = Time.unscaledTime;
		}
		if (this.refresh && Time.unscaledTime - this.mostRecentEditTime > 0.4f)
		{
			this.Refresh();
			this.refresh = false;
		}
		ImGui.DragInt("Score Threshold", ref this.scoreThreshold, 0.5f, 0, 100);
		StringBuilder stringBuilder = new StringBuilder();
		if (ImGui.CollapsingHeader("Techs"))
		{
			ImGui.Indent();
			foreach (SearchUtil.TechCache techCache in this.techs)
			{
				if (this.IsPassingScore(techCache.Score))
				{
					stringBuilder.Clear();
					stringBuilder.AppendFormat("{0}##Tech", DevToolFuzzy.FormatScoreDisplay(techCache.Score, techCache.tech.nameDesc.name.text));
					string text = stringBuilder.ToString();
					if (ImGui.CollapsingHeader(text))
					{
						ImGui.Indent();
						this.DisplayIfScorePasses(techCache.tech);
						foreach (KeyValuePair<string, SearchUtil.TechItemCache> keyValuePair in techCache.techItems)
						{
							this.TechItemUi(stringBuilder, text, keyValuePair.Value, null);
						}
						ImGui.Unindent();
					}
				}
			}
			ImGui.Unindent();
		}
		if (ImGui.CollapsingHeader("TechItems"))
		{
			ImGui.Indent();
			foreach (SearchUtil.TechCache techCache2 in this.techs)
			{
				foreach (KeyValuePair<string, SearchUtil.TechItemCache> keyValuePair2 in techCache2.techItems)
				{
					this.TechItemUi(stringBuilder, "TechItem", keyValuePair2.Value, techCache2);
				}
			}
			ImGui.Unindent();
		}
		if (ImGui.CollapsingHeader("BuildingDefs"))
		{
			ImGui.Indent();
			foreach (SearchUtil.BuildingDefCache buildingDefCache in this.buildingDefs)
			{
				if (this.IsPassingScore(buildingDefCache.Score))
				{
					stringBuilder.Clear();
					stringBuilder.AppendFormat("{0}##BuildingDef", DevToolFuzzy.FormatScoreDisplay(buildingDefCache.Score, buildingDefCache.nameDescSearchTerms.nameDesc.name.text));
					string text2 = stringBuilder.ToString();
					if (ImGui.CollapsingHeader(text2))
					{
						ImGui.Indent();
						this.DisplayIfScorePasses(buildingDefCache.nameDescSearchTerms);
						this.DisplayIfScorePasses("Effect", buildingDefCache.effect);
						this.RecipesUi(stringBuilder, text2, buildingDefCache.recipes);
						ImGui.Unindent();
					}
				}
			}
			ImGui.Unindent();
		}
	}

	private void Refresh()
	{
		string text = this.searchText.ToUpper().Trim();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (this.techs.Count == 0)
		{
			foreach (KeyValuePair<string, SearchUtil.TechCache> keyValuePair in SearchUtil.CacheTechs())
			{
				this.techs.Add(keyValuePair.Value);
			}
		}
		foreach (SearchUtil.TechCache techCache in this.techs)
		{
			techCache.Bind(text);
		}
		this.techs.Sort();
		if (this.buildingDefs.Count == 0)
		{
			foreach (BuildingDef buildingDef in Assets.BuildingDefs)
			{
				this.buildingDefs.Add(SearchUtil.MakeBuildingDefCache(buildingDef));
			}
		}
		foreach (SearchUtil.BuildingDefCache buildingDefCache in this.buildingDefs)
		{
			buildingDefCache.Bind(text);
		}
		this.buildingDefs.Sort();
	}

	private bool IsPassingScore(int score)
	{
		return score >= this.scoreThreshold;
	}

	private static string FormatScoreDisplay(int score, string text)
	{
		return string.Format("[{0}] {1}", score, FuzzySearch.Canonicalize(text));
	}

	private static void DisplayScore(int score, string label, string token, string text)
	{
		ImGui.Text(string.Format("[{0}]", score));
		ImGui.SameLine();
		ImGui.Text(label);
		ImGui.SameLine();
		ImGui.Text(string.Format("({0})", token));
		ImGui.SameLine();
		ImGui.TextWrapped(text);
	}

	private static void DisplayScore(string label, SearchUtil.MatchCache match)
	{
		DevToolFuzzy.DisplayScore(match.Score, label, match.FuzzyMatch.token, match.text);
	}

	private void DisplayIfScorePasses(string label, SearchUtil.MatchCache match)
	{
		if (this.IsPassingScore(match.Score))
		{
			DevToolFuzzy.DisplayScore(label, match);
		}
	}

	private void DisplayIfScorePasses(SearchUtil.NameDescCache nameDesc)
	{
		this.DisplayIfScorePasses("Name", nameDesc.name);
		this.DisplayIfScorePasses("Desc", nameDesc.desc);
	}

	private void DisplayIfScorePasses(SearchUtil.NameDescSearchTermsCache nameDescSearchTerms)
	{
		this.DisplayIfScorePasses(nameDescSearchTerms.nameDesc);
		if (this.IsPassingScore(nameDescSearchTerms.SearchTermsScore.score))
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendJoin<string>(", ", nameDescSearchTerms.searchTerms);
			DevToolFuzzy.DisplayScore(nameDescSearchTerms.SearchTermsScore.score, "SearchTerms", nameDescSearchTerms.SearchTermsScore.token, stringBuilder.ToString());
		}
	}

	private string searchText = "";

	private float mostRecentEditTime;

	private bool refresh;

	private const float REFRESH_DELAY = 0.4f;

	private int scoreThreshold = 79;

	private readonly List<SearchUtil.TechCache> techs = new List<SearchUtil.TechCache>();

	private readonly List<SearchUtil.BuildingDefCache> buildingDefs = new List<SearchUtil.BuildingDefCache>();
}
