using System;
using FMODUnity;

public class MinionSounds : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<MinionSounds>(-1762453998, MinionSounds.OnStartMiningSoundDelegate);
		base.Subscribe<MinionSounds>(939543986, MinionSounds.OnStopMiningSoundDelegate);
	}

	private void OnStartMiningSound(object data)
	{
		if (this.miningSound == null)
		{
			Workable workable = this.worker.workable;
			Diggable diggable = workable as Diggable;
			if (diggable != null)
			{
				Element targetElement = diggable.GetTargetElement();
				string text = targetElement.substance.GetMiningSound();
				if (text == null || text == string.Empty)
				{
					return;
				}
				text = "Mine_" + text;
				this.miningSoundMigrated = GlobalAssets.GetSound(text, false);
				if (this.miningSoundMigrated != null)
				{
					this.loopingSounds.StartSound(this.miningSoundMigrated);
				}
			}
		}
	}

	private void OnStopMiningSound(object data)
	{
		if (this.miningSoundMigrated != null)
		{
			this.loopingSounds.StopSound(this.miningSoundMigrated);
			this.miningSound = null;
		}
	}

	[MyCmpReq]
	private Worker worker;

	[MyCmpGet]
	private LoopingSounds loopingSounds;

	private FMODAsset miningSound;

	[EventRef]
	private string miningSoundMigrated;

	private static readonly EventSystem.IntraObjectHandler<MinionSounds> OnStartMiningSoundDelegate = new EventSystem.IntraObjectHandler<MinionSounds>(delegate(MinionSounds component, object data)
	{
		component.OnStartMiningSound(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionSounds> OnStopMiningSoundDelegate = new EventSystem.IntraObjectHandler<MinionSounds>(delegate(MinionSounds component, object data)
	{
		component.OnStopMiningSound(data);
	});
}
