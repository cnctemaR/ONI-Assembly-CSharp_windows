using System;
using System.IO;

namespace Database
{
	public class VentXKG : ColonyAchievementRequirement
	{
		public VentXKG(SimHashes element, float kilogramsToVent)
		{
			this.element = element;
			this.kilogramsToVent = kilogramsToVent;
		}

		public override bool Success()
		{
			float num = 0f;
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(ConduitType.Gas);
			foreach (UtilityNetwork utilityNetwork in networkManager.GetNetworks())
			{
				FlowUtilityNetwork flowUtilityNetwork = utilityNetwork as FlowUtilityNetwork;
				if (flowUtilityNetwork != null)
				{
					foreach (FlowUtilityNetwork.IItem item in flowUtilityNetwork.sinks)
					{
						Vent component = item.GameObject.GetComponent<Vent>();
						if (component != null)
						{
							num += component.GetVentedMass(this.element);
						}
					}
				}
			}
			return num >= this.kilogramsToVent * 1000f;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write((int)this.element);
			writer.Write(this.kilogramsToVent);
		}

		public override void Deserialize(IReader reader)
		{
			this.element = (SimHashes)reader.ReadInt32();
			this.kilogramsToVent = reader.ReadSingle();
		}

		private SimHashes element;

		private float kilogramsToVent;
	}
}
