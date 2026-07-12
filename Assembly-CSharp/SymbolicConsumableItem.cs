using System;

public class SymbolicConsumableItem : IConsumableUIItem
{
	public SymbolicConsumableItem(string id, string name, int majorOrder, int minorOrder, bool display, string overrideSpriteName, Func<bool> revealTest)
	{
		this.id = id;
		this.name = name;
		this.majorOrder = majorOrder;
		this.minorOrder = minorOrder;
		this.display = display;
		this.overrideSpriteName = overrideSpriteName;
		this.revealTest = revealTest;
	}

	string IConsumableUIItem.ConsumableId
	{
		get
		{
			return this.id;
		}
	}

	string IConsumableUIItem.ConsumableName
	{
		get
		{
			return this.name;
		}
	}

	int IConsumableUIItem.MajorOrder
	{
		get
		{
			return this.majorOrder;
		}
	}

	int IConsumableUIItem.MinorOrder
	{
		get
		{
			return this.minorOrder;
		}
	}

	bool IConsumableUIItem.Display
	{
		get
		{
			return this.display;
		}
	}

	string IConsumableUIItem.OverrideSpriteName()
	{
		return this.overrideSpriteName;
	}

	bool IConsumableUIItem.RevealTest()
	{
		return this.revealTest();
	}

	private string id;

	private string name;

	private int majorOrder;

	private int minorOrder;

	private bool display;

	private string overrideSpriteName;

	private Func<bool> revealTest;
}
