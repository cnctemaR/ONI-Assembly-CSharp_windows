using System;
using FMODUnity;

public class MinionSounds : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(-1762453998, new Action<object>(this.OnStartMiningSound));
		base.Subscribe(939543986, new Action<object>(this.OnStopMiningSound));
	}

	private void OnPlayOneShot(object data)
	{
		if (data is FMODAsset)
		{
			SoundEvent.PlayOneShot((string)data, base.transform.position);
		}
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
				if (text != null && !(text == ""))
				{
					text = "Mine_" + text;
					this.miningSoundMigrated = GlobalAssets.GetSound(text, false);
					if (this.miningSoundMigrated != null)
					{
						this.loopingSounds.StartSound(this.miningSoundMigrated, base.transform.position);
					}
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

	private FMODAsset miningSound = null;

	[EventRef]
	private string miningSoundMigrated;
}
