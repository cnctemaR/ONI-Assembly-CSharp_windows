using System;
using System.IO;
using UnityEngine;

namespace Database
{
	public class AutomateABuilding : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (UtilityNetwork utilityNetwork in Game.Instance.logicCircuitSystem.GetNetworks())
			{
				LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)utilityNetwork;
				if (logicCircuitNetwork.Receivers.Count > 0 && logicCircuitNetwork.Senders.Count > 0)
				{
					bool flag = false;
					foreach (ILogicEventReceiver logicEventReceiver in logicCircuitNetwork.Receivers)
					{
						GameObject gameObject = Grid.Objects[logicEventReceiver.GetLogicCell(), 1];
						if (gameObject != null)
						{
							KPrefabID component = gameObject.GetComponent<KPrefabID>();
							if (!component.HasTag(GameTags.TemplateBuilding))
							{
								flag = true;
								break;
							}
						}
					}
					bool flag2 = false;
					foreach (ILogicEventSender logicEventSender in logicCircuitNetwork.Senders)
					{
						GameObject gameObject2 = Grid.Objects[logicEventSender.GetLogicCell(), 1];
						if (gameObject2 != null)
						{
							KPrefabID component2 = gameObject2.GetComponent<KPrefabID>();
							if (!component2.HasTag(GameTags.TemplateBuilding))
							{
								flag2 = true;
								break;
							}
						}
					}
					return flag && flag2;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override void Deserialize(IReader reader)
		{
		}
	}
}
