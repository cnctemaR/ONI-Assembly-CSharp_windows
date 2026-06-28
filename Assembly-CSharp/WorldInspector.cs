using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class WorldInspector : MonoBehaviour
{
	private void Update()
	{
		this.Refresh();
	}

	private void Refresh()
	{
		if (SelectTool.Instance.selected == null)
		{
			return;
		}
		CellSelectionObject component = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
		if (component != null)
		{
			this.UpdateAsSimCell(component);
		}
		ElementChunk component2 = SelectTool.Instance.selected.GetComponent<ElementChunk>();
		if (component2 != null)
		{
			this.UpdateAsElementChunk(component2);
		}
		Edible component3 = SelectTool.Instance.selected.GetComponent<Edible>();
		if (component3 != null)
		{
			this.UpdateAsEdible(component3);
		}
	}

	private void UpdateAsSimCell(CellSelectionObject cellObject)
	{
		string[] array = WorldInspector.MassStrings(cellObject.mouseCell);
		this.PropertyLeftText.text = array[0] + array[1] + " " + array[2];
		this.PropertyIcon_Left.sprite = this.propertySprites.Mass;
		this.PropertyRightText.text = cellObject.tags.ProperName();
		this.PropertyIcon_Right.sprite = this.propertySprites.Resource;
		this.TemperatureTextDisplay.text = GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(cellObject.temperature), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
		this.Tooltip_CurrentTemperature.toolTip = "Current Temperature: " + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(cellObject.temperature), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
		int state = this.GetState(cellObject);
		this.SetStateColorScheme(state);
		this.Tooltip_CurrentTemperature.toolTip = this.SetCurrentTemperatureTooltip(cellObject.element, state);
		if ((byte)(cellObject.state & Element.State.TemperatureInsulated) == 16)
		{
			this.TemperatureNotch.SetActive(false);
		}
		else
		{
			this.TemperatureNotch.SetActive(true);
		}
		this.TemperatureNotch.rectTransform().anchoredPosition = new Vector2(this.GetTemperaturePosition(cellObject), this.TemperatureNotch.rectTransform().anchoredPosition.y);
	}

	private void UpdateAsElementChunk(ElementChunk _chunkObject)
	{
		PrimaryElement component = _chunkObject.GetComponent<PrimaryElement>();
		string text = string.Empty;
		this.PropertyLeftText.text = string.Format("{0:0.00}", component.Mass) + " kg";
		this.PropertyIcon_Left.sprite = this.propertySprites.Mass;
		this.PropertyRightText.text = ElementLoader.FindElementByHash(component.ElementID).GetMaterialCategoryTag().ProperName();
		this.PropertyIcon_Right.sprite = this.propertySprites.Resource;
		this.TemperatureTextDisplay.text = GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(component.Temperature), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
		text = "Current Temperature: " + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(component.Temperature), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
		this.SetStateColorScheme(0);
		text += this.SetCurrentTemperatureTooltip(ElementLoader.FindElementByHash(component.ElementID), 0);
		text = text + "\nMelts at: <color=yellow>" + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(ElementLoader.FindElementByHash(component.ElementID).highTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute) + "</color>";
		this.Tooltip_CurrentTemperature.toolTip = text;
		this.TemperatureNotch.SetActive(true);
		this.TemperatureNotch.rectTransform().anchoredPosition = new Vector2(this.GetTemperaturePosition(component), this.TemperatureNotch.rectTransform().anchoredPosition.y);
	}

	private void UpdateAsEdible(Edible edibleObject)
	{
		string text = string.Empty;
		this.PropertyLeftText.text = edibleObject.rations.ToString() + " Rations";
		this.PropertyIcon_Left.sprite = this.propertySprites.Rations;
		this.PropertyRightText.text = edibleObject.GetQuality().ToString();
		this.PropertyIcon_Right.sprite = this.propertySprites.Quality;
		float num = Grid.Temperature[Grid.PosToCell(edibleObject)];
		this.TemperatureTextDisplay.text = GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(num), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
		text = "Current Temperature: " + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(num), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
		this.SetStateColorScheme(0);
		text = text + "\nRots at temperatures above: <color=yellow>" + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(edibleObject.FoodInfo.RotTemperature), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute) + "</color>";
		this.Tooltip_CurrentTemperature.toolTip = text;
		this.TemperatureNotch.SetActive(true);
		this.TemperatureNotch.rectTransform().anchoredPosition = new Vector2(this.GetTemperaturePosition(edibleObject), this.TemperatureNotch.rectTransform().anchoredPosition.y);
	}

	private int GetState(CellSelectionObject cellObject)
	{
		int num = 0;
		if (cellObject.element.IsGas)
		{
			num = 2;
		}
		else if (cellObject.element.IsLiquid)
		{
			num = 1;
		}
		else if (cellObject.element.IsSolid)
		{
			num = 0;
		}
		return num;
	}

	private void SetStateColorScheme(int state)
	{
		switch (state)
		{
		case 0:
			this.TemperatureBarImage.sprite = this.SolidState.TemperatureBarBG;
			this.TemperatureNotchSymbol.sprite = this.SolidState.StateIcon;
			this.TemperatureNotchBG.color = this.SolidState.StateColor;
			this.TemperatureTextDisplay.color = this.SolidState.StateColor;
			this.TransitionStateIcon_Low.sprite = this.SolidState.StateIcon;
			this.TransitionStateIcon_Low.color = this.SolidState.StateColor;
			this.TransitionStateIcon_High.sprite = this.LiquidState.StateIcon;
			this.TransitionStateIcon_High.color = this.LiquidState.StateColor;
			break;
		case 1:
			this.TemperatureBarImage.sprite = this.LiquidState.TemperatureBarBG;
			this.TemperatureNotchSymbol.sprite = this.LiquidState.StateIcon;
			this.TemperatureNotchBG.color = this.LiquidState.StateColor;
			this.TemperatureTextDisplay.color = this.LiquidState.StateColor;
			this.TransitionStateIcon_Low.sprite = this.SolidState.StateIcon;
			this.TransitionStateIcon_Low.color = this.SolidState.StateColor;
			this.TransitionStateIcon_High.sprite = this.GasState.StateIcon;
			this.TransitionStateIcon_High.color = this.GasState.StateColor;
			break;
		case 2:
			this.TemperatureBarImage.sprite = this.GasState.TemperatureBarBG;
			this.TemperatureNotchSymbol.sprite = this.GasState.StateIcon;
			this.TemperatureNotchBG.color = this.GasState.StateColor;
			this.TemperatureTextDisplay.color = this.GasState.StateColor;
			this.TransitionStateIcon_Low.sprite = this.LiquidState.StateIcon;
			this.TransitionStateIcon_Low.color = this.LiquidState.StateColor;
			this.TransitionStateIcon_High.sprite = this.GasState.StateIcon;
			this.TransitionStateIcon_High.color = this.GasState.StateColor;
			break;
		}
	}

	private string SetCurrentTemperatureTooltip(Element element, int state)
	{
		string text = string.Empty;
		switch (state)
		{
		case 0:
			text = text + "\nMelts at: <color=yellow>" + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(element.highTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute) + "</color>";
			break;
		case 1:
			text = text + "\nFreezes at: <color=cyan>" + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(element.lowTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute) + "</color>";
			text = text + "\nEvaporates at: <color=red>" + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(element.highTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute) + "</color>";
			break;
		case 2:
			text = text + "\nCondenses at: <color=yellow>" + GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(element.lowTemp), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute) + "</color>";
			break;
		}
		return text;
	}

	private float GetTemperaturePosition(CellSelectionObject cellObject)
	{
		float num = 1f;
		float num2 = 2000f;
		switch (this.GetState(cellObject))
		{
		case 0:
			num2 = cellObject.element.highTemp;
			break;
		case 1:
			num = cellObject.element.lowTemp;
			num2 = cellObject.element.highTemp;
			break;
		case 2:
			num = cellObject.element.lowTemp;
			num2 = num + 300f;
			break;
		}
		float num3 = num2 - num;
		float num4 = this.temperaturePositionWidgetX_Min;
		float num5 = this.temperaturePositionWidgetX_Max;
		float num6 = num5 - num4;
		return Mathf.Clamp(num4 + (cellObject.temperature - num) * num6 / num3, num4, num5);
	}

	private float GetTemperaturePosition(PrimaryElement chunkObject)
	{
		float num = 1f;
		float highTemp = ElementLoader.FindElementByHash(chunkObject.ElementID).highTemp;
		float num2 = highTemp - num;
		float num3 = this.temperaturePositionWidgetX_Min;
		float num4 = this.temperaturePositionWidgetX_Max;
		float num5 = num4 - num3;
		return Mathf.Clamp(num3 + (chunkObject.Temperature - num) * num5 / num2, num3, num4);
	}

	private float GetTemperaturePosition(Edible edibleObject)
	{
		float num = 1f;
		float rotTemperature = edibleObject.FoodInfo.RotTemperature;
		float num2 = rotTemperature - num;
		float num3 = this.temperaturePositionWidgetX_Min;
		float num4 = this.temperaturePositionWidgetX_Max;
		float num5 = num4 - num3;
		float num6 = Grid.Temperature[Grid.PosToCell(edibleObject)];
		return Mathf.Clamp(num3 + num6 * num5 / num2, num3, num4);
	}

	public static string[] MassStrings(int cell)
	{
		string[] array = new string[]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};
		if (Grid.IsValidCell(cell))
		{
			float num = Grid.Cell[cell].mass;
			array[3] = " " + GameUtil.GetBreathableString(ElementLoader.elements[(int)Grid.Cell[cell].elementIdx], num);
			if (ElementLoader.elements[(int)Grid.Cell[cell].elementIdx] == ElementLoader.FindElementByHash(SimHashes.Vacuum))
			{
				array[0] = "N/A";
				array[1] = string.Empty;
				array[2] = string.Empty;
			}
			else if (ElementLoader.elements[(int)Grid.Cell[cell].elementIdx] == ElementLoader.FindElementByHash(SimHashes.Unobtanium))
			{
				array[0] = UI.NEUTRONIUMMASS;
				array[1] = string.Empty;
				array[2] = string.Empty;
			}
			else
			{
				array[2] = "kg";
				if (num < 5f)
				{
					num *= 1000f;
					array[2] = "g";
				}
				if (num < 5f)
				{
					num *= 1000f;
					array[2] = "mg";
				}
				if (num < 5f)
				{
					num *= 1000f;
					array[2] = "mcg";
					num = Mathf.Floor(num);
				}
				array[0] = string.Format("{0:0}", num);
				array[1] = "." + ((float)Mathf.FloorToInt(10f * (num - (float)Mathf.FloorToInt(num)))).ToString();
			}
		}
		return array;
	}

	public Text PropertyLeftText;

	public Text PropertyRightText;

	public Image PropertyIcon_Left;

	public Image PropertyIcon_Right;

	public WorldInspector.PropertyIcons propertySprites;

	public GameObject TemperatureNotch;

	public Image TemperatureNotchBG;

	public Image TemperatureNotchSymbol;

	public Text TemperatureTextDisplay;

	public Image TemperatureBarImage;

	public Image TransitionStateIcon_Low;

	public Image TransitionStateIcon_High;

	public ToolTip Tooltip_CurrentTemperature;

	public WorldInspector.StateSetting SolidState;

	public WorldInspector.StateSetting LiquidState;

	public WorldInspector.StateSetting GasState;

	private float temperaturePositionWidgetX_Min = 30f;

	private float temperaturePositionWidgetX_Max = 172f;

	[Serializable]
	public struct StateSetting
	{
		public Color StateColor;

		public Color StateColor_Dark;

		public Sprite TemperatureBarBG;

		public Sprite StateIcon;
	}

	[Serializable]
	public struct PropertyIcons
	{
		public Sprite Mass;

		public Sprite Rations;

		public Sprite Quality;

		public Sprite Resource;
	}
}
