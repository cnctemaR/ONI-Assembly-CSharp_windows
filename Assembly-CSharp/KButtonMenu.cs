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
			KButtonMenu.ButtonInfo buttonInfo = this.buttons[j];
			GameObject gameObject = global::UnityEngine.Object.Instantiate(this.buttonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			this.buttonObjects[j] = gameObject;
			Transform transform = ((!(this.buttonParent != null)) ? this.transform : this.buttonParent);
			gameObject.transform.SetParent(transform, false);
			gameObject.SetActive(true);
			gameObject.name = buttonInfo.text + "Button";
			LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>(true);
			if (componentsInChildren != null)
			{
				foreach (LocText locText in componentsInChildren)
				{
					locText.text = ((!(locText.name == "Hotkey")) ? buttonInfo.text : GameUtil.GetActionString(buttonInfo.shortcutKey));
					locText.color = ((!buttonInfo.isEnabled) ? new Color(0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f));
				}
			}
			RawImage componentInChildren = gameObject.GetComponentInChildren<RawImage>();
			if (componentInChildren != null && buttonInfo.visualizer != null)
			{
				Portrait component = Util.KInstantiate(EntityPrefabs.Instance.Portrait, SceneOrganizer.Instance.GetFolder(Folder.Portraits), null).GetComponent<Portrait>();
				RectTransform component2 = componentInChildren.GetComponent<RectTransform>();
				componentInChildren.texture = component.CreateTexture((int)component2.rect.width, (int)component2.rect.height);
				component.SetTarget(buttonInfo.visualizer);
				component.destroyTargetOnCleanup = true;
				this.buttons[j].portrait = component;
			}
			ToolTip componentInChildren2 = gameObject.GetComponentInChildren<ToolTip>();
			if (buttonInfo.toolTip != null && buttonInfo.toolTip != string.Empty && componentInChildren2 != null)
			{
				componentInChildren2.toolTip = buttonInfo.toolTip;
			}
			KButtonMenu screen = this;
			Button component3 = gameObject.GetComponent<Button>();
			component3.interactable = buttonInfo.isEnabled;
			if (buttonInfo.popupOptions == null && buttonInfo.onPopulatePopup == null)
			{
				UnityAction onClick = buttonInfo.onClick;
				UnityAction unityAction = delegate
				{
					onClick();
					if (!this.keepMenuOpen && screen != null)
					{
						screen.Deactivate();
					}
				};
				Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
				buttonClickedEvent.AddListener(unityAction);
				component3.onClick = buttonClickedEvent;
				buttonInfo.onClick = unityAction;
			}
			else
			{
				component3.onClick = this.SetupPopupMenu(buttonInfo, component3);
			}
			buttonInfo.uibutton = component3;
			if (buttonInfo.onHover != null)
			{
			}
		}
		this.Update();
		for (int l = 0; l < this.buttons.Count - 1; l++)
		{
			for (int m = l + 1; m < this.buttons.Count; m++)
			{
				if (this.buttons[l].shortcutKey != global::Action.NumActions && this.buttons[l].shortcutKey == this.buttons[m].shortcutKey)
				{
					Output.LogWarning(new object[] { this.buttons[l].text + " has the same shortcut key as " + this.buttons[m].text });
				}
			}
		}
	}

	private Button.ButtonClickedEvent SetupPopupMenu(KButtonMenu.ButtonInfo binfo, Button button)
	{
		Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
		UnityAction unityAction = delegate
		{
			List<KButtonMenu.ButtonInfo> list = new List<KButtonMenu.ButtonInfo>();
			if (binfo.onPopulatePopup != null)
			{
				binfo.popupOptions = binfo.onPopulatePopup();
			}
			string[] popupOptions = binfo.popupOptions;
			for (int i = 0; i < popupOptions.Length; i++)
			{
				string text = popupOptions[i];
				string delegate_str = text;
				list.Add(new KButtonMenu.ButtonInfo(delegate_str, delegate
				{
					binfo.onPopupClick(delegate_str);
					if (!this.keepMenuOpen)
					{
						this.Deactivate();
					}
				}, global::Action.NumActions, null, null, null, true, null, null, null));
			}
			KButtonMenu component = Util.KInstantiate(ScreenPrefabs.Instance.ButtonGrid.gameObject, null, null).GetComponent<KButtonMenu>();
			component.SetButtons(list.ToArray());
			RootMenu.Instance.AddSubMenu(component);
			Game.Instance.LocalPlayer.ScreenManager.ActivateScreen(component.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			Vector3 vector = default(Vector3);
			if (Util.IsOnLeftSideOfScreen(button.transform.position))
			{
				vector.x = button.GetComponent<RectTransform>().rect.width * 0.25f;
			}
			else
			{
				vector.x = -button.GetComponent<RectTransform>().rect.width * 0.25f;
			}
			component.transform.SetPosition(button.transform.position + vector);
		};
		binfo.onClick = unityAction;
		buttonClickedEvent.AddListener(unityAction);
		return buttonClickedEvent;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.buttons == null)
		{
			return;
		}
		foreach (KButtonMenu.ButtonInfo buttonInfo in this.buttons)
		{
			if (e.TryConsume(buttonInfo.shortcutKey))
			{
				buttonInfo.onClick();
				break;
			}
		}
		base.OnKeyDown(e);
	}

	protected override void OnPrefabInit()
	{
		this.Subscribe(315865555, new EventSystem.EventHandler(this.OnSetActivator));
	}

	private void OnSetActivator(object data)
	{
		this.go = (GameObject)data;
		this.Update();
	}

	protected override void OnDeactivate()
	{
		if (this.buttons == null)
		{
			return;
		}
		foreach (KButtonMenu.ButtonInfo buttonInfo in this.buttons)
		{
			if (buttonInfo != null)
			{
				if (buttonInfo.portrait != null)
				{
					global::UnityEngine.Object.Destroy(buttonInfo.portrait.gameObject);
				}
			}
		}
	}

	private void Update()
	{
		if (!this.followGameObject || this.go == null || base.canvas == null)
		{
			return;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(this.go.transform.position);
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
			this.portrait = null;
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
			this.portrait = null;
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
			this.portrait = null;
			this.uibutton = null;
		}

		public string text;

		public global::Action shortcutKey;

		public GameObject visualizer;

		public Portrait portrait;

		public UnityAction onClick;

		public KButtonMenu.ButtonInfo.HoverCallback onHover;

		public FMODAsset clickSound;

		public Button uibutton;

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
