using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class InputBindingsScreen : KModalScreen
{
	public override bool IsModal()
	{
		return true;
	}

	private bool IsKeyDown(KeyCode key_code)
	{
		return Input.GetKey(key_code) || Input.GetKeyDown(key_code);
	}

	private string GetModifierString(Modifier modifiers)
	{
		string text = string.Empty;
		foreach (object obj in Enum.GetValues(typeof(Modifier)))
		{
			Modifier modifier = (Modifier)((int)obj);
			if ((modifiers & modifier) != Modifier.None)
			{
				text = text + " + " + modifier.ToString();
			}
		}
		return text;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.entryPrefab.SetActive(false);
	}

	protected override void OnActivate()
	{
		this.CollectScreens();
		this.screenTitle.text = this.screens[this.activeScreen];
		this.closeButton.onClick += this.OnBack;
		this.backButton.onClick += this.OnBack;
		this.resetButton.onClick += this.OnReset;
		this.BuildDisplay();
	}

	private void CollectScreens()
	{
		this.screens.Clear();
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			if (bindingEntry.mScreen != null && bindingEntry.mRebindable && !this.screens.Contains(bindingEntry.mScreen))
			{
				if (bindingEntry.mScreen == "Root")
				{
					this.activeScreen = this.screens.Count;
				}
				this.screens.Add(bindingEntry.mScreen);
			}
		}
	}

	protected override void OnDeactivate()
	{
		GameInputMapping.SaveBindings();
		this.DestroyDisplay();
	}

	private void BuildDisplay()
	{
		this.screenTitle.text = this.screens[this.activeScreen];
		if (this.entryPool == null)
		{
			this.entryPool = new UIPool<HorizontalLayoutGroup>(this.entryPrefab.GetComponent<HorizontalLayoutGroup>());
		}
		this.DestroyDisplay();
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry binding = GameInputMapping.KeyBindings[i];
			if (binding.mScreen == this.screens[this.activeScreen] && binding.mRebindable)
			{
				GameObject gameObject = this.entryPool.GetFreeElement(this.parent, true).gameObject;
				LocText componentInChildren = gameObject.transform.GetChild(0).GetComponentInChildren<LocText>();
				componentInChildren.text = binding.mAction.ToString();
				LocText componentInChildren2 = gameObject.transform.GetChild(1).GetComponentInChildren<LocText>();
				componentInChildren2.text = binding.mKeyCode.ToString() + this.GetModifierString(binding.mModifier);
				KButton button = gameObject.GetComponentInChildren<KButton>();
				button.onClick += delegate
				{
					this.waitingForKeyPress = true;
					this.actionToRebind = binding.mAction;
					this.activeButton = button;
				};
			}
		}
	}

	private void DestroyDisplay()
	{
		this.entryPool.ClearAll();
	}

	private void Update()
	{
		if (this.waitingForKeyPress)
		{
			Modifier modifier = Modifier.None;
			modifier |= ((!this.IsKeyDown(KeyCode.LeftAlt) && !this.IsKeyDown(KeyCode.RightAlt)) ? Modifier.None : Modifier.Alt);
			modifier |= ((!this.IsKeyDown(KeyCode.LeftControl) && !this.IsKeyDown(KeyCode.RightControl)) ? Modifier.None : Modifier.Ctrl);
			modifier |= ((!this.IsKeyDown(KeyCode.LeftShift) && !this.IsKeyDown(KeyCode.RightShift)) ? Modifier.None : Modifier.Shift);
			modifier |= ((!this.IsKeyDown(KeyCode.CapsLock)) ? Modifier.None : Modifier.CapsLock);
			bool flag = false;
			for (int i = 0; i < InputBindingsScreen.validKeys.Length; i++)
			{
				KeyCode keyCode = InputBindingsScreen.validKeys[i];
				if (Input.GetKeyDown(keyCode))
				{
					KKeyCode kkeyCode = (KKeyCode)keyCode;
					this.Bind(kkeyCode, modifier);
					flag = true;
				}
			}
			if (!flag)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				KKeyCode kkeyCode2 = KKeyCode.None;
				if (axis < 0f)
				{
					kkeyCode2 = KKeyCode.MouseScrollDown;
				}
				else if (axis > 0f)
				{
					kkeyCode2 = KKeyCode.MouseScrollUp;
				}
				if (kkeyCode2 != KKeyCode.None)
				{
					this.Bind(kkeyCode2, modifier);
				}
			}
		}
	}

	private BindingEntry GetDuplicatedBinding(string activeScreen, BindingEntry new_binding)
	{
		BindingEntry bindingEntry = default(BindingEntry);
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[i];
			if ((bindingEntry2.mScreen == null || bindingEntry2.mScreen == activeScreen) && new_binding.IsBindingEqual(bindingEntry2))
			{
				bindingEntry = bindingEntry2;
				break;
			}
		}
		return bindingEntry;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.waitingForKeyPress)
		{
			e.Consumed = true;
			return;
		}
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	private void OnBack()
	{
		int num = this.NumUnboundActions();
		if (num == 0)
		{
			this.Deactivate();
		}
		else
		{
			string text;
			if (num == 1)
			{
				BindingEntry firstUnbound = this.GetFirstUnbound();
				text = string.Format(UI.FRONTEND.INPUTBINDINGSCREEN.UNBOUND_ACTION, firstUnbound.mAction.ToString());
			}
			else
			{
				text = UI.FRONTEND.INPUTBINDINGSCREEN.MULTIPLE_UNBOUND_ACTIONS;
			}
			this.confirmDialog = Util.KInstantiateUI(this.confirmPrefab.gameObject, this.transform.gameObject, false).GetComponent<ConfirmDialogScreen>();
			this.confirmDialog.PopupConfirmDialog(text, delegate
			{
				this.Deactivate();
			}, delegate
			{
				this.confirmDialog.Deactivate();
			}, null, null);
			this.confirmDialog.gameObject.SetActive(true);
		}
	}

	private int NumUnboundActions()
	{
		int num = 0;
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			if (bindingEntry.mKeyCode == KKeyCode.None)
			{
				num++;
			}
		}
		return num;
	}

	private BindingEntry GetFirstUnbound()
	{
		BindingEntry bindingEntry = default(BindingEntry);
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[i];
			if (bindingEntry2.mKeyCode == KKeyCode.None)
			{
				bindingEntry = bindingEntry2;
				break;
			}
		}
		return bindingEntry;
	}

	private void OnReset()
	{
		GameInputMapping.KeyBindings = (BindingEntry[])GameInputMapping.DefaultBindings.Clone();
		Global.Instance.GetInputManager().RebindControls();
		this.BuildDisplay();
	}

	public void OnPrevScreen()
	{
		if (this.activeScreen > 0)
		{
			this.activeScreen--;
			this.BuildDisplay();
		}
	}

	public void OnNextScreen()
	{
		if (this.activeScreen < this.screens.Count - 1)
		{
			this.activeScreen++;
			this.BuildDisplay();
		}
	}

	private void Bind(KKeyCode kkey_code, Modifier modifier)
	{
		BindingEntry bindingEntry = new BindingEntry(this.screens[this.activeScreen], GamepadButton.NumButtons, kkey_code, modifier, this.actionToRebind, true);
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[i];
			if (bindingEntry2.mAction == this.actionToRebind)
			{
				BindingEntry duplicatedBinding = this.GetDuplicatedBinding(this.screens[this.activeScreen], bindingEntry);
				GameInputMapping.KeyBindings[i] = bindingEntry;
				LocText componentInChildren = this.activeButton.GetComponentInChildren<LocText>();
				componentInChildren.text = bindingEntry.mKeyCode.ToString() + this.GetModifierString(bindingEntry.mModifier);
				if (duplicatedBinding.mAction != global::Action.Invalid && duplicatedBinding.mAction != this.actionToRebind)
				{
					this.confirmDialog = Util.KInstantiateUI(this.confirmPrefab.gameObject, this.transform.gameObject, false).GetComponent<ConfirmDialogScreen>();
					string text = duplicatedBinding.mKeyCode.ToString() + this.GetModifierString(duplicatedBinding.mModifier);
					string text2 = string.Format(UI.FRONTEND.INPUTBINDINGSCREEN.DUPLICATE, duplicatedBinding.mAction.ToString(), text);
					this.Unbind(duplicatedBinding.mAction);
					this.confirmDialog.PopupConfirmDialog(text2, null, null, null, null);
					this.confirmDialog.gameObject.SetActive(true);
				}
				Global.Instance.GetInputManager().RebindControls();
				this.waitingForKeyPress = false;
				this.actionToRebind = global::Action.NumActions;
				this.activeButton = null;
				this.BuildDisplay();
				break;
			}
		}
	}

	private void Unbind(global::Action action)
	{
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			if (bindingEntry.mAction == action)
			{
				bindingEntry.mKeyCode = KKeyCode.None;
				bindingEntry.mModifier = Modifier.None;
				GameInputMapping.KeyBindings[i] = bindingEntry;
			}
		}
	}

	[SerializeField]
	private OptionsMenuScreen optionsScreen;

	[SerializeField]
	private ConfirmDialogScreen confirmPrefab;

	public KButton backButton;

	public KButton resetButton;

	public KButton closeButton;

	private bool waitingForKeyPress;

	private global::Action actionToRebind = global::Action.NumActions;

	private KButton activeButton;

	[SerializeField]
	private LocText screenTitle;

	[SerializeField]
	private GameObject parent;

	[SerializeField]
	private GameObject entryPrefab;

	private ConfirmDialogScreen confirmDialog;

	private int activeScreen = -1;

	private List<string> screens = new List<string>();

	private UIPool<HorizontalLayoutGroup> entryPool;

	private static readonly KeyCode[] validKeys = new KeyCode[]
	{
		KeyCode.Backspace,
		KeyCode.Tab,
		KeyCode.Clear,
		KeyCode.Return,
		KeyCode.Pause,
		KeyCode.Escape,
		KeyCode.Space,
		KeyCode.Exclaim,
		KeyCode.DoubleQuote,
		KeyCode.Hash,
		KeyCode.Dollar,
		KeyCode.Ampersand,
		KeyCode.Quote,
		KeyCode.LeftParen,
		KeyCode.RightParen,
		KeyCode.Asterisk,
		KeyCode.Plus,
		KeyCode.Comma,
		KeyCode.Minus,
		KeyCode.Period,
		KeyCode.Slash,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9,
		KeyCode.Colon,
		KeyCode.Semicolon,
		KeyCode.Less,
		KeyCode.Equals,
		KeyCode.Greater,
		KeyCode.Question,
		KeyCode.At,
		KeyCode.LeftBracket,
		KeyCode.Backslash,
		KeyCode.RightBracket,
		KeyCode.Caret,
		KeyCode.Underscore,
		KeyCode.BackQuote,
		KeyCode.A,
		KeyCode.B,
		KeyCode.C,
		KeyCode.D,
		KeyCode.E,
		KeyCode.F,
		KeyCode.G,
		KeyCode.H,
		KeyCode.I,
		KeyCode.J,
		KeyCode.K,
		KeyCode.L,
		KeyCode.M,
		KeyCode.N,
		KeyCode.O,
		KeyCode.P,
		KeyCode.Q,
		KeyCode.R,
		KeyCode.S,
		KeyCode.T,
		KeyCode.U,
		KeyCode.V,
		KeyCode.W,
		KeyCode.X,
		KeyCode.Y,
		KeyCode.Z,
		KeyCode.Delete,
		KeyCode.Keypad0,
		KeyCode.Keypad1,
		KeyCode.Keypad2,
		KeyCode.Keypad3,
		KeyCode.Keypad4,
		KeyCode.Keypad5,
		KeyCode.Keypad6,
		KeyCode.Keypad7,
		KeyCode.Keypad8,
		KeyCode.Keypad9,
		KeyCode.KeypadPeriod,
		KeyCode.KeypadDivide,
		KeyCode.KeypadMultiply,
		KeyCode.KeypadMinus,
		KeyCode.KeypadPlus,
		KeyCode.KeypadEnter,
		KeyCode.KeypadEquals,
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.RightArrow,
		KeyCode.LeftArrow,
		KeyCode.Insert,
		KeyCode.Home,
		KeyCode.End,
		KeyCode.PageUp,
		KeyCode.PageDown,
		KeyCode.F1,
		KeyCode.F2,
		KeyCode.F3,
		KeyCode.F4,
		KeyCode.F5,
		KeyCode.F6,
		KeyCode.F7,
		KeyCode.F8,
		KeyCode.F9,
		KeyCode.F10,
		KeyCode.F11,
		KeyCode.F12,
		KeyCode.F13,
		KeyCode.F14,
		KeyCode.F15,
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6
	};
}
