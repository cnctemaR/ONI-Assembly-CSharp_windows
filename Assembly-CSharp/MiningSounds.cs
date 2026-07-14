using System;
using FMODUnity;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MiningSounds")]
public class MiningSounds : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<MiningSounds>(-1762453998, MiningSounds.OnStartMiningSoundDelegate);
		base.Subscribe<MiningSounds>(939543986, MiningSounds.OnStopMiningSoundDelegate);
	}

	private void OnStartMiningSound(object data)
	{
		if (this.miningSound != null)
		{
			return;
		}
		Element element = data as Element;
		if (element == null)
		{
			return;
		}
		string text = element.substance.GetMiningSound();
		if (text == null || text == "")
		{
			return;
		}
		if (this.IsTargetCellLiquid())
		{
			return;
		}
		text = "Mine_" + text;
		string sound = GlobalAssets.GetSound(text, false);
		this.miningSoundEvent = RuntimeManager.PathToEventReference(sound);
		DebugUtil.DevAssert(!this.miningSoundEvent.IsNull, "Failed to find mining sound event for element", null);
		if (this.miningSoundEvent.IsNull)
		{
			return;
		}
		this.loopingSounds.StartSound(this.miningSoundEvent);
	}

	private void OnStopMiningSound(object data)
	{
		if (!this.miningSoundEvent.IsNull)
		{
			this.loopingSounds.StopSound(this.miningSoundEvent);
			this.miningSound = null;
		}
	}

	public void SetPercentComplete(float progress)
	{
		if (!this.miningSoundEvent.IsNull)
		{
			this.loopingSounds.SetParameter(this.miningSoundEvent, MiningSounds.HASH_PERCENTCOMPLETE, progress);
		}
	}

	private bool IsTargetCellLiquid()
	{
		WorkerBase workerBase;
		if (!base.TryGetComponent<WorkerBase>(out workerBase))
		{
			return false;
		}
		Workable workable = workerBase.GetWorkable();
		return !(workable == null) && Grid.IsLiquid(Grid.PosToCell(workable));
	}

	private static HashedString HASH_PERCENTCOMPLETE = "percentComplete";

	[MyCmpGet]
	private LoopingSounds loopingSounds;

	private FMODAsset miningSound;

	private EventReference miningSoundEvent;

	private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStartMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>(delegate(MiningSounds component, object data)
	{
		component.OnStartMiningSound(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStopMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>(delegate(MiningSounds component, object data)
	{
		component.OnStopMiningSound(data);
	});
}
