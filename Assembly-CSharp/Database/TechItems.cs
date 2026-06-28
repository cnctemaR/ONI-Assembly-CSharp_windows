using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class TechItems : ResourceSet<TechItem>
	{
		public TechItems(ResourceSet parent)
			: base("TechItems", parent)
		{
			this.automationOverlay = this.AddTechItem("AutomationOverlay", RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.DESC, this.GetSpriteFnBuilder("overlay_logic"));
			this.suitsOverlay = this.AddTechItem("SuitsOverlay", RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.DESC, this.GetSpriteFnBuilder("overlay_suit"));
			this.betaResearchPoint = this.AddTechItem("BetaResearchPoint", RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.NAME, RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.DESC, this.GetSpriteFnBuilder("research_type_beta_icon"));
		}

		private Func<string, Sprite> GetSpriteFnBuilder(string spriteName)
		{
			return (string anim) => Assets.GetSprite(spriteName);
		}

		public TechItem AddTechItem(string id, string name, string description, Func<string, Sprite> getUISprite)
		{
			TechItem techItem;
			if (base.TryGet(id) != null)
			{
				Output.LogWarning(new object[] { "Tried adding a tech item called", id, name, "but it was already added!" });
				techItem = base.Get(id);
			}
			else
			{
				Tech tech = this.LookupGroupForID(id);
				if (tech == null)
				{
					techItem = null;
				}
				else
				{
					TechItem techItem2 = new TechItem(id, this, name, description, getUISprite, tech);
					base.Add(techItem2);
					tech.unlockedItems.Add(techItem2);
					techItem = techItem2;
				}
			}
			return techItem;
		}

		public bool IsTechItemComplete(string id)
		{
			foreach (TechItem techItem in this)
			{
				if (techItem.Id == id)
				{
					return techItem.IsComplete();
				}
			}
			return true;
		}

		public bool IsTechItemAvailable(string id)
		{
			foreach (TechItem techItem in this)
			{
				if (techItem.Id == id)
				{
					return techItem.IsComplete() || techItem.parentTech.ArePrerequisitesComplete();
				}
			}
			return true;
		}

		private Tech LookupGroupForID(string itemID)
		{
			foreach (KeyValuePair<string, string[]> keyValuePair in Techs.TECH_GROUPING)
			{
				if (Array.IndexOf<string>(keyValuePair.Value, itemID) != -1)
				{
					return Db.Get().Techs.Get(keyValuePair.Key);
				}
			}
			return null;
		}

		public const string AUTOMATION_OVERLAY_ID = "AutomationOverlay";

		public TechItem automationOverlay;

		public const string SUITS_OVERLAY_ID = "SuitsOverlay";

		public TechItem suitsOverlay;

		public const string BETA_RESEARCH_POINT_ID = "BetaResearchPoint";

		public TechItem betaResearchPoint;
	}
}
