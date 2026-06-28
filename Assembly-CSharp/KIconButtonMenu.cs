using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KIconButtonMenu : KScreen
{
	protected override void OnActivate()
	{
		base.OnActivate();
		this.RefreshButtons();
	}

	public void SetButtons(IList<KIconButtonMenu.ButtonInfo> buttons)
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
		if (this.buttons == null || this.buttons.Count == 0)
		{
			return;
		}
		this.buttonObjects = new GameObject[this.buttons.Count];
		for (int j = 0; j < this.buttons.Count; j++)
		{
			KIconButtonMenu.ButtonInfo buttonInfo = this.buttons[j];
			if (buttonInfo != null)
			{
				GameObject binstance = global::UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab, Vector3.zero, Quaternion.identity);
				buttonInfo.buttonGo = binstance;
				this.buttonObjects[j] = binstance;
				Transform transform = ((!(this.buttonParent != null)) ? base.transform : this.buttonParent);
				binstance.transform.SetParent(transform, false);
				binstance.SetActive(true);
				binstance.name = buttonInfo.text + "Button";
				KButton component = binstance.GetComponent<KButton>();
				if (component != null && buttonInfo.onClick != null)
				{
					component.onClick += buttonInfo.onClick;
				}
				Image image = null;
				if (component)
				{
					image = component.fgImage;
				}
				if (image != null)
				{
					image.gameObject.SetActive(false);
					foreach (Sprite sprite in this.icons)
					{
						if (sprite != null && sprite.name == buttonInfo.iconName)
						{
							image.sprite = sprite;
							image.gameObject.SetActive(true);
							break;
						}
					}
				}
				if (buttonInfo.texture != null)
				{
					RawImage componentInChildren = binstance.GetComponentInChildren<RawImage>();
					if (componentInChildren != null)
					{
						componentInChildren.gameObject.SetActive(true);
						componentInChildren.texture = buttonInfo.texture;
					}
				}
				ToolTip componentInChildren2 = binstance.GetComponentInChildren<ToolTip>();
				if (buttonInfo.text != null && buttonInfo.text != string.Empty && componentInChildren2 != null)
				{
					componentInChildren2.toolTip = buttonInfo.GetTooltipText();
					LocText componentInChildren3 = binstance.GetComponentInChildren<LocText>();
					if (componentInChildren3 != null)
					{
						componentInChildren3.text = buttonInfo.text;
					}
				}
				if (buttonInfo.onToolTip != null)
				{
					componentInChildren2.OnToolTip = buttonInfo.onToolTip;
				}
				KIconButtonMenu screen = this;
				global::System.Action onClick = buttonInfo.onClick;
				global::System.Action action = delegate
				{
					onClick.Signal();
					if (!this.keepMenuOpen && screen != null)
					{
						screen.Deactivate();
					}
					if (binstance != null)
					{
						KToggle component3 = binstance.GetComponent<KToggle>();
						if (component3 != null)
						{
							this.SelectToggle(component3);
						}
					}
				};
				KToggle componentInChildren4 = binstance.GetComponentInChildren<KToggle>();
				if (componentInChildren4 != null)
				{
					componentInChildren4.onRefresh += buttonInfo.onRefresh;
					ToggleGroup component2 = base.GetComponent<ToggleGroup>();
					if (component2 == null)
					{
						component2 = this.externalToggleGroup;
					}
					componentInChildren4.group = component2;
					componentInChildren4.onClick += action;
					Navigation navigation = componentInChildren4.navigation;
					navigation.mode = ((!this.automaticNavigation) ? Navigation.Mode.None : Navigation.Mode.Automatic);
					componentInChildren4.navigation = navigation;
				}
				else
				{
					KBasicToggle componentInChildren5 = binstance.GetComponentInChildren<KBasicToggle>();
					if (componentInChildren5 != null)
					{
						componentInChildren5.onClick += action;
					}
				}
				if (component != null)
				{
					component.isInteractable = buttonInfo.isInteractable;
				}
				buttonInfo.onCreate.Signal(buttonInfo);
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

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.buttons == null)
		{
			return;
		}
		if (!base.gameObject.activeSelf || !base.enabled)
		{
			return;
		}
		for (int i = 0; i < this.buttons.Count; i++)
		{
			KIconButtonMenu.ButtonInfo buttonInfo = this.buttons[i];
			if (e.TryConsume(buttonInfo.shortcutKey))
			{
				this.buttonObjects[i].GetComponent<KButton>().PlayPointerDownSound();
				this.buttonObjects[i].GetComponent<KButton>().SignalClick();
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

	protected void SelectToggle(KToggle selectedToggle)
	{
		if (global::UnityEngine.EventSystems.EventSystem.current == null || !global::UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			return;
		}
		if (this.currentlySelectedToggle == selectedToggle)
		{
			this.currentlySelectedToggle = null;
		}
		else
		{
			this.currentlySelectedToggle = selectedToggle;
		}
		foreach (GameObject gameObject in this.buttonObjects)
		{
			KToggle component = gameObject.GetComponent<KToggle>();
			if (component != null)
			{
				if (component == this.currentlySelectedToggle)
				{
					component.Select();
					component.isOn = true;
				}
				else
				{
					component.Deselect();
					component.isOn = false;
				}
			}
		}
	}

	public void ClearSelection()
	{
		foreach (GameObject gameObject in this.buttonObjects)
		{
			KToggle component = gameObject.GetComponent<KToggle>();
			if (component != null)
			{
				component.Deselect();
				component.isOn = false;
			}
			else
			{
				KBasicToggle component2 = gameObject.GetComponent<KBasicToggle>();
				if (component2 != null)
				{
					component2.isOn = false;
				}
			}
			ImageToggleState component3 = gameObject.GetComponent<ImageToggleState>();
			if (component3.GetIsActive())
			{
				component3.SetInactive();
			}
		}
		ToggleGroup component4 = base.GetComponent<ToggleGroup>();
		if (component4 != null)
		{
			component4.SetAllTogglesOff();
		}
		this.SelectToggle(null);
	}

	[SerializeField]
	protected bool followGameObject;

	[SerializeField]
	protected bool keepMenuOpen;

	[SerializeField]
	protected bool automaticNavigation = true;

	[SerializeField]
	private Transform buttonParent;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	protected Sprite[] icons;

	[SerializeField]
	private ToggleGroup externalToggleGroup;

	protected KToggle currentlySelectedToggle;

	[NonSerialized]
	public GameObject[] buttonObjects;

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public string tooltipHotKeyColor = "#ff2222ff";

	protected GameObject go;

	protected IList<KIconButtonMenu.ButtonInfo> buttons;

	public class ButtonInfo
	{
		public ButtonInfo(string iconName = "", string text = "", global::System.Action on_click = null, global::Action shortcutKey = global::Action.NumActions, Action<GameObject> on_refresh = null, Action<KIconButtonMenu.ButtonInfo> on_create = null, Texture texture = null, string tooltipText = "", bool is_interactable = true)
		{
			this.iconName = iconName;
			this.text = text;
			this.shortcutKey = shortcutKey;
			this.onClick = on_click;
			this.onRefresh = on_refresh;
			this.onCreate = on_create;
			this.texture = texture;
			this.tooltipText = tooltipText;
			this.isInteractable = is_interactable;
		}

		public string GetTooltipText()
		{
			string text = ((!(this.tooltipText == string.Empty)) ? this.tooltipText : this.text);
			if (this.shortcutKey != global::Action.NumActions)
			{
				text = text + " " + GameUtil.GetHotkeyString(this.shortcutKey);
			}
			return text;
		}

		public string iconName;

		public string text;

		public string tooltipText;

		public string[] multiText;

		public global::Action shortcutKey;

		public bool isInteractable;

		public Action<KIconButtonMenu.ButtonInfo> onCreate;

		public global::System.Action onClick;

		public Action<GameObject> onRefresh;

		public Func<string> onToolTip;

		public GameObject buttonGo;

		public object userData;

		public Texture texture;

		public delegate void Callback();
	}
}
