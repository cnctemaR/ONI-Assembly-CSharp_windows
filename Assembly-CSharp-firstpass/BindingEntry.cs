using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct BindingEntry
{
	public BindingEntry(string screen, GamepadButton button, KKeyCode key_code, Modifier modifier, global::Action action, bool rebindable = true)
	{
		this.mScreen = screen;
		this.mButton = button;
		this.mKeyCode = key_code;
		this.mAction = action;
		this.mModifier = modifier;
		this.mRebindable = rebindable;
	}

	public static KKeyCode GetGamepadKeyCode(int gamepad_number, GamepadButton button)
	{
		switch (gamepad_number)
		{
		case 0:
			return (KKeyCode)(button + 350);
		case 1:
			return (KKeyCode)(button + 370);
		case 2:
			return (KKeyCode)(button + 390);
		case 3:
			return (KKeyCode)(button + 410);
		default:
			DebugUtil.Assert(false, "Assert!");
			return KKeyCode.None;
		}
	}

	public bool IsBindingEqual(BindingEntry other)
	{
		return this.mButton == other.mButton && this.mKeyCode == other.mKeyCode && this.mModifier == other.mModifier;
	}

	public override bool Equals(object o)
	{
		if (!(o is BindingEntry))
		{
			return false;
		}
		BindingEntry bindingEntry = (BindingEntry)o;
		return this == bindingEntry;
	}

	public override int GetHashCode()
	{
		return (int)(this.mButton ^ (GamepadButton)this.mKeyCode ^ (GamepadButton)this.mAction);
	}

	public static bool operator ==(BindingEntry a, BindingEntry b)
	{
		return a.mScreen == b.mScreen && a.mButton == b.mButton && a.mKeyCode == b.mKeyCode && a.mAction == b.mAction && a.mModifier == b.mModifier && a.mRebindable == b.mRebindable;
	}

	public static bool operator !=(BindingEntry a, BindingEntry b)
	{
		return !(a == b);
	}

	[JsonIgnore]
	public string mScreen;

	[JsonIgnore]
	public bool mRebindable;

	[JsonConverter(typeof(StringEnumConverter))]
	public GamepadButton mButton;

	[JsonConverter(typeof(StringEnumConverter))]
	public KKeyCode mKeyCode;

	[JsonConverter(typeof(StringEnumConverter))]
	public global::Action mAction;

	[JsonConverter(typeof(StringEnumConverter))]
	public Modifier mModifier;
}
