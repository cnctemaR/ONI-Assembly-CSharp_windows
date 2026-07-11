using System;

public class MaterialsStatusItem : StatusItem
{
	public MaterialsStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString overlay)
		: base(id, prefix, icon, icon_type, notification_type, allow_multiples, overlay, true, 63486)
	{
	}
}
