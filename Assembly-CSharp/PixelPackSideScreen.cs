using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelPackSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.swatch_object_by_color.Count == 0)
		{
			this.InitializeColorSwatch();
		}
		this.PopulateColorSelections();
		this.copyActiveToStandbyButton.onClick += this.CopyActiveToStandby;
		this.copyStandbyToActiveButton.onClick += this.CopyStandbyToActive;
		this.swapColorsButton.onClick += this.SwapColors;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<PixelPack>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetPixelPack = target.GetComponent<PixelPack>();
		this.PopulateColorSelections();
		this.HighlightUsedColors();
	}

	private void HighlightUsedColors()
	{
		if (this.swatch_object_by_color.Count == 0)
		{
			this.InitializeColorSwatch();
		}
		for (int i = 0; i < this.highlightedSwatchGameObjects.Count; i++)
		{
			this.highlightedSwatchGameObjects[i].GetComponent<HierarchyReferences>().GetReference("used").GetComponentInChildren<Image>()
				.gameObject.SetActive(false);
		}
		this.highlightedSwatchGameObjects.Clear();
		for (int j = 0; j < this.targetPixelPack.colorSettings.Count; j++)
		{
			this.swatch_object_by_color[this.targetPixelPack.colorSettings[j].activeColor].GetComponent<HierarchyReferences>().GetReference("used").gameObject.SetActive(true);
			this.swatch_object_by_color[this.targetPixelPack.colorSettings[j].standbyColor].GetComponent<HierarchyReferences>().GetReference("used").gameObject.SetActive(true);
			this.highlightedSwatchGameObjects.Add(this.swatch_object_by_color[this.targetPixelPack.colorSettings[j].activeColor]);
			this.highlightedSwatchGameObjects.Add(this.swatch_object_by_color[this.targetPixelPack.colorSettings[j].standbyColor]);
		}
	}

	private void PopulateColorSelections()
	{
		for (int i = 0; i < this.targetPixelPack.colorSettings.Count; i++)
		{
			int current_id = i;
			this.activeColors[i].GetComponent<Image>().color = this.targetPixelPack.colorSettings[i].activeColor;
			this.activeColors[i].GetComponent<KButton>().onClick += delegate
			{
				PixelPack.ColorPair colorPair = this.targetPixelPack.colorSettings[current_id];
				this.activeColors[current_id].GetComponent<Image>().color = this.paintingColor;
				colorPair.activeColor = this.paintingColor;
				this.targetPixelPack.colorSettings[current_id] = colorPair;
				this.targetPixelPack.UpdateColors();
				this.HighlightUsedColors();
			};
			this.standbyColors[i].GetComponent<Image>().color = this.targetPixelPack.colorSettings[i].standbyColor;
			this.standbyColors[i].GetComponent<KButton>().onClick += delegate
			{
				PixelPack.ColorPair colorPair2 = this.targetPixelPack.colorSettings[current_id];
				this.standbyColors[current_id].GetComponent<Image>().color = this.paintingColor;
				colorPair2.standbyColor = this.paintingColor;
				this.targetPixelPack.colorSettings[current_id] = colorPair2;
				this.targetPixelPack.UpdateColors();
				this.HighlightUsedColors();
			};
		}
	}

	private void InitializeColorSwatch()
	{
		bool flag = false;
		for (int i = 0; i < this.colorSwatch.Count; i++)
		{
			GameObject swatch = Util.KInstantiateUI(this.swatchEntry, this.colorSwatchContainer, true);
			Image component = swatch.GetComponent<Image>();
			component.color = this.colorSwatch[i];
			KButton component2 = swatch.GetComponent<KButton>();
			Color color = this.colorSwatch[i];
			if (component.color == Color.black)
			{
				swatch.GetComponent<HierarchyReferences>().GetReference("selected").GetComponentInChildren<Image>()
					.color = Color.white;
			}
			if (!flag)
			{
				this.SelectColor(color, swatch);
				flag = true;
			}
			component2.onClick += delegate
			{
				this.SelectColor(color, swatch);
			};
			this.swatch_object_by_color[color] = swatch;
		}
	}

	private void SelectColor(Color color, GameObject swatchEntry)
	{
		this.paintingColor = color;
		swatchEntry.GetComponent<HierarchyReferences>().GetReference("selected").gameObject.SetActive(true);
		if (this.selectedSwatchEntry != null && this.selectedSwatchEntry != swatchEntry)
		{
			this.selectedSwatchEntry.GetComponent<HierarchyReferences>().GetReference("selected").gameObject.SetActive(false);
		}
		this.selectedSwatchEntry = swatchEntry;
	}

	private void CopyActiveToStandby()
	{
		for (int i = 0; i < this.targetPixelPack.colorSettings.Count; i++)
		{
			PixelPack.ColorPair colorPair = this.targetPixelPack.colorSettings[i];
			colorPair.standbyColor = colorPair.activeColor;
			this.targetPixelPack.colorSettings[i] = colorPair;
			this.standbyColors[i].GetComponent<Image>().color = colorPair.activeColor;
		}
		this.HighlightUsedColors();
		this.targetPixelPack.UpdateColors();
	}

	private void CopyStandbyToActive()
	{
		for (int i = 0; i < this.targetPixelPack.colorSettings.Count; i++)
		{
			PixelPack.ColorPair colorPair = this.targetPixelPack.colorSettings[i];
			colorPair.activeColor = colorPair.standbyColor;
			this.targetPixelPack.colorSettings[i] = colorPair;
			this.activeColors[i].GetComponent<Image>().color = colorPair.standbyColor;
		}
		this.HighlightUsedColors();
		this.targetPixelPack.UpdateColors();
	}

	private void SwapColors()
	{
		for (int i = 0; i < this.targetPixelPack.colorSettings.Count; i++)
		{
			PixelPack.ColorPair colorPair = default(PixelPack.ColorPair);
			colorPair.activeColor = this.targetPixelPack.colorSettings[i].standbyColor;
			colorPair.standbyColor = this.targetPixelPack.colorSettings[i].activeColor;
			this.targetPixelPack.colorSettings[i] = colorPair;
			this.activeColors[i].GetComponent<Image>().color = colorPair.activeColor;
			this.standbyColors[i].GetComponent<Image>().color = colorPair.standbyColor;
		}
		this.HighlightUsedColors();
		this.targetPixelPack.UpdateColors();
	}

	public PixelPack targetPixelPack;

	public KButton copyActiveToStandbyButton;

	public KButton copyStandbyToActiveButton;

	public KButton swapColorsButton;

	public GameObject colorSwatchContainer;

	public GameObject swatchEntry;

	public GameObject activeColorsContainer;

	public GameObject standbyColorsContainer;

	public List<GameObject> activeColors = new List<GameObject>();

	public List<GameObject> standbyColors = new List<GameObject>();

	public Color paintingColor;

	public GameObject selectedSwatchEntry;

	private Dictionary<Color, GameObject> swatch_object_by_color = new Dictionary<Color, GameObject>();

	private List<GameObject> highlightedSwatchGameObjects = new List<GameObject>();

	private List<Color> colorSwatch = new List<Color>
	{
		new Color(0.4862745f, 0.4862745f, 0.4862745f),
		new Color(0f, 0f, 0.9882353f),
		new Color(0f, 0f, 0.7372549f),
		new Color(0.26666668f, 0.15686275f, 0.7372549f),
		new Color(0.5803922f, 0f, 0.5176471f),
		new Color(0.65882355f, 0f, 0.1254902f),
		new Color(0.65882355f, 0.0627451f, 0f),
		new Color(0.53333336f, 0.078431375f, 0f),
		new Color(0.3137255f, 0.1882353f, 0f),
		new Color(0f, 0.47058824f, 0f),
		new Color(0f, 0.40784314f, 0f),
		new Color(0f, 0.34509805f, 0f),
		new Color(0f, 0.2509804f, 0.34509805f),
		new Color(0f, 0f, 0f),
		new Color(0.7372549f, 0.7372549f, 0.7372549f),
		new Color(0f, 0.47058824f, 0.972549f),
		new Color(0f, 0.34509805f, 0.972549f),
		new Color(0.40784314f, 0.26666668f, 0.9882353f),
		new Color(0.84705883f, 0f, 0.8f),
		new Color(0.89411765f, 0f, 0.34509805f),
		new Color(0.972549f, 0.21960784f, 0f),
		new Color(0.89411765f, 0.36078432f, 0.0627451f),
		new Color(0.6745098f, 0.4862745f, 0f),
		new Color(0f, 0.72156864f, 0f),
		new Color(0f, 0.65882355f, 0f),
		new Color(0f, 0.65882355f, 0.26666668f),
		new Color(0f, 0.53333336f, 0.53333336f),
		new Color(0f, 0f, 0f),
		new Color(0.972549f, 0.972549f, 0.972549f),
		new Color(0.23529412f, 0.7372549f, 0.9882353f),
		new Color(0.40784314f, 0.53333336f, 0.9882353f),
		new Color(0.59607846f, 0.47058824f, 0.972549f),
		new Color(0.972549f, 0.47058824f, 0.972549f),
		new Color(0.972549f, 0.34509805f, 0.59607846f),
		new Color(0.972549f, 0.47058824f, 0.34509805f),
		new Color(0.9882353f, 0.627451f, 0.26666668f),
		new Color(0.972549f, 0.72156864f, 0f),
		new Color(0.72156864f, 0.972549f, 0.09411765f),
		new Color(0.34509805f, 0.84705883f, 0.32941177f),
		new Color(0.34509805f, 0.972549f, 0.59607846f),
		new Color(0f, 0.9098039f, 0.84705883f),
		new Color(0.47058824f, 0.47058824f, 0.47058824f),
		new Color(0.9882353f, 0.9882353f, 0.9882353f),
		new Color(0.6431373f, 0.89411765f, 0.9882353f),
		new Color(0.72156864f, 0.72156864f, 0.972549f),
		new Color(0.84705883f, 0.72156864f, 0.972549f),
		new Color(0.972549f, 0.72156864f, 0.972549f),
		new Color(0.972549f, 0.72156864f, 0.7529412f),
		new Color(0.9411765f, 0.8156863f, 0.6901961f),
		new Color(0.9882353f, 0.8784314f, 0.65882355f),
		new Color(0.972549f, 0.84705883f, 0.47058824f),
		new Color(0.84705883f, 0.972549f, 0.47058824f),
		new Color(0.72156864f, 0.972549f, 0.72156864f),
		new Color(0.72156864f, 0.972549f, 0.84705883f),
		new Color(0f, 0.9882353f, 0.9882353f),
		new Color(0.84705883f, 0.84705883f, 0.84705883f)
	};
}
