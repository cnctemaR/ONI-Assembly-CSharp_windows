using System;
using FMODUnity;
using UnityEngine;

public class MinionSounds : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-1762453998, new EventSystem.EventHandler(this.OnStartMiningSound));
		this.Subscribe(939543986, new EventSystem.EventHandler(this.OnStopMiningSound));
	}

	private void OnPlayOneShot(object data)
	{
		if (data is FMODAsset)
		{
			SoundEvent.PlayOneShot((string)data, this.transform.position);
		}
	}

	private void OnStartMiningSound(object data)
	{
		if (this.miningSound == null)
		{
			Workable workTarget = this.worker.GetWorkTarget();
			Diggable diggable = workTarget as Diggable;
			if (diggable != null)
			{
				Element targetElement = diggable.GetTargetElement();
				string text = targetElement.substance.GetMiningSound();
				if (text == null || text == string.Empty)
				{
					Debug.LogWarning("Missing Mining sound for element [" + targetElement.name + "]");
					return;
				}
				text = "Mine_" + text;
				this.miningSoundMigrated = GlobalAssets.GetSound(text, false);
				if (this.miningSoundMigrated != null)
				{
					this.loopingSounds.StartSound(this.miningSoundMigrated, this.transform.position);
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
}
