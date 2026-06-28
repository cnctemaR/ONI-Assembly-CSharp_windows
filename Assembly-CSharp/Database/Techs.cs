using System;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		public Techs(ResourceSet parent)
			: base("Techs", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			foreach (ResourceTreeNode resourceTreeNode in resourceTreeLoader)
			{
				Tech tech = base.TryGet(resourceTreeNode.Id);
				if (tech == null)
				{
					tech = new Tech(resourceTreeNode.Id, this, Strings.Get("STRINGS.RESEARCH." + resourceTreeNode.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH." + resourceTreeNode.Id.ToUpper() + ".DESC"), resourceTreeNode);
				}
				foreach (ResourceTreeNode resourceTreeNode2 in resourceTreeNode.references)
				{
					Tech tech2 = base.TryGet(resourceTreeNode2.Id);
					if (tech2 == null)
					{
						tech2 = new Tech(resourceTreeNode2.Id, this, Strings.Get("STRINGS.RESEARCH." + resourceTreeNode2.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH." + resourceTreeNode2.Id.ToUpper() + ".DESC"), resourceTreeNode2);
					}
					tech2.requiredTech.Add(tech);
					tech.unlockedTech.Add(tech2);
				}
			}
			this.tierCount = 0;
			foreach (Tech tech3 in this)
			{
				tech3.tier = this.GetTier(tech3);
				if (tech3.tier == 0)
				{
					tech3.costsByResearchTypeID.Add(ResearchTypes.ID.ALPHA, (float)(20 * (tech3.tier + 1)));
				}
				else
				{
					tech3.costsByResearchTypeID.Add(ResearchTypes.ID.ALPHA, (float)(20 * tech3.tier));
				}
				tech3.costsByResearchTypeID.Add(ResearchTypes.ID.BETA, (float)Mathf.Max(0, 10 * tech3.tier));
				this.tierCount = Math.Max(tech3.tier + 1, this.tierCount);
			}
		}

		private int GetTier(Tech tech)
		{
			if (tech.requiredTech.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (Tech tech2 in tech.requiredTech)
			{
				num = Math.Max(num, this.GetTier(tech2));
			}
			return num + 1;
		}

		private void AddPrerequisite(Tech tech, string prerequisite_name)
		{
			Tech tech2 = base.TryGet(prerequisite_name);
			if (tech2 != null)
			{
				tech.requiredTech.Add(tech2);
				tech2.unlockedTech.Add(tech);
			}
		}

		public int tierCount;
	}
}
