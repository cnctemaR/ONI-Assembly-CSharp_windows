using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class BuildingFacadeResource : PermitResource
	{
		[Obsolete("Please use constructor with dlcIds parameter")]
		public BuildingFacadeResource(string Id, string Name, string Description, PermitRarity Rarity, string PrefabID, string AnimFile, Dictionary<string, string> workables = null)
			: this(Id, Name, Description, Rarity, PrefabID, AnimFile, workables, null, null)
		{
		}

		[Obsolete("Please use constructor with dlcIds parameter")]
		public BuildingFacadeResource(string Id, string Name, string Description, PermitRarity Rarity, string PrefabID, string AnimFile, string[] dlcIds, Dictionary<string, string> workables = null)
			: this(Id, Name, Description, Rarity, PrefabID, AnimFile, workables, null, null)
		{
		}

		public BuildingFacadeResource(string Id, string Name, string Description, PermitRarity Rarity, string PrefabID, string AnimFile, Dictionary<string, string> workables = null, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
			: base(Id, Name, Description, PermitCategory.Building, Rarity, requiredDlcIds, forbiddenDlcIds)
		{
			this.Id = Id;
			this.PrefabID = PrefabID;
			this.AnimFile = AnimFile;
			this.InteractFile = workables;
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
				KAnimFileData data = def.AnimFiles[0].GetData();
				KAnimFileData data2 = Assets.GetAnim(this.AnimFile).GetData();
				for (int i = 0; i < data.animCount; i++)
				{
					KAnim.Anim anim = data.GetAnim(i);
					KAnim.Anim anim2 = data2.GetAnim(anim.name);
					if (anim2 != null)
					{
						bool flag = GameAudioSheets.Get().events.ContainsKey(anim.id);
						if (!GameAudioSheets.Get().events.ContainsKey(anim2.id) && flag)
						{
							GameAudioSheets.Get().skinToBaseAnim[anim2.id] = anim.id;
						}
					}
				}
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
