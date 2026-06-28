using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverTextScreen : KScreen
{
	public bool LoadPreConfiguredToolFields(HoverTextConfiguration config)
	{
		if (this.currentConfiguration != null)
		{
			this.SaveCurrentFieldsAs(this.currentConfiguration);
			this.ClearLabels();
			this.currentConfiguration.SetNotConfigured();
			this.ToggleIncubating(true);
		}
		if (this.CachedToolFields != null && this.CachedToolFields.ContainsKey(config))
		{
			this.ClearLabels();
			this.MultiLabelDisplays = this.CachedToolFields[config].MultiLabelDisplays;
			this.ShadowBars = this.CachedToolFields[config].ShadowBars;
			this.currentConfiguration = config;
			this.ActivateLabels();
			return true;
		}
		return false;
	}

	public void SaveCurrentFieldsAs(HoverTextConfiguration config)
	{
		if (config == null)
		{
			return;
		}
		if (this.CachedToolFields.ContainsKey(config))
		{
			this.CachedToolFields[config] = this.CurrentFields();
		}
		else
		{
			this.CachedToolFields.Add(config, this.CurrentFields());
		}
	}

	public ToolHoverFields CurrentFields()
	{
		return new ToolHoverFields(this.MultiLabelDisplays, this.ShadowBars);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		HoverTextScreen.Instance = this;
		this.ToggleIncubating(true);
	}

	private void Update()
	{
		if (!this.incubating && this.transform.rectTransform().localScale != Vector3.one)
		{
			this.transform.rectTransform().localScale = Vector3.one;
		}
		this.ToggleIncubating(false);
		if (this.Container.activeSelf)
		{
			this.PositionShadowBars();
		}
		if (PlayerController.Instance.IsUsingDefaultTool())
		{
			KSelectable hover = SelectTool.Instance.hover;
			if (hover != null && hover == this.previousHover)
			{
				this.hoverDelay -= Time.unscaledDeltaTime;
			}
			else
			{
				this.ResetHoverDelay();
			}
			this.previousHover = hover;
			bool flag = hover != null;
			this.IsVisible = flag;
			this.Container.SetActive(flag);
		}
		else
		{
			bool flag2 = PlayerController.Instance.ActiveTool.ShowHoverUI();
			this.IsVisible = flag2;
			this.Container.SetActive(flag2);
		}
		RectTransform component = this.Container.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector3(Input.mousePosition.x + this.offset.x, Input.mousePosition.y + this.offset.y, 0f);
		float canvasScale = this.transform.parent.GetComponent<KCanvasScaler>().GetCanvasScale();
		component.anchoredPosition = new Vector2(component.anchoredPosition.x / canvasScale, component.anchoredPosition.y / canvasScale);
	}

	public Sprite GetSprite(string byName)
	{
		foreach (Sprite sprite in this.HoverIcons)
		{
			if (sprite.name == byName)
			{
				return sprite;
			}
		}
		Debug.LogWarning("No icon named " + byName + " was found on HoverTextScreen.prefab");
		return null;
	}

	public void ResetHoverDelay()
	{
		this.hoverDelay = 0f;
	}

	public void SetMessage(string Message)
	{
		this.ClearLabels();
		this.AddText(Message, null, true);
		this.Container.SetActive(true);
		RectTransform component = this.Container.GetComponent<RectTransform>();
		component.anchoredPosition = new Vector3(Input.mousePosition.x + this.offset.x, Input.mousePosition.y + this.offset.y, 0f);
	}

	public void ClearLabels()
	{
		for (int i = this.MultiLabelDisplays.Count - 1; i >= 0; i--)
		{
			this.MultiLabelDisplays[i].SetActive(false);
		}
		for (int j = this.ShadowBars.Count - 1; j >= 0; j--)
		{
			this.ShadowBars[j].gameObject.SetActive(false);
		}
		this.MultiLabelDisplays.Clear();
		this.ShadowBars.Clear();
		this.ToggleIncubating(true);
	}

	public void ClearConfigurationLabels(HoverTextConfiguration config)
	{
		Debug.Log("Clearing configuration for: " + config.ActionName);
		if (this.CachedToolFields.ContainsKey(config))
		{
			string text = string.Empty;
			for (int i = this.CachedToolFields[config].MultiLabelDisplays.Count - 1; i >= 0; i--)
			{
				text = text + "\n" + this.CachedToolFields[config].MultiLabelDisplays[i].name;
				this.CachedToolFields[config].MultiLabelDisplays[i].SetActive(false);
			}
			for (int j = this.CachedToolFields[config].ShadowBars.Count - 1; j >= 0; j--)
			{
				text = text + "\n" + this.CachedToolFields[config].ShadowBars[j].gameObject.name;
				this.CachedToolFields[config].ShadowBars[j].gameObject.SetActive(false);
			}
			Debug.Log(text);
		}
	}

	public void ActivateLabels()
	{
		for (int i = this.MultiLabelDisplays.Count - 1; i >= 0; i--)
		{
			this.MultiLabelDisplays[i].SetActive(true);
		}
		for (int j = this.ShadowBars.Count - 1; j >= 0; j--)
		{
			this.ShadowBars[j].gameObject.SetActive(true);
		}
		this.ToggleIncubating(true);
	}

	public void ToggleIncubating(bool incubating)
	{
		if (incubating)
		{
			incubating = true;
			this.transform.rectTransform().localScale = Vector3.zero;
		}
		else
		{
			incubating = false;
		}
	}

	public GameObject NewLine(string optionalDebugName = "NewLine", int height = 24)
	{
		GameObject gameObject = Util.KInstantiateUI(this.LinePrefab, this.Container, true);
		gameObject.GetComponent<LayoutElement>().minHeight = (float)height;
		gameObject.GetComponent<LayoutElement>().preferredHeight = -1f;
		gameObject.name = optionalDebugName;
		this.MultiLabelDisplays.Add(gameObject);
		return gameObject;
	}

	public GameObject GetLine(string debugName)
	{
		return this.MultiLabelDisplays.Find((GameObject li) => li.name == debugName);
	}

	public ShadowBar StartShadowBar(float leftIndet = 0f, float rightIndent = 0f, bool selectedBorder = false)
	{
		ShadowBar shadowBar = new ShadowBar();
		shadowBar.leftIndent = leftIndet;
		shadowBar.rightIndent = rightIndent;
		shadowBar.gameObject = Util.KInstantiateUI(this.ShadowBarPrefab, this.Container, true);
		shadowBar.selectionBorder = shadowBar.gameObject.transform.FindChild("SelectBorder").gameObject;
		shadowBar.startLineIndex = this.MultiLabelDisplays.Count;
		shadowBar.gameObject.transform.FindChild("SelectBorder").gameObject.SetActive(selectedBorder);
		this.ShadowBars.Add(shadowBar);
		return shadowBar;
	}

	public void EndShadowBar()
	{
		this.ShadowBars[this.ShadowBars.Count - 1].endLineIndex = this.MultiLabelDisplays.Count;
		this.ShadowBars[this.ShadowBars.Count - 1].gameObject.transform.SetAsFirstSibling();
	}

	private void PositionShadowBars()
	{
		if (this.ShadowBars == null || this.ShadowBars.Count == 0)
		{
			return;
		}
		foreach (ShadowBar shadowBar in this.ShadowBars)
		{
			float num = 0f;
			float num2 = 0f;
			shadowBar.gameObject.rectTransform().anchoredPosition = this.MultiLabelDisplays[shadowBar.startLineIndex].rectTransform().anchoredPosition + new Vector2(-shadowBar.SizeBleed.x, shadowBar.SizeBleed.y) + Vector2.right * shadowBar.leftIndent;
			int num3 = 0;
			VerticalLayoutGroup component = this.Container.GetComponent<VerticalLayoutGroup>();
			for (int i = shadowBar.startLineIndex; i < shadowBar.endLineIndex; i++)
			{
				if (this.MultiLabelDisplays[i].gameObject.activeSelf)
				{
					num3++;
					num2 += this.MultiLabelDisplays[i].rectTransform().sizeDelta.y;
					if (component)
					{
						num2 += component.spacing;
					}
					if (this.MultiLabelDisplays[i].rectTransform().sizeDelta.x > num)
					{
						num = this.MultiLabelDisplays[i].rectTransform().sizeDelta.x;
					}
				}
			}
			if (num3 == 0 && shadowBar.gameObject.activeSelf)
			{
				shadowBar.gameObject.SetActive(false);
			}
			else if (!shadowBar.gameObject.activeSelf && num3 > 0)
			{
				shadowBar.gameObject.SetActive(true);
			}
			num -= shadowBar.leftIndent;
			if (num > 0f && num2 > 0f)
			{
				shadowBar.gameObject.rectTransform().sizeDelta = new Vector2(num, num2) + shadowBar.SizeBleed * 2f;
			}
			else
			{
				shadowBar.gameObject.rectTransform().sizeDelta = Vector2.zero;
			}
		}
	}

	public void AddIndent(float width = 36f, float height = 18f)
	{
		this.AddIcon(null, width, height, Color.white);
	}

	public LocText AddText(string Text, TextStyleSetting styleSetting = null, bool AllCaps = true)
	{
		if (this.MultiLabelDisplays.Count == 0)
		{
			this.NewLine("NewLine", 24);
		}
		LayoutElement component = this.MultiLabelDisplays[this.MultiLabelDisplays.Count - 1].GetComponent<LayoutElement>();
		GameObject gameObject = Util.KInstantiateUI(this.TextPrefab, component.gameObject, true);
		LayoutElement component2 = gameObject.GetComponent<LayoutElement>();
		component2.minHeight = component.minHeight;
		if (AllCaps)
		{
			Text = Text.ToUpper();
		}
		gameObject.GetComponent<LocText>().text = Text;
		if (styleSetting != null)
		{
			gameObject.GetComponent<LocText>().textStyleSetting = styleSetting;
			gameObject.GetComponent<SetTextStyleSetting>().SetStyle(styleSetting);
		}
		return gameObject.GetComponent<LocText>();
	}

	public Image AddIcon(Sprite icon, float forceWidth, Color color)
	{
		Image image = this.AddIcon(icon, color, forceWidth);
		image.rectTransform().sizeDelta = new Vector2(forceWidth, image.rectTransform().sizeDelta.y);
		LayoutElement component = image.GetComponent<LayoutElement>();
		component.minWidth = forceWidth;
		component.preferredWidth = forceWidth;
		component.minHeight = forceWidth;
		component.preferredHeight = forceWidth;
		return image;
	}

	public Image AddIcon(Sprite icon, float forceWidth, float forceHeight, Color color)
	{
		Image image = this.AddIcon(icon, color, forceWidth);
		image.rectTransform().sizeDelta = new Vector2(forceWidth, image.rectTransform().sizeDelta.y);
		LayoutElement component = image.GetComponent<LayoutElement>();
		component.minWidth = forceWidth;
		component.preferredWidth = forceWidth;
		component.minHeight = forceHeight;
		component.preferredHeight = forceHeight;
		return image;
	}

	public Image AddIcon(Sprite icon, float minWidth = 18f)
	{
		return this.AddIcon(icon, Color.white, minWidth);
	}

	public Image AddIcon(Sprite icon, Color color, float minWidth = 18f)
	{
		if (this.MultiLabelDisplays.Count == 0)
		{
			this.NewLine("NewLine", 24);
		}
		LayoutElement component = this.MultiLabelDisplays[this.MultiLabelDisplays.Count - 1].GetComponent<LayoutElement>();
		GameObject gameObject = Util.KInstantiateUI(this.IconPrefab, component.gameObject, true);
		Image component2 = gameObject.GetComponent<Image>();
		component2.sprite = icon;
		if (icon == null)
		{
			component2.color = Color.clear;
			gameObject.name = "EmptyIconIndent";
		}
		else
		{
			gameObject.name = icon.name;
			component2.color = color;
		}
		gameObject.gameObject.GetComponent<LayoutElement>().minWidth = minWidth;
		gameObject.gameObject.GetComponent<LayoutElement>().minHeight = Mathf.Clamp(component.minHeight - 4f, 0f, component.minHeight);
		return gameObject.GetComponent<Image>();
	}

	public Image AddUnBoundedIcon(Sprite icon, Color color, float imageWidth, float imageHeight, float layoutElementWidth = 24f, float layoutElementHeight = 18f)
	{
		if (this.MultiLabelDisplays.Count == 0)
		{
			this.NewLine("NewLine", 24);
		}
		LayoutElement component = this.MultiLabelDisplays[this.MultiLabelDisplays.Count - 1].GetComponent<LayoutElement>();
		GameObject gameObject = Util.KInstantiateUI(this.UnboundedIconPrefab, component.gameObject, true);
		LayoutElement component2 = gameObject.GetComponent<LayoutElement>();
		RectTransform component3 = gameObject.transform.FindChild("Image").GetComponent<RectTransform>();
		LayoutElement layoutElement = component2;
		component2.preferredWidth = layoutElementWidth;
		layoutElement.minWidth = layoutElementWidth;
		LayoutElement layoutElement2 = component2;
		component2.preferredHeight = layoutElementHeight;
		layoutElement2.minHeight = layoutElementHeight;
		Image component4 = gameObject.transform.FindChild("Image").GetComponent<Image>();
		component4.sprite = icon;
		gameObject.name = icon.name;
		component4.color = color;
		component3.sizeDelta = new Vector2(imageWidth, imageHeight);
		LayoutElement component5 = component3.GetComponent<LayoutElement>();
		LayoutElement layoutElement3 = component5;
		component5.preferredWidth = imageWidth;
		layoutElement3.minWidth = imageWidth;
		LayoutElement layoutElement4 = component5;
		component5.preferredHeight = imageHeight;
		layoutElement4.minHeight = imageHeight;
		return gameObject.transform.FindChild("Image").GetComponent<Image>();
	}

	[SerializeField]
	public Vector2 offset;

	[SerializeField]
	private GameObject Container;

	[SerializeField]
	private GameObject LinePrefab;

	[SerializeField]
	private GameObject TextPrefab;

	[SerializeField]
	private GameObject IconPrefab;

	[SerializeField]
	private GameObject UnboundedIconPrefab;

	[SerializeField]
	private GameObject ShadowBarPrefab;

	private bool incubating;

	public Sprite[] HoverIcons;

	private List<GameObject> MultiLabelDisplays = new List<GameObject>();

	private List<ShadowBar> ShadowBars = new List<ShadowBar>();

	public HoverTextConfiguration currentConfiguration;

	public Dictionary<HoverTextConfiguration, ToolHoverFields> CachedToolFields = new Dictionary<HoverTextConfiguration, ToolHoverFields>();

	private KSelectable previousHover;

	private float hoverDelay;

	public static HoverTextScreen Instance;

	public bool IsVisible;

	public bool JustBecameVisible;

	public struct HoverTextUpdateTimer
	{
		public bool tick()
		{
			this.timeElapsed += Time.unscaledDeltaTime;
			if (this.timeElapsed >= this.tickInterval)
			{
				this.timeElapsed = 0f;
				return true;
			}
			return false;
		}

		public void Prime()
		{
			this.timeElapsed = this.tickInterval;
		}

		public float tickInterval;

		private float timeElapsed;
	}
}
