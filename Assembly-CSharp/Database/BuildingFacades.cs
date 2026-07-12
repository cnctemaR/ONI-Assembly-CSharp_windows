using System;
using System.Collections.Generic;

namespace Database
{
	public class BuildingFacades : ResourceSet<BuildingFacadeResource>
	{
		public BuildingFacades(ResourceSet parent)
			: base("BuildingFacades", parent)
		{
			base.Initialize();
			foreach (BuildingFacadeInfo buildingFacadeInfo in Blueprints.Get().all.buildingFacades)
			{
				this.Add(buildingFacadeInfo.id, buildingFacadeInfo.name, buildingFacadeInfo.desc, buildingFacadeInfo.rarity, buildingFacadeInfo.prefabId, buildingFacadeInfo.animFile, buildingFacadeInfo.dlcIds, buildingFacadeInfo.workables);
			}
		}

		[Obsolete("Please use Add(...) with dlcIds parameter")]
		public void Add(string id, LocString Name, LocString Desc, PermitRarity rarity, string prefabId, string animFile, Dictionary<string, string> workables = null)
		{
			this.Add(id, Name, Desc, rarity, prefabId, animFile, DlcManager.AVAILABLE_ALL_VERSIONS, workables);
		}

		public void Add(string id, LocString Name, LocString Desc, PermitRarity rarity, string prefabId, string animFile, string[] dlcIds, Dictionary<string, string> workables = null)
		{
			BuildingFacadeResource buildingFacadeResource = new BuildingFacadeResource(id, Name, Desc, rarity, prefabId, animFile, dlcIds, workables);
			this.resources.Add(buildingFacadeResource);
		}

		public void PostProcess()
		{
			foreach (BuildingFacadeResource buildingFacadeResource in this.resources)
			{
				buildingFacadeResource.Init();
			}
		}
	}
}
