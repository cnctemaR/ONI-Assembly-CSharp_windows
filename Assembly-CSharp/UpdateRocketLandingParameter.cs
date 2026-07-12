using System;
using System.Collections.Generic;
using FMOD.Studio;

internal class UpdateRocketLandingParameter : LoopingSoundParameterUpdater
{
	public UpdateRocketLandingParameter()
		: base("rocketLanding")
	{
	}

	public override void Add(LoopingSoundParameterUpdater.Sound sound)
	{
		UpdateRocketLandingParameter.Entry entry = new UpdateRocketLandingParameter.Entry
		{
			rocketModule = sound.transform.GetComponent<RocketModule>(),
			ev = sound.ev,
			parameterId = sound.description.GetParameterId(base.parameter)
		};
		this.entries.Add(entry);
	}

	public override void Update(float dt)
	{
		foreach (UpdateRocketLandingParameter.Entry entry in this.entries)
		{
			if (!(entry.rocketModule == null))
			{
				LaunchConditionManager conditionManager = entry.rocketModule.conditionManager;
				if (!(conditionManager == null))
				{
					ILaunchableRocket component = conditionManager.GetComponent<ILaunchableRocket>();
					if (component != null)
					{
						if (component.isLanding)
						{
							EventInstance eventInstance = entry.ev;
							eventInstance.setParameterByID(entry.parameterId, 1f, false);
						}
						else
						{
							EventInstance eventInstance = entry.ev;
							eventInstance.setParameterByID(entry.parameterId, 0f, false);
						}
					}
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
				return;
			}
		}
	}

	private List<UpdateRocketLandingParameter.Entry> entries = new List<UpdateRocketLandingParameter.Entry>();

	private struct Entry
	{
		public RocketModule rocketModule;

		public EventInstance ev;

		public PARAMETER_ID parameterId;
	}
}
