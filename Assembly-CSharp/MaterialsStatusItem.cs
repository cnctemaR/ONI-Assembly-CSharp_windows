using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MaterialsStatusItem : StatusItem
{
	public MaterialsStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode overlay)
		: base(id, prefix, icon, icon_type, notification_type, allow_multiples, overlay, true, 30718)
	{
	}

	public abstract bool ShouldAdd(IFetchList fetch_list, Dictionary<Tag, float> remaining);

	protected float GetAmountInStorage(Storage storage, Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < storage.items.Count; i++)
		{
			GameObject gameObject = storage.items[i];
			if (!(gameObject == null))
			{
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (!(component == null))
				{
					if (component.KPrefabID.HasTag(tag))
					{
						num += component.TotalAmount;
					}
				}
			}
		}
		return num;
	}
}
