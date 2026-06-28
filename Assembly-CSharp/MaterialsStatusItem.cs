using System;
using System.Collections.Generic;

public abstract class MaterialsStatusItem : StatusItem
{
	public MaterialsStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode overlay, SimViewMode second_overlay)
		: base(id, prefix, icon, icon_type, notification_type, allow_multiples, overlay, second_overlay)
	{
	}

	public abstract bool ShouldAdd(IFetchList fetch_list, Dictionary<Tag, float> remaining);
}
