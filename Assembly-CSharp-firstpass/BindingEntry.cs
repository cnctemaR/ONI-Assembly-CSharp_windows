using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct BindingEntry : IEquatable<BindingEntry>
{
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
			DebugUtil.Assert(false);
			return KKeyCode.None;
		}
	}

	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, global::Action action, bool rebindable = true, bool ignore_root_conflicts = false)
	{
		this = new BindingEntry(group, button, key_code, modifier, action, rebindable, ignore_root_conflicts, null);
	}

	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, global::Action action, string[] dlcIds)
	{
		this = new BindingEntry(group, button, key_code, modifier, action, true, false, dlcIds);
	}

	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, global::Action action, bool rebindable, bool ignore_root_conflicts, string[] dlcIds)
	{
		this.mGroup = group;
		this.mButton = button;
		this.mKeyCode = key_code;
		this.mAction = action;
		this.mModifier = modifier;
		this.mRebindable = rebindable;
		this.mIgnoreRootConflics = ignore_root_conflicts;
		this.dlcIds = dlcIds;
		if (this.dlcIds == null)
		{
			this.dlcIds = DlcManager.AVAILABLE_ALL_VERSIONS;
		}
	}

	public bool Equals(BindingEntry other)
	{
		return this == other;
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

	[JsonIgnore]
	public string mGroup;

	[JsonIgnore]
	public bool mRebindable;

	[JsonIgnore]
	public bool mIgnoreRootConflics;

	[JsonIgnore]
	public string[] dlcIds;

	[JsonConverter(typeof(StringEnumConverter))]
	public GamepadButton mButton;

	[JsonConverter(typeof(StringEnumConverter))]
	public KKeyCode mKeyCode;

	[JsonConverter(typeof(StringEnumConverter))]
	public global::Action mAction;

	[JsonConverter(typeof(StringEnumConverter))]
	public Modifier mModifier;
}
