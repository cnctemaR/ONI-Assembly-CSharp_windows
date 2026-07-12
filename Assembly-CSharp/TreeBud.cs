using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TreeBud")]
public class TreeBud : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		PlantBranch.Instance smi = base.gameObject.GetSMI<PlantBranch.Instance>();
		if (smi != null && !smi.IsRunning())
		{
			smi.StartSM();
		}
	}

	public BuddingTrunk GetAndForgetOldTrunk()
	{
		BuddingTrunk buddingTrunk = ((this.buddingTrunk == null) ? null : this.buddingTrunk.Get());
		this.buddingTrunk = null;
		return buddingTrunk;
	}

	[Serialize]
	public Ref<BuddingTrunk> buddingTrunk;
}
