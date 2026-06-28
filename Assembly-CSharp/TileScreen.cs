using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileScreen : KScreen
{
	private bool SetSliderColour(float temperature, float transition_temperature)
	{
		if (Mathf.Abs(temperature - transition_temperature) < 5f)
		{
			this.temperatureSliderText.color = this.temperatureTransitionColour;
			this.temperatureSliderIcon.color = this.temperatureTransitionColour;
			return true;
		}
		this.temperatureSliderText.color = this.temperatureDefaultColour;
		this.temperatureSliderIcon.color = this.temperatureDefaultColour;
		return false;
	}

	private void DisplayTileInfo()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = -Camera.main.transform.position.z - Grid.CellSizeInMeters;
		Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
		int num = Grid.PosToCell(vector);
		if (Grid.IsValidCell(num) && (float)Grid.Visible[num] + PropertyTextures.FogOfWarScale != 0f)
		{
			Element element = Grid.Element[num];
			this.nameLabel.text = element.name;
			float num2 = Grid.Cell[num].mass;
			string text = "kg";
			if (num2 < 5f)
			{
				num2 *= 1000f;
				text = "g";
			}
			if (num2 < 5f)
			{
				num2 *= 1000f;
				text = "mg";
			}
			if (num2 < 5f)
			{
				num2 *= 1000f;
				text = "mcg";
				num2 = Mathf.Floor(num2);
			}
			this.massAmtLabel.text = string.Format("{0:0.0} {1}", num2, text);
			this.massTitleLabel.text = "mass";
			float num3 = Grid.Temperature[num];
			if (element.IsSolid)
			{
				this.solidIcon.gameObject.transform.parent.gameObject.SetActive(true);
				this.gasIcon.gameObject.transform.parent.gameObject.SetActive(false);
				this.massIcon.sprite = this.solidIcon.sprite;
				this.solidText.text = ((int)element.highTemp).ToString();
				this.gasText.text = string.Empty;
				this.liquidIcon.rectTransform.SetParent(this.solidIcon.transform.parent, true);
				this.liquidIcon.rectTransform.localPosition = new Vector3(0f, 64f);
				this.SetSliderColour(num3, element.highTemp);
				this.temperatureSlider.SetMinMaxValue(element.highTemp, Mathf.Min(element.highTemp + 100f, 4000f), Mathf.Max(element.highTemp - 100f, 0f), Mathf.Min(element.highTemp + 100f, 4000f));
			}
			else if (element.IsLiquid)
			{
				this.solidIcon.gameObject.transform.parent.gameObject.SetActive(true);
				this.gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				this.massIcon.sprite = this.liquidIcon.sprite;
				this.solidText.text = ((int)element.lowTemp).ToString();
				this.gasText.text = ((int)element.highTemp).ToString();
				this.liquidIcon.rectTransform.SetParent(this.temperatureSlider.transform.parent, true);
				this.liquidIcon.rectTransform.localPosition = new Vector3(-80f, 0f);
				if (!this.SetSliderColour(num3, element.lowTemp))
				{
					this.SetSliderColour(num3, element.highTemp);
				}
				this.temperatureSlider.SetMinMaxValue(element.lowTemp, element.highTemp, Mathf.Max(element.lowTemp - 100f, 0f), Mathf.Min(element.highTemp + 100f, 5200f));
			}
			else if (element.IsGas)
			{
				this.solidText.text = string.Empty;
				this.gasText.text = ((int)element.lowTemp).ToString();
				this.solidIcon.gameObject.transform.parent.gameObject.SetActive(false);
				this.gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				this.massIcon.sprite = this.gasIcon.sprite;
				this.SetSliderColour(num3, element.lowTemp);
				this.liquidIcon.rectTransform.SetParent(this.gasIcon.transform.parent, true);
				this.liquidIcon.rectTransform.localPosition = new Vector3(0f, -64f);
				this.temperatureSlider.SetMinMaxValue(0f, Mathf.Max(element.lowTemp - 100f, 0f), 0f, element.lowTemp + 100f);
			}
			this.temperatureSlider.SetExtraValue(num3);
			this.temperatureSliderText.text = GameUtil.GetFormattedTemperature((float)((int)num3), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
			Dictionary<int, float> info = FallingWater.instance.GetInfo(num);
			if (info.Count > 0)
			{
				List<Element> elements = ElementLoader.elements;
				foreach (KeyValuePair<int, float> keyValuePair in info)
				{
					Element element2 = elements[keyValuePair.Key];
					Text text2 = this.nameLabel;
					text2.text = text2.text + "\n" + element2.name + string.Format(" {0:0.00} kg", keyValuePair.Value);
				}
			}
		}
		else
		{
			this.nameLabel.text = "Unknown";
		}
	}

	private void DisplayConduitFlowInfo()
	{
		SimViewMode mode = OverlayScreen.Instance.GetMode();
		UtilityNetworkManager<FlowUtilityNetwork, Vent> utilityNetworkManager = ((mode != SimViewMode.GasVentMap) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem);
		ConduitFlow conduitFlow = ((mode != SimViewMode.GasVentMap) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow);
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = -Camera.main.transform.position.z - Grid.CellSizeInMeters;
		Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
		int num = Grid.PosToCell(vector);
		if (Grid.IsValidCell(num) && utilityNetworkManager.GetConnections(num, true) != (UtilityConnections)0)
		{
			ConduitFlow.ConduitContents contents = conduitFlow.GetContents(num);
			SimHashes element = contents.element;
			Element element2 = ElementLoader.FindElementByHash(element);
			float num2 = contents.mass;
			float temperature = contents.temperature;
			this.nameLabel.text = element2.name;
			string text = "kg";
			if (num2 < 5f)
			{
				num2 *= 1000f;
				text = "g";
			}
			this.massAmtLabel.text = string.Format("{0:0.0} {1}", num2, text);
			this.massTitleLabel.text = "mass";
			if (element2.IsLiquid)
			{
				this.solidIcon.gameObject.transform.parent.gameObject.SetActive(true);
				this.gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				this.massIcon.sprite = this.liquidIcon.sprite;
				this.solidText.text = ((int)element2.lowTemp).ToString();
				this.gasText.text = ((int)element2.highTemp).ToString();
				this.liquidIcon.rectTransform.SetParent(this.temperatureSlider.transform.parent, true);
				this.liquidIcon.rectTransform.localPosition = new Vector3(-80f, 0f);
				if (!this.SetSliderColour(temperature, element2.lowTemp))
				{
					this.SetSliderColour(temperature, element2.highTemp);
				}
				this.temperatureSlider.SetMinMaxValue(element2.lowTemp, element2.highTemp, Mathf.Max(element2.lowTemp - 100f, 0f), Mathf.Min(element2.highTemp + 100f, 5200f));
			}
			else if (element2.IsGas)
			{
				this.solidText.text = string.Empty;
				this.gasText.text = ((int)element2.lowTemp).ToString();
				this.solidIcon.gameObject.transform.parent.gameObject.SetActive(false);
				this.gasIcon.gameObject.transform.parent.gameObject.SetActive(true);
				this.massIcon.sprite = this.gasIcon.sprite;
				this.SetSliderColour(temperature, element2.lowTemp);
				this.liquidIcon.rectTransform.SetParent(this.gasIcon.transform.parent, true);
				this.liquidIcon.rectTransform.localPosition = new Vector3(0f, -64f);
				this.temperatureSlider.SetMinMaxValue(0f, Mathf.Max(element2.lowTemp - 100f, 0f), 0f, element2.lowTemp + 100f);
			}
			this.temperatureSlider.SetExtraValue(temperature);
			this.temperatureSliderText.text = GameUtil.GetFormattedTemperature((float)((int)temperature), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
		}
		else
		{
			this.nameLabel.text = "No Conduit";
			this.symbolLabel.text = string.Empty;
			this.massAmtLabel.text = string.Empty;
			this.massTitleLabel.text = string.Empty;
		}
	}

	private void Update()
	{
		base.transform.SetPosition(Input.mousePosition);
		SimViewMode mode = OverlayScreen.Instance.GetMode();
		if (mode != SimViewMode.GasVentMap && mode != SimViewMode.LiquidVentMap)
		{
			this.DisplayTileInfo();
		}
		else
		{
			this.DisplayConduitFlowInfo();
		}
	}

	public Text nameLabel;

	public Text symbolLabel;

	public Text massTitleLabel;

	public Text massAmtLabel;

	public Image massIcon;

	public MinMaxSlider temperatureSlider;

	public Text temperatureSliderText;

	public Image temperatureSliderIcon;

	public Image solidIcon;

	public Image liquidIcon;

	public Image gasIcon;

	public Text solidText;

	public Text gasText;

	[SerializeField]
	private Color temperatureDefaultColour;

	[SerializeField]
	private Color temperatureTransitionColour;
}
