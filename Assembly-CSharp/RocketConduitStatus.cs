using System;
using UnityEngine;

public static class RocketConduitStatus
{
	public static void UpdateRocketConduitOperational(GameObject sender, GameObject receiver)
	{
		object obj = sender != null && sender.GetComponent<Activatable>().IsActivated;
		bool flag = receiver != null && receiver.GetComponent<Activatable>().IsActivated;
		object obj2 = obj;
		bool flag2 = (obj2 & flag) != null;
		int num = 0;
		if (obj2 != null)
		{
			num++;
		}
		if (flag)
		{
			num++;
		}
		if (sender != null)
		{
			sender.GetComponent<Operational>().SetFlag(RocketConduitStatus.warpConnectedFlag, flag2);
			if (num != 2)
			{
				sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, false);
				sender.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, num);
			}
			else
			{
				sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, false);
			}
		}
		if (receiver != null)
		{
			receiver.GetComponent<Operational>().SetFlag(RocketConduitStatus.warpConnectedFlag, flag2);
			if (num != 2)
			{
				receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, false);
				receiver.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, num);
				return;
			}
			receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, false);
		}
	}

	public static readonly Operational.Flag warpConnectedFlag = new Operational.Flag("warp_conduit_connected", Operational.Flag.Type.Requirement);
}
