using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForMaterialsStatusItem : MaterialsStatusItem
{
	public WaitingForMaterialsStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode overlay)
		: base(id, prefix, icon, icon_type, notification_type, allow_multiples, overlay)
	{
	}

	public override bool ShouldAdd(IFetchList fetch_list, Dictionary<Tag, float> remaining)
	{
		foreach (KeyValuePair<Tag, float> keyValuePair in remaining)
		{
			if (this.GetActualAvailable(keyValuePair.Key, keyValuePair.Value) < keyValuePair.Value)
			{
				return false;
			}
		}
		return true;
	}

	private float GetActualAvailable(Tag tag, float reserve_amount)
	{
		float totalAmount = WorldInventory.Instance.GetTotalAmount(tag);
		float amount = WorldInventory.Instance.GetAmount(tag);
		float num = Mathf.Min(reserve_amount, totalAmount);
		return amount + num;
	}
}
