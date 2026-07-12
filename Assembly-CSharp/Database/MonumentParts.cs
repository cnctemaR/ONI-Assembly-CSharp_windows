using System;
using System.Collections.Generic;

namespace Database
{
	public class MonumentParts : ResourceSet<MonumentPartResource>
	{
		public MonumentParts(ResourceSet parent)
			: base("MonumentParts", parent)
		{
			base.Initialize();
			foreach (MonumentPartInfo monumentPartInfo in Blueprints.Get().all.monumentParts)
			{
				this.Add(monumentPartInfo.id, monumentPartInfo.name, monumentPartInfo.desc, monumentPartInfo.rarity, monumentPartInfo.animFile, monumentPartInfo.state, monumentPartInfo.symbolName, monumentPartInfo.part, monumentPartInfo.dlcIds);
			}
		}

		public void Add(string id, string name, string desc, PermitRarity rarity, string animFilename, string state, string symbolName, MonumentPartResource.Part part, string[] dlcIds)
		{
			MonumentPartResource monumentPartResource = new MonumentPartResource(id, name, desc, rarity, animFilename, state, symbolName, part, dlcIds);
			this.resources.Add(monumentPartResource);
		}

		public List<MonumentPartResource> GetParts(MonumentPartResource.Part part)
		{
			return this.resources.FindAll((MonumentPartResource mpr) => mpr.part == part);
		}
	}
}
