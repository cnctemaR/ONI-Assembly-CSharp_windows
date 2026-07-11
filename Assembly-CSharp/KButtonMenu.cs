using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KButtonMenu : KScreen
{
	protected override void OnActivate()
	{
		this.ConsumeMouseScroll = this.ShouldConsumeMouseScroll;
		this.RefreshButtons();
	}

	public void SetButtons(IList<KButtonMenu.ButtonInfo> buttons)
	{
		this.buttons = buttons;
		if (this.activateOnSpawn)
		{
			this.RefreshButtons();
		}
	}

	public virtual void RefreshButtons()
	{
		if (this.buttonObjects != null)
		{
			for (int i = 0; i < this.buttonObjects.Length; i++)
			{
				global::UnityEngine.Object.Destroy(this.buttonObjects[i]);
			}
			this.buttonObjects = null;
		}
		if (this.buttons == null)
		{
			return;
		}
		this.buttonObjects = new GameObject[this.buttons.Count];
		for (int j = 0; j < this.buttons.Count; j++)
		{
			KButtonMenu.ButtonInfo binfo = this.buttons[j];
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab, Vector3.zero, Quaternion.identity);
			this.buttonObjects[j] = gameObject;
			Transform transform = ((!(this.buttonParent != null)) ? base.transform : this.buttonParent);
			gameObject.transform.SetParent(transform, false);
			gameObject.SetActive(true);
			gameObject.name = binfo.text + "Button";
			LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>(true);
			if (componentsInChildren != null)
			{
				foreach (LocText locText in componentsInChildren)
				{
					locText.text = ((!(locText.name == "Hotkey")) ? binfo.text : GameUtil.GetActionString(binfo.shortcutKey));
					locText.color = ((!binfo.isEnabled) ? new Color(0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f));
				}
			}
			ToolTip componentInChildren = gameObject.GetComponentInChildren<ToolTip>();
			if (binfo.toolTip != null && binfo.toolTip != string.Empty && componentInChildren != null)
			{
				componentInChildren.toolTip = binfo.toolTip;
			}
			KButtonMenu screen = this;
			KButton button = gameObject.GetComponent<KButton>();
			button.isInteractable = binfo.isEnabled;
			if (binfo.popupOptions == null && binfo.onPopulatePopup == null)
			{
				UnityAction onClick = binfo.onClick;
				global::System.Action action = delegate
				{
					onClick();
					if (!this.keepMenuOpen && screen != null)
					{
						screen.Deactivate();
					}
				};
				button.onClick += action;
			}
			else
			{
				button.onClick += delegate
				{
					this.SetupPopupMenu(binfo, button);
				};
			}
			binfo.uibutton = button;
			if (binfo.onHover != null)
			{
			}
		}
		this.Update();
	}

	private Button.ButtonClickedEvent SetupPopupMenu(KButtonMenu.ButtonInfo binfo, KButton button)
	{
		KButtonMenu.<SetupPopupMenu>c__AnonStorey2 <SetupPopupMenu>c__AnonStorey = new KButtonMenu.<SetupPopupMenu>c__AnonStorey2();
		<SetupPopupMenu>c__AnonStorey.binfo = binfo;
		<SetupPopupMenu>c__AnonStorey.button = button;
		<SetupPopupMenu>c__AnonStorey.$this = this;
		Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
		UnityAction unityAction = delegate
		{
			List<KButtonMenu.ButtonInfo> list = new List<KButtonMenu.ButtonInfo>();
			if (<SetupPopupMenu>c__AnonStorey.binfo.onPopulatePopup != null)
			{
				<SetupPopupMenu>c__AnonStorey.binfo.popupOptions = <SetupPopupMenu>c__AnonStorey.binfo.onPopulatePopup();
			}
			string[] popupOptions = <SetupPopupMenu>c__AnonStorey.binfo.popupOptions;
			for (int i = 0; i < popupOptions.Length; i++)
			{
				string text = popupOptions[i];
				string delegate_str = text;
				list.Add(new KButtonMenu.ButtonInfo(delegate_str, delegate
				{
					<SetupPopupMenu>c__AnonStorey.binfo.onPopupClick(delegate_str);
					if (!<SetupPopupMenu>c__AnonStorey.keepMenuOpen)
					{
						<SetupPopupMenu>c__AnonStorey.Deactivate();
					}
				}, global::Action.NumActions, null, null, null, true, null, null, null));
			}
			KButtonMenu component = Util.KInstantiate(ScreenPrefabs.Instance.ButtonGrid.gameObject, null, null).GetComponent<KButtonMenu>();
			component.SetButtons(list.ToArray());
			RootMenu.Instance.AddSubMenu(component);
			Game.Instance.LocalPlayer.ScreenManager.ActivateScreen(component.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			Vector3 vector = default(Vector3);
			if (Util.IsOnLeftSideOfScreen(<SetupPopupMenu>c__AnonStorey.button.transform.GetPosition()))
			{
				vector.x = <SetupPopupMenu>c__AnonStorey.button.GetComponent<RectTransform>().rect.width * 0.25f;
			}
			else
			{
				vector.x = -<SetupPopupMenu>c__AnonStorey.button.GetComponent<RectTransform>().rect.width * 0.25f;
			}
			component.transform.SetPosition(<SetupPopupMenu>c__AnonStorey.button.transform.GetPosition() + vector);
		};
		<SetupPopupMenu>c__AnonStorey.binfo.onClick = unityAction;
		buttonClickedEvent.AddListener(unityAction);
		return buttonClickedEvent;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.buttons == null)
		{
			return;
		}
		for (int i = 0; i < this.buttons.Count; i++)
		{
			KButtonMenu.ButtonInfo buttonInfo = this.buttons[i];
			if (e.TryConsume(buttonInfo.shortcutKey))
			{
				this.buttonObjects[i].GetComponent<KButton>().PlayPointerDownSound();
				this.buttonObjects[i].GetComponent<KButton>().SignalClick(KKeyCode.Mouse0);
				break;
			}
		}
		base.OnKeyDown(e);
	}

	protected override void OnPrefabInit()
	{
		base.Subscribe(315865555, new Action<object>(this.OnSetActivator));
	}

	private void OnSetActivator(object data)
	{
		this.go = (GameObject)data;
		this.Update();
	}

	protected override void OnDeactivate()
	{
	}

	private void Update()
	{
		if (!this.followGameObject || this.go == null || base.canvas == null)
		{
			return;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(this.go.transform.GetPosition());
		RectTransform component = base.GetComponent<RectTransform>();
		RectTransform component2 = base.canvas.GetComponent<RectTransform>();
		if (component != null)
		{
			component.anchoredPosition = new Vector2(vector.x * component2.sizeDelta.x - component2.sizeDelta.x * 0.5f, vector.y * component2.sizeDelta.y - component2.sizeDelta.y * 0.5f);
		}
	}

	[SerializeField]
	protected bool followGameObject;

	[SerializeField]
	protected bool keepMenuOpen;

	[SerializeField]
	private Transform buttonParent;

	public GameObject buttonPrefab;

	public bool ShouldConsumeMouseScroll;

	[NonSerialized]
	public GameObject[] buttonObjects;

	protected GameObject go;

	protected IList<KButtonMenu.ButtonInfo> buttons;

	public class ButtonInfo
	{
		public ButtonInfo(string text = null, UnityAction on_click = null, global::Action shortcut_key = global::Action.NumActions, KButtonMenu.ButtonInfo.HoverCallback on_hover = null, string tool_tip = null, GameObject visualizer = null, bool is_enabled = true, string[] popup_options = null, Action<string> on_popup_click = null, Func<string[]> on_populate_popup = null)
		{
			this.text = text;
			this.shortcutKey = shortcut_key;
			this.onClick = on_click;
			this.onHover = on_hover;
			this.visualizer = visualizer;
			this.toolTip = tool_tip;
			this.isEnabled = is_enabled;
			this.uibutton = null;
			this.popupOptions = popup_options;
			this.onPopupClick = on_popup_click;
			this.onPopulatePopup = on_populate_popup;
		}

		public ButtonInfo(string text, global::Action shortcutKey, UnityAction onClick, KButtonMenu.ButtonInfo.HoverCallback onHover = null, object userData = null)
		{
			this.text = text;
			this.shortcutKey = shortcutKey;
			this.onClick = onClick;
			this.onHover = onHover;
			this.userData = userData;
			this.visualizer = null;
			this.uibutton = null;
		}

		public ButtonInfo(string text, GameObject visualizer, global::Action shortcutKey, UnityAction onClick, KButtonMenu.ButtonInfo.HoverCallback onHover = null, object userData = null)
		{
			this.text = text;
			this.shortcutKey = shortcutKey;
			this.onClick = onClick;
			this.onHover = onHover;
			this.visualizer = visualizer;
			this.userData = userData;
			this.uibutton = null;
		}

		public string text;

		public global::Action shortcutKey;

		public GameObject visualizer;

		public UnityAction onClick;

		public KButtonMenu.ButtonInfo.HoverCallback onHover;

		public FMODAsset clickSound;

		public KButton uibutton;

		public string toolTip;

		public bool isEnabled = true;

		public string[] popupOptions;

		public Action<string> onPopupClick;

		public Func<string[]> onPopulatePopup;

		public object userData;

		public delegate void HoverCallback(GameObject hoverTarget);

		public delegate void Callback();
	}
}
