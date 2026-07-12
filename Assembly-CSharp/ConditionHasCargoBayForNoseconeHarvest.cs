using System;
using System.Collections.Generic;
using STRINGS;

public class ConditionHasCargoBayForNoseconeHarvest : ProcessCondition
{
	public ConditionHasCargoBayForNoseconeHarvest(LaunchableRocketCluster launchable)
	{
		this.launchable = launchable;
	}

	public override ProcessCondition.Status EvaluateCondition()
	{
		using (IEnumerator<Ref<RocketModuleCluster>> enumerator = this.launchable.parts.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Get().GetComponent<CargoBayCluster>())
				{
					return ProcessCondition.Status.Ready;
				}
			}
		}
		return ProcessCondition.Status.Warning;
	}

	public override string GetStatusMessage(ProcessCondition.Status status)
	{
		string text = "";
		switch (status)
		{
		case ProcessCondition.Status.Failure:
			text = UI.STARMAP.LAUNCHCHECKLIST.HAS_CARGO_BAY_FOR_NOSECONE_HARVEST.STATUS.FAILURE;
			break;
		case ProcessCondition.Status.Warning:
			text = UI.STARMAP.LAUNCHCHECKLIST.HAS_CARGO_BAY_FOR_NOSECONE_HARVEST.STATUS.WARNING;
			break;
		case ProcessCondition.Status.Ready:
			text = UI.STARMAP.LAUNCHCHECKLIST.HAS_CARGO_BAY_FOR_NOSECONE_HARVEST.STATUS.READY;
			break;
		}
		return text;
	}

	public override string GetStatusTooltip(ProcessCondition.Status status)
	{
		string text = "";
		switch (status)
		{
		case ProcessCondition.Status.Failure:
			text = UI.STARMAP.LAUNCHCHECKLIST.HAS_CARGO_BAY_FOR_NOSECONE_HARVEST.TOOLTIP.FAILURE;
			break;
		case ProcessCondition.Status.Warning:
			text = UI.STARMAP.LAUNCHCHECKLIST.HAS_CARGO_BAY_FOR_NOSECONE_HARVEST.TOOLTIP.WARNING;
			break;
		case ProcessCondition.Status.Ready:
			text = UI.STARMAP.LAUNCHCHECKLIST.HAS_CARGO_BAY_FOR_NOSECONE_HARVEST.TOOLTIP.READY;
			break;
		}
		return text;
	}

	public override bool ShowInUI()
	{
		using (IEnumerator<Ref<RocketModuleCluster>> enumerator = this.launchable.parts.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Get().HasTag("NoseconeHarvest"))
				{
					return true;
				}
			}
		}
		return false;
	}

	private LaunchableRocketCluster launchable;
}
