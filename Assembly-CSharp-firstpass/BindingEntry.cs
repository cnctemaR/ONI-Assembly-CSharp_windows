using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct BindingEntry
{
	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, global::Action action, bool rebindable = true, bool ignore_root_conflicts = false)
	{
		this.mGroup = group;
		this.mButton = button;
		this.mKeyCode = key_code;
		this.mAction = action;
		this.mModifier = modifier;
		this.mRebindable = rebindable;
		this.mIgnoreRootConflics = ignore_root_conflicts;
	}

	public static KKeyCode GetGamepadKeyCode(int gamepad_number, GamepadButton button)
	{
		KKeyCode kkeyCode;
		switch (gamepad_number)
		{
		case 0:
			kkeyCode = (KKeyCode)(button + 350);
			break;
		case 1:
			kkeyCode = (KKeyCode)(button + 370);
			break;
		case 2:
			kkeyCode = (KKeyCode)(button + 390);
			break;
		case 3:
			kkeyCode = (KKeyCode)(button + 410);
			break;
		default:
			DebugUtil.Assert(false, "Assert!");
			kkeyCode = KKeyCode.None;
			break;
		}
		return kkeyCode;
	}

	public static bool operator ==(BindingEntry a, BindingEntry b)
	{
		return a.mGroup == b.mGroup && a.mButton == b.mButton && a.mKeyCode == b.mKeyCode && a.mAction == b.mAction && a.mModifier == b.mModifier && a.mRebindable == b.mRebindable;
	}

	public bool IsBindingEqual(BindingEntry other)
	{
		return this.mButton == other.mButton && this.mKeyCode == other.mKeyCode && this.mModifier == other.mModifier;
	}

	public static bool operator !=(BindingEntry a, BindingEntry b)
	{
		return !(a == b);
	}

	public override bool Equals(object o)
	{
		bool flag;
		if (!(o is BindingEntry))
		{
			flag = false;
		}
		else
		{
			BindingEntry bindingEntry = (BindingEntry)o;
			flag = this == bindingEntry;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return (int)(this.mButton ^ (GamepadButton)this.mKeyCode ^ (GamepadButton)this.mAction);
	}

	[JsonIgnore]
	public string mGroup;

	[JsonIgnore]
	public bool mRebindable;

	[JsonIgnore]
	public bool mIgnoreRootConflics;

	[JsonConverter(typeof(StringEnumConverter))]
	public GamepadButton mButton;

	[JsonConverter(typeof(StringEnumConverter))]
	public KKeyCode mKeyCode;

	[JsonConverter(typeof(StringEnumConverter))]
	public global::Action mAction;

	[JsonConverter(typeof(StringEnumConverter))]
	public Modifier mModifier;
}
