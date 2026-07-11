using System;
using System.Collections.Generic;
using FMOD.Studio;

internal class UpdatePercentCompleteParameter : LoopingSoundParameterUpdater
{
	public UpdatePercentCompleteParameter()
		: base("percentComplete")
	{
	}

	public override void Add(LoopingSoundParameterUpdater.Sound sound)
	{
		UpdatePercentCompleteParameter.Entry entry = new UpdatePercentCompleteParameter.Entry
		{
			worker = sound.transform.GetComponent<Worker>(),
			ev = sound.ev,
			parameterIdx = sound.description.GetParameterIdx(base.parameter)
		};
		this.entries.Add(entry);
	}

	public override void Update(float dt)
	{
		foreach (UpdatePercentCompleteParameter.Entry entry in this.entries)
		{
			if (!(entry.worker == null))
			{
				Workable workable = entry.worker.workable;
				if (!(workable == null))
				{
					float percentComplete = workable.GetPercentComplete();
					entry.ev.setParameterValueByIndex(entry.parameterIdx, percentComplete);
				}
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

	private List<UpdatePercentCompleteParameter.Entry> entries = new List<UpdatePercentCompleteParameter.Entry>();

	private struct Entry
	{
		public Worker worker;

		public EventInstance ev;

		public int parameterIdx;
	}
}
