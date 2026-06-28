using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsUnavailableForRefillStatusItem : MaterialsStatusItem
{
	public MaterialsUnavailableForRefillStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode overlay)
		: base(id, prefix, icon, icon_type, notification_type, allow_multiples, overlay)
	{
	}

	public override bool ShouldAdd(IFetchList fetch_list, Dictionary<Tag, float> remaining)
	{
		foreach (KeyValuePair<Tag, float> keyValuePair in remaining)
		{
			float amountInStorage = base.GetAmountInStorage(fetch_list.Destination, keyValuePair.Key);
			float actualAvailable = this.GetActualAvailable(keyValuePair.Key, keyValuePair.Value);
			if (amountInStorage + actualAvailable > fetch_list.GetMinimumAmount(keyValuePair.Key) && keyValuePair.Value > actualAvailable)
			{
				return true;
			}
		}
		return false;
	}

	private float GetActualAvailable(Tag tag, float reserve_amount)
	{
		float totalAmount = WorldInventory.Instance.GetTotalAmount(tag);
		float amount = WorldInventory.Instance.GetAmount(tag);
		float num = Mathf.Min(reserve_amount, totalAmount);
		return amount + num;
	}
}
