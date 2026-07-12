using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class BuildingFacadeResource : PermitResource
	{
		[Obsolete]
		public BuildingFacadeResource(string Id, string Name, string Description, PermitRarity Rarity, string PrefabID, string AnimFile, Dictionary<string, string> workables = null)
			: base(Id, Name, Description, PermitCategory.Building, Rarity, DlcManager.AVAILABLE_ALL_VERSIONS)
		{
			this.Id = Id;
			this.PrefabID = PrefabID;
			this.AnimFile = AnimFile;
			this.InteractFile = workables;
		}

		public BuildingFacadeResource(string Id, string Name, string Description, PermitRarity Rarity, string PrefabID, string AnimFile, string[] DLCIds, Dictionary<string, string> workables = null)
			: base(Id, Name, Description, PermitCategory.Building, Rarity, DLCIds)
		{
			this.Id = Id;
			this.PrefabID = PrefabID;
			this.AnimFile = AnimFile;
			this.InteractFile = workables;
		}

		public BuildingFacadeResource(string Id, string Name, string Description, PermitRarity Rarity, string PrefabID, string AnimFile, string[] DLCIds, List<FacadeInfo.workable> workables = null)
			: base(Id, Name, Description, PermitCategory.Building, Rarity, DLCIds)
		{
			this.Id = Id;
			this.PrefabID = PrefabID;
			this.AnimFile = AnimFile;
			this.InteractFile = new Dictionary<string, string>();
			if (workables != null)
			{
				foreach (FacadeInfo.workable workable in workables)
				{
					this.InteractFile.Add(workable.workableName, workable.workableAnim);
				}
			}
		}

		public void Init()
		{
			GameObject gameObject = Assets.TryGetPrefab(this.PrefabID);
			if (gameObject == null)
			{
				return;
			}
			gameObject.AddOrGet<BuildingFacade>();
			BuildingDef def = gameObject.GetComponent<Building>().Def;
			if (def != null)
			{
				def.AddFacade(this.Id);
			}
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			permitPresentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(this.AnimFile), "ui", false, "");
			permitPresentationInfo.SetFacadeForPrefabID(this.PrefabID);
			return permitPresentationInfo;
		}

		public string PrefabID;

		public string AnimFile;

		public Dictionary<string, string> InteractFile;
	}
}
