using System;
using System.IO;
using UnityEngine;

namespace Database
{
	public class PumpXDistanceY : ColonyAchievementRequirement
	{
		public PumpXDistanceY(SimHashes element, float distance)
		{
			this.element = element;
			this.conduitType = ((!ElementLoader.GetElement(element.CreateTag()).IsLiquid) ? ConduitType.Gas : ConduitType.Liquid);
			this.distance = (double)distance;
		}

		public override bool Success()
		{
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
			foreach (UtilityNetwork utilityNetwork in networkManager.GetNetworks())
			{
				FlowUtilityNetwork flowUtilityNetwork = utilityNetwork as FlowUtilityNetwork;
				if (flowUtilityNetwork != null)
				{
					foreach (FlowUtilityNetwork.IItem item in flowUtilityNetwork.sinks)
					{
						ConduitConsumer component = item.GameObject.GetComponent<ConduitConsumer>();
						if (!(component == null) && component.IsConnected && component.lastConsumedElement == this.element)
						{
							Vector3 vector = Grid.CellToPos(item.Cell);
							foreach (FlowUtilityNetwork.IItem item2 in flowUtilityNetwork.sources)
							{
								ConduitDispenser component2 = item2.GameObject.GetComponent<ConduitDispenser>();
								if (!(component2 == null) && component2.IsConnected && component2.ConduitContents.element == this.element)
								{
									Vector3 vector2 = Grid.CellToPos(item2.Cell);
									if ((double)Vector3.Distance(vector, vector2) > this.distance)
									{
										return true;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write((int)this.element);
			writer.Write((int)this.conduitType);
			writer.Write(this.distance);
		}

		public override void Deserialize(IReader reader)
		{
			this.element = (SimHashes)reader.ReadInt32();
			this.conduitType = (ConduitType)reader.ReadInt32();
			this.distance = reader.ReadDouble();
		}

		private ConduitType conduitType = ConduitType.Liquid;

		private SimHashes element;

		private double distance;
	}
}
