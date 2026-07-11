using System;
using System.Collections.Generic;
using FMOD.Studio;

internal class UpdateConsumedMassParameter : LoopingSoundParameterUpdater
{
	public UpdateConsumedMassParameter()
		: base("consumedMass")
	{
	}

	public override void Add(LoopingSoundParameterUpdater.Sound sound)
	{
		UpdateConsumedMassParameter.Entry entry = new UpdateConsumedMassParameter.Entry
		{
			creatureCalorieMonitor = sound.transform.GetSMI<CreatureCalorieMonitor.Instance>(),
			ev = sound.ev,
			parameterIdx = sound.description.GetParameterIdx(base.parameter)
		};
		this.entries.Add(entry);
	}

	public override void Update(float dt)
	{
		foreach (UpdateConsumedMassParameter.Entry entry in this.entries)
		{
			if (!entry.creatureCalorieMonitor.IsNullOrStopped())
			{
				float fullness = entry.creatureCalorieMonitor.stomach.GetFullness();
				EventInstance ev = entry.ev;
				ev.setParameterValueByIndex(entry.parameterIdx, fullness);
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
				return;
			}
		}
	}

	private List<UpdateConsumedMassParameter.Entry> entries = new List<UpdateConsumedMassParameter.Entry>();

	private struct Entry
	{
		public CreatureCalorieMonitor.Instance creatureCalorieMonitor;

		public EventInstance ev;

		public int parameterIdx;
	}
}
