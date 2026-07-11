using System;
using UnityEngine;

namespace Database
{
	public class TechTreeTitles : ResourceSet<TechTreeTitle>
	{
		public TechTreeTitles(ResourceSet parent)
			: base("TreeTitles", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			foreach (ResourceTreeNode resourceTreeNode in new ResourceTreeLoader<ResourceTreeNode>(tree_file))
			{
				if (string.Equals(resourceTreeNode.Id.Substring(0, 1), "_"))
				{
					new TechTreeTitle(resourceTreeNode.Id, this, Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + resourceTreeNode.Id.ToUpper()), resourceTreeNode);
				}
			}
		}
	}
}
