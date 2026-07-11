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
			this.gammaResearchPoint = this.AddTechItem("GammaResearchPoint", RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.NAME, RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.DESC, this.GetSpriteFnBuilder("research_type_gamma_icon"));
			this.conveyorOverlay = this.AddTechItem("ConveyorOverlay", RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.DESC, this.GetSpriteFnBuilder("overlay_conveyor"));
			this.jetSuit = this.AddTechItem("JetSuit", RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.NAME, RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.DESC, this.GetSpriteFnBuilder("overlay_suit"));
		}

		private Func<string, bool, Sprite> GetSpriteFnBuilder(string spriteName)
		{
			return (string anim, bool centered) => Assets.GetSprite(spriteName);
		}

		public TechItem AddTechItem(string id, string name, string description, Func<string, bool, Sprite> getUISprite)
		{
			if (base.TryGet(id) != null)
			{
				DebugUtil.LogWarningArgs(new object[] { "Tried adding a tech item called", id, name, "but it was already added!" });
				return base.Get(id);
			}
			Tech tech = this.LookupGroupForID(id);
			if (tech == null)
			{
				return null;
			}
			TechItem techItem = new TechItem(id, this, name, description, getUISprite, tech);
			base.Add(techItem);
			tech.unlockedItems.Add(techItem);
			return techItem;
		}

		public bool IsTechItemComplete(string id)
		{
			foreach (TechItem techItem in this.resources)
			{
				if (techItem.Id == id)
				{
					return techItem.IsComplete();
				}
			}
			return true;
		}

		public Tech LookupGroupForID(string itemID)
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

		public const string JET_SUIT_ID = "JetSuit";

		public TechItem jetSuit;

		public const string BETA_RESEARCH_POINT_ID = "BetaResearchPoint";

		public TechItem betaResearchPoint;

		public const string GAMMA_RESEARCH_POINT_ID = "GammaResearchPoint";

		public TechItem gammaResearchPoint;

		public const string CONVEYOR_OVERLAY_ID = "ConveyorOverlay";

		public TechItem conveyorOverlay;
	}
}
