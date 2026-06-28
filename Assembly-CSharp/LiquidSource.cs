using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class LiquidSource : SubstanceSource
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = this.pickupable;
		pickupable.OnGetAnim = (Func<Worker, Workable.AnimInfo>)Delegate.Combine(pickupable.OnGetAnim, new Func<Worker, Workable.AnimInfo>((Worker worker) => new Workable.AnimInfo
		{
			smi = new MultitoolController.Instance(base.GetComponent<Pickupable>(), worker, "fetchliquid", EffectPrefabs.Instance.WhirlpoolEffect)
		}));
		this.pickupable.isKinematic = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.LiquidSources.Add(this);
	}

	protected override CellOffset[] GetOffsetGroup()
	{
		return OffsetGroups.LiquidSource;
	}

	protected override IChunkManager GetChunkManager()
	{
		return LiquidSourceManager.Instance;
	}

	protected override void OnCleanUp()
	{
		Components.LiquidSources.Remove(this);
		base.OnCleanUp();
	}
}
