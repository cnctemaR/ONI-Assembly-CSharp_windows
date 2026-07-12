using System;

namespace Database
{
	public class EquippableFacades : ResourceSet<EquippableFacadeResource>
	{
		public EquippableFacades(ResourceSet parent)
			: base("EquippableFacades", parent)
		{
			base.Initialize();
			foreach (EquippableFacadeInfo equippableFacadeInfo in Blueprints.Get().all.equippableFacades)
			{
				this.Add(equippableFacadeInfo.id, equippableFacadeInfo.name, equippableFacadeInfo.desc, equippableFacadeInfo.rarity, equippableFacadeInfo.defID, equippableFacadeInfo.buildOverride, equippableFacadeInfo.animFile, equippableFacadeInfo.dlcIds);
			}
		}

		[Obsolete("Please use Add(...) with dlcIds parameter")]
		public void Add(string id, string name, string desc, PermitRarity rarity, string defID, string buildOverride, string animFile)
		{
			this.Add(id, name, desc, rarity, defID, buildOverride, animFile, DlcManager.AVAILABLE_ALL_VERSIONS);
		}

		public void Add(string id, string name, string desc, PermitRarity rarity, string defID, string buildOverride, string animFile, string[] dlcIds)
		{
			EquippableFacadeResource equippableFacadeResource = new EquippableFacadeResource(id, name, desc, rarity, buildOverride, defID, animFile, dlcIds);
			this.resources.Add(equippableFacadeResource);
		}
	}
}
