using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuddingTrunk")]
public class BuddingTrunk : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		PlantBranchGrower.Instance smi = base.gameObject.GetSMI<PlantBranchGrower.Instance>();
		if (smi != null && !smi.IsRunning())
		{
			smi.StartSM();
		}
	}

	public KPrefabID[] GetAndForgetOldSerializedBranches()
	{
		KPrefabID[] array = null;
		if (this.buds != null)
		{
			array = new KPrefabID[this.buds.Length];
			for (int i = 0; i < this.buds.Length; i++)
			{
				HarvestDesignatable harvestDesignatable = ((this.buds[i] == null) ? null : this.buds[i].Get());
				array[i] = ((harvestDesignatable == null) ? null : harvestDesignatable.GetComponent<KPrefabID>());
			}
		}
		this.buds = null;
		return array;
	}

	[Serialize]
	private Ref<HarvestDesignatable>[] buds;
}
