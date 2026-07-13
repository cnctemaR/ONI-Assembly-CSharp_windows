using System;
using System.Collections.Generic;

namespace Database
{
	public class ArtableStages : ResourceSet<ArtableStage>
	{
		[Obsolete("Use ArtableStages with required/forbidden")]
		public ArtableStage Add(string id, string name, string desc, PermitRarity rarity, string animFile, string anim, int decor_value, bool cheer_on_complete, string status_id, string prefabId, string symbolname, string[] dlcIds)
		{
			DlcRestrictionsUtil.TemporaryHelperObject transientHelperObjectFromAllowList = DlcRestrictionsUtil.GetTransientHelperObjectFromAllowList(dlcIds);
			return this.Add(id, name, desc, rarity, animFile, anim, decor_value, cheer_on_complete, status_id, prefabId, symbolname, transientHelperObjectFromAllowList.GetRequiredDlcIds(), transientHelperObjectFromAllowList.GetForbiddenDlcIds());
		}

		public ArtableStage Add(string id, string name, string desc, PermitRarity rarity, string animFile, string anim, int decor_value, bool cheer_on_complete, string status_id, string prefabId, string symbolname, string[] requiredDlcIds, string[] forbiddenDlcIds)
		{
			ArtableStatusItem artableStatusItem = Db.Get().ArtableStatuses.Get(status_id);
			ArtableStage artableStage = new ArtableStage(id, name, desc, rarity, animFile, anim, decor_value, cheer_on_complete, artableStatusItem, prefabId, symbolname, requiredDlcIds, forbiddenDlcIds);
			this.resources.Add(artableStage);
			return artableStage;
		}

		public ArtableStages(ResourceSet parent)
			: base("ArtableStages", parent)
		{
			foreach (ArtableInfo artableInfo in Blueprints.Get().all.artables)
			{
				this.Add(artableInfo.id, artableInfo.name, artableInfo.desc, artableInfo.rarity, artableInfo.animFile, artableInfo.anim, artableInfo.decor_value, artableInfo.cheer_on_complete, artableInfo.status_id, artableInfo.prefabId, artableInfo.symbolname, artableInfo.GetRequiredDlcIds(), artableInfo.GetForbiddenDlcIds());
			}
		}

		public List<ArtableStage> GetPrefabStages(Tag prefab_id)
		{
			return this.resources.FindAll((ArtableStage stage) => stage.prefabId == prefab_id);
		}

		public ArtableStage DefaultPrefabStage(Tag prefab_id)
		{
			return this.GetPrefabStages(prefab_id).Find((ArtableStage stage) => stage.statusItem == Db.Get().ArtableStatuses.AwaitingArting);
		}
	}
}
