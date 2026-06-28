using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class Message : ISaveLoadableJson
{
	public abstract string GetTitle();

	public abstract string GetSound();

	public abstract string GetMessageBody();

	public abstract string GetTooltip();

	public virtual Message.clickFn OnClick
	{
		get
		{
			return null;
		}
	}

	public virtual void OnCleanUp()
	{
	}

	public virtual bool IsValid()
	{
		return true;
	}

	public delegate void clickFn();
}
