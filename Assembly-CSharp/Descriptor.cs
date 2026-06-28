using System;

public struct Descriptor
{
	public void SetupDescriptor(string txt, string tooltip)
	{
		this.text = txt;
		this.tooltipText = GameUtil.StripTextFormatting(tooltip);
	}

	public string text;

	public string tooltipText;
}
