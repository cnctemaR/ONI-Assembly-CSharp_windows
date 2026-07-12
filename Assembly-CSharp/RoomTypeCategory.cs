using System;

public class RoomTypeCategory : Resource
{
	public string colorName { get; private set; }

	public string icon { get; private set; }

	public RoomTypeCategory(string id, string name, string colorName, string icon)
		: base(id, name)
	{
		this.colorName = colorName;
		this.icon = icon;
	}
}
