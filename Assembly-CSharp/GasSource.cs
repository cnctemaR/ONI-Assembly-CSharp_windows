using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class GasSource : SubstanceSource
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = this.pickupable;
		pickupable.OnGetAnim = (Func<Worker, Workable.AnimInfo>)Delegate.Combine(pickupable.OnGetAnim, new Func<Worker, Workable.AnimInfo>((Worker worker) => new Workable.AnimInfo
		{
			smi = new MultitoolController.Instance(base.GetComponent<Pickupable>(), worker, "fetchliquid", EffectPrefabs.Instance.WhirlpoolEffect)
		}));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override CellOffset[] GetOffsetGroup()
	{
		return OffsetGroups.LiquidSource;
	}

	protected override IChunkManager GetChunkManager()
	{
		return GasSourceManager.Instance;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}
}
