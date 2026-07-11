using System;
using System.IO;

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
					return true;
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
