using System;

[Serializable]
public struct ButtonMenuTextOverride
{
	public bool IsValid
	{
		get
		{
			return !string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.ToolTip);
		}
	}

	public bool HasCancelText
	{
		get
		{
			return !string.IsNullOrEmpty(this.CancelText) && !string.IsNullOrEmpty(this.CancelToolTip);
		}
	}

	public LocString Text;

	public LocString CancelText;

	public LocString ToolTip;

	public LocString CancelToolTip;
}
