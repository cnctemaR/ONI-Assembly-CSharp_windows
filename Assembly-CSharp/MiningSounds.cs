using System;
using FMODUnity;

public class MiningSounds : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<MiningSounds>(-1762453998, MiningSounds.OnStartMiningSoundDelegate);
		base.Subscribe<MiningSounds>(939543986, MiningSounds.OnStopMiningSoundDelegate);
	}

	private void OnStartMiningSound(object data)
	{
		if (this.miningSound == null)
		{
			Element element = data as Element;
			if (element != null)
			{
				string text = element.substance.GetMiningSound();
				if (text == null || text == string.Empty)
				{
					return;
				}
				text = "Mine_" + text;
				this.miningSoundEvent = GlobalAssets.GetSound(text, false);
				if (this.miningSoundEvent != null)
				{
					this.loopingSounds.StartSound(this.miningSoundEvent);
				}
			}
		}
	}

	private void OnStopMiningSound(object data)
	{
		if (this.miningSoundEvent != null)
		{
			this.loopingSounds.StopSound(this.miningSoundEvent);
			this.miningSound = null;
		}
	}

	public void SetPercentComplete(float progress)
	{
		this.loopingSounds.SetParameter(this.miningSoundEvent, MiningSounds.HASH_PERCENTCOMPLETE, progress);
	}

	private static HashedString HASH_PERCENTCOMPLETE = "percentComplete";

	[MyCmpGet]
	private LoopingSounds loopingSounds;

	private FMODAsset miningSound;

	[EventRef]
	private string miningSoundEvent;

	private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStartMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>(delegate(MiningSounds component, object data)
	{
		component.OnStartMiningSound(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStopMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>(delegate(MiningSounds component, object data)
	{
		component.OnStopMiningSound(data);
	});
}
