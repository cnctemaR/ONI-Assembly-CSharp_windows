using System;
using System.Collections.Generic;
using FMOD.Studio;

internal class UpdateDistanceToImpactParameter : LoopingSoundParameterUpdater
{
	public UpdateDistanceToImpactParameter()
		: base("distanceToImpact")
	{
	}

	public override void Add(LoopingSoundParameterUpdater.Sound sound)
	{
		UpdateDistanceToImpactParameter.Entry entry = new UpdateDistanceToImpactParameter.Entry
		{
			comet = sound.transform.GetComponent<Comet>(),
			ev = sound.ev,
			parameterIdx = sound.description.GetParameterIdx(base.parameter)
		};
		this.entries.Add(entry);
	}

	public override void Update(float dt)
	{
		foreach (UpdateDistanceToImpactParameter.Entry entry in this.entries)
		{
			if (!(entry.comet == null))
			{
				float soundDistance = entry.comet.GetSoundDistance();
				entry.ev.setParameterValueByIndex(entry.parameterIdx, soundDistance);
			}
		}
	}

	public override void Remove(LoopingSoundParameterUpdater.Sound sound)
	{
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].ev.handle == sound.ev.handle)
			{
				this.entries.RemoveAt(i);
				break;
			}
		}
	}

	private List<UpdateDistanceToImpactParameter.Entry> entries = new List<UpdateDistanceToImpactParameter.Entry>();

	private struct Entry
	{
		public Comet comet;

		public EventInstance ev;

		public int parameterIdx;
	}
}
