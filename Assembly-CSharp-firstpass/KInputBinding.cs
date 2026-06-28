using System;

public class KInputBinding
{
	public KInputBinding(KKeyCode key_code, Modifier modifier, global::Action action)
	{
		this.mKeyCode = key_code;
		this.mAction = action;
		this.mModifier = modifier;
	}

	public KKeyCode mKeyCode;

	public global::Action mAction;

	public Modifier mModifier;
}
