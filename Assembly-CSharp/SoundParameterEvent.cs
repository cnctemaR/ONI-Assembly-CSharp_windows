using System;
using FMODUnity;
using UnityEngine;

[Serializable]
public class SoundParameterEvent : AnimEvent, ISerializationCallbackReceiver
{
	public override void OnPlay(IAnimBehaviour behaviour)
	{
		LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
		component.SetParameter(this.soundMigrated, this.parameter, this.startValue);
		if (this.startValue != this.endValue)
		{
			behaviour.AddUpdatingEvent(this);
		}
	}

	public override void OnUpdate(IAnimBehaviour behaviour)
	{
		LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
		component.SetParameter(this.soundMigrated, this.parameter, this.startValue + (this.endValue - this.startValue) * behaviour.normalizedTime);
	}

	public override void Stop(IAnimBehaviour behaviour)
	{
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		if (this.soundMigrated == null)
		{
			this.soundMigrated = GameUtil.MigrateFMOD(this.sound);
		}
	}

	public FMODAsset sound;

	[EventRef]
	public string soundMigrated;

	public string parameter;

	public float startValue;

	public float endValue;
}
