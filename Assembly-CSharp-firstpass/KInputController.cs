using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class KInputController : IInputHandler
{
	public KInputController(bool is_gamepad)
	{
		this.mBindings = new List<KInputBinding>();
		this.mEvents = new List<KInputEvent>();
		this.mDirtyBindings = false;
		this.IsGamepad = is_gamepad;
		this.mAxis = new float[4];
		this.mActiveModifiers = Modifier.None;
		this.mActionState = new bool[229];
		this.mScrollState = new bool[2];
		this.inputHandler = new KInputHandler(this, this);
	}

	public KInputHandler inputHandler { get; set; }

	public bool IsGamepad { get; private set; }

	public void ClearBindings()
	{
		this.mBindings.Clear();
	}

	public void Bind(KKeyCode key_code, Modifier modifier, global::Action action)
	{
		this.mBindings.Add(new KInputBinding(key_code, modifier, action));
		this.mDirtyBindings = true;
	}

	public void QueueButtonEvent(KInputController.KeyDef key_def, bool is_down)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		bool[] mActionFlags = key_def.mActionFlags;
		key_def.mIsDown = is_down;
		InputEventType inputEventType = ((!is_down) ? InputEventType.KeyUp : InputEventType.KeyDown);
		for (int i = 0; i < mActionFlags.Length; i++)
		{
			if (mActionFlags[i])
			{
				this.mActionState[i] = is_down;
			}
		}
		KButtonEvent kbuttonEvent = new KButtonEvent(this, inputEventType, mActionFlags);
		this.mEvents.Add(kbuttonEvent);
		KInputManager.SetUserActive();
	}

	private void GenerateActionFlagTable()
	{
		this.mKeyDefLookup.Clear();
		foreach (KInputBinding kinputBinding in this.mBindings)
		{
			KInputController.KeyDefEntry keyDefEntry = new KInputController.KeyDefEntry(kinputBinding.mKeyCode, kinputBinding.mModifier);
			KInputController.KeyDef keyDef = null;
			if (!this.mKeyDefLookup.TryGetValue(keyDefEntry, out keyDef))
			{
				keyDef = new KInputController.KeyDef(kinputBinding.mKeyCode, kinputBinding.mModifier);
				this.mKeyDefLookup[keyDefEntry] = keyDef;
			}
			keyDef.mActionFlags[(int)kinputBinding.mAction] = true;
		}
		this.mKeyDefs = new KInputController.KeyDef[this.mKeyDefLookup.Count];
		this.mKeyDefLookup.Values.CopyTo(this.mKeyDefs, 0);
	}

	private bool GetKeyDown(KKeyCode key_code)
	{
		bool flag = false;
		if (key_code < KKeyCode.KleiKeys)
		{
			flag = Input.GetKeyDown((KeyCode)key_code);
		}
		else if (key_code != KKeyCode.MouseScrollUp)
		{
			if (key_code == KKeyCode.MouseScrollDown)
			{
				flag = this.mScrollState[1];
			}
		}
		else
		{
			flag = this.mScrollState[0];
		}
		return flag;
	}

	private bool GetKeyUp(KKeyCode key_code)
	{
		return key_code < KKeyCode.KleiKeys && Input.GetKeyUp((KeyCode)key_code);
	}

	public void CheckModifier(KKeyCode[] key_codes, Modifier modifier)
	{
		this.mActiveModifiers &= ~modifier;
		foreach (KKeyCode kkeyCode in key_codes)
		{
			if (this.GetKeyDown(kkeyCode) || Input.GetKey((KeyCode)kkeyCode))
			{
				this.mActiveModifiers |= modifier;
				break;
			}
		}
	}

	private void UpdateAxis()
	{
		this.mAxis[2] = Input.GetAxis("Mouse X");
		this.mAxis[3] = Input.GetAxis("Mouse Y");
	}

	private void UpdateModifiers()
	{
		this.CheckModifier(KInputController.altCodes, Modifier.Alt);
		this.CheckModifier(KInputController.ctrlCodes, Modifier.Ctrl);
		this.CheckModifier(KInputController.shiftCodes, Modifier.Shift);
		this.CheckModifier(KInputController.capsCodes, Modifier.CapsLock);
	}

	private void UpdateScrollStates()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		this.mScrollState[1] = axis < 0f;
		this.mScrollState[0] = axis > 0f;
	}

	public void ToggleKeyboard(bool active)
	{
		this.mIgnoreKeyboard = active;
	}

	public void ToggleMouse(bool active)
	{
		this.mIgnoreMouse = active;
	}

	public void Update()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (this.mDirtyBindings)
		{
			this.GenerateActionFlagTable();
			this.mDirtyBindings = false;
		}
		if (!this.IsGamepad)
		{
			this.UpdateScrollStates();
			this.UpdateAxis();
			this.UpdateModifiers();
			foreach (KInputController.KeyDef keyDef in this.mKeyDefs)
			{
				int mKeyCode = (int)keyDef.mKeyCode;
				if (!this.mIgnoreKeyboard || mKeyCode >= 323)
				{
					if (!this.mIgnoreMouse || ((mKeyCode < 323 || mKeyCode >= 330) && mKeyCode != 1001 && mKeyCode != 1002))
					{
						if (this.GetKeyDown(keyDef.mKeyCode))
						{
							bool flag = this.mActiveModifiers == keyDef.mModifier;
							if (flag)
							{
								this.QueueButtonEvent(keyDef, true);
							}
						}
						if (keyDef.mIsDown && this.GetKeyUp(keyDef.mKeyCode))
						{
							this.QueueButtonEvent(keyDef, false);
						}
					}
				}
			}
		}
	}

	public void Dispatch()
	{
		foreach (KInputEvent kinputEvent in this.mEvents)
		{
			this.inputHandler.HandleEvent(kinputEvent);
		}
		this.mEvents.Clear();
	}

	public bool IsActive(global::Action action)
	{
		return this.mActionState[(int)action];
	}

	public float GetAxis(Axis axis)
	{
		return this.mAxis[(int)axis];
	}

	public void HandleCancelInput()
	{
		foreach (KInputController.KeyDef keyDef in this.mKeyDefs)
		{
			if (this.IsGamepad || (keyDef.mIsDown && keyDef.mKeyCode < KKeyCode.KleiKeys && !Input.GetKey((KeyCode)keyDef.mKeyCode)))
			{
				this.QueueButtonEvent(keyDef, false);
			}
		}
		this.UpdateModifiers();
	}

	public KKeyCode GetInputForAction(global::Action action)
	{
		foreach (KInputBinding kinputBinding in this.mBindings)
		{
			if (kinputBinding.mAction == action)
			{
				return kinputBinding.mKeyCode;
			}
		}
		return KKeyCode.None;
	}

	private List<KInputBinding> mBindings;

	private List<KInputEvent> mEvents;

	private KInputController.KeyDef[] mKeyDefs = new KInputController.KeyDef[0];

	private bool mDirtyBindings;

	private float[] mAxis;

	private Modifier mActiveModifiers;

	private bool[] mActionState;

	private bool[] mScrollState;

	private bool mIgnoreKeyboard;

	private bool mIgnoreMouse;

	private Dictionary<KInputController.KeyDefEntry, KInputController.KeyDef> mKeyDefLookup = new Dictionary<KInputController.KeyDefEntry, KInputController.KeyDef>();

	private static readonly KKeyCode[] altCodes = new KKeyCode[]
	{
		KKeyCode.LeftAlt,
		KKeyCode.RightAlt
	};

	private static readonly KKeyCode[] ctrlCodes = new KKeyCode[]
	{
		KKeyCode.LeftControl,
		KKeyCode.RightControl
	};

	private static readonly KKeyCode[] shiftCodes = new KKeyCode[]
	{
		KKeyCode.LeftShift,
		KKeyCode.RightShift
	};

	private static readonly KKeyCode[] capsCodes = new KKeyCode[] { KKeyCode.CapsLock };

	private enum Scroll
	{
		Up,
		Down,
		NumStates
	}

	public struct KeyDefEntry
	{
		public KeyDefEntry(KKeyCode key_code, Modifier modifier)
		{
			this.mKeyCode = key_code;
			this.mModifier = modifier;
		}

		private void Print()
		{
			global::Debug.Log(this.mKeyCode.ToString() + this.mModifier.ToString(), null);
		}

		private KKeyCode mKeyCode;

		private Modifier mModifier;
	}

	[DebuggerDisplay("Key: {mKeyCode} Mod: {mModifier}")]
	public class KeyDef
	{
		public KeyDef(KKeyCode key_code, Modifier modifier)
		{
			this.mKeyCode = key_code;
			this.mModifier = modifier;
			this.mActionFlags = new bool[229];
		}

		public KKeyCode mKeyCode;

		public Modifier mModifier;

		public bool[] mActionFlags;

		public bool mIsDown;
	}
}
