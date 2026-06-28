using System;
using STRINGS;

internal struct EffectorEntry
{
	public EffectorEntry(string name, float value)
	{
		this.name = name;
		this.value = value;
		this.count = 1;
	}

	public override string ToString()
	{
		string text = "";
		if (this.count > 1)
		{
			text = string.Format(UI.OVERLAYS.DECOR.COUNT, this.count);
		}
		return string.Format(UI.OVERLAYS.DECOR.ENTRY, GameUtil.GetFormattedDecor(this.value), this.name, text);
	}

	public string name;

	public int count;

	public float value;
}
