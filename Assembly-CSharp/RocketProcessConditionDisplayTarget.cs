using System;
using System.Collections.Generic;

public class RocketProcessConditionDisplayTarget : KMonoBehaviour, IProcessConditionSet, ISim1000ms
{
	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		if (this.craftModuleInterface == null)
		{
			this.craftModuleInterface = base.GetComponent<RocketModuleCluster>().CraftInterface;
		}
		return this.craftModuleInterface.GetConditionSet(conditionType);
	}

	public int PopulateConditionSet(ProcessCondition.ProcessConditionType conditionType, List<ProcessCondition> conditions)
	{
		if (this.craftModuleInterface == null)
		{
			this.craftModuleInterface = base.GetComponent<RocketModuleCluster>().CraftInterface;
		}
		return this.craftModuleInterface.PopulateConditionSet(conditionType, conditions);
	}

	public void Sim1000ms(float dt)
	{
		bool flag = false;
		List<ProcessCondition> list;
		using (ProcessCondition.ListPool.Get(out list))
		{
			this.PopulateConditionSet(ProcessCondition.ProcessConditionType.All, list);
			using (List<ProcessCondition>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.EvaluateCondition() == ProcessCondition.Status.Failure)
					{
						flag = true;
						if (this.statusHandle == Guid.Empty)
						{
							this.statusHandle = this.kselectable.AddStatusItem(Db.Get().BuildingStatusItems.RocketChecklistIncomplete, null);
							break;
						}
						break;
					}
				}
			}
		}
		if (!flag && this.statusHandle != Guid.Empty)
		{
			this.kselectable.RemoveStatusItem(this.statusHandle, false);
		}
	}

	private CraftModuleInterface craftModuleInterface;

	[MyCmpReq]
	private KSelectable kselectable;

	private Guid statusHandle = Guid.Empty;
}
