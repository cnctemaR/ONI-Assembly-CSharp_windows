using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : KMonoBehaviour
{
	private void Initialize()
	{
		if (this.unitLabel == null)
		{
			this.unitLabel = this.currentKJLabel.gameObject.GetComponentInChildrenOnly<LocText>();
		}
		if (this.sizeMap == null || this.sizeMap.Count == 0)
		{
			this.sizeMap = new Dictionary<float, float>();
			this.sizeMap.Add(20000f, 10f);
			this.sizeMap.Add(40000f, 25f);
			this.sizeMap.Add(60000f, 40f);
		}
	}

	public void SetContent(Battery bat)
	{
		if (bat == null)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			return;
		}
		this.Initialize();
		RectTransform component = this.batteryBG.GetComponent<RectTransform>();
		float num = 0f;
		foreach (KeyValuePair<float, float> keyValuePair in this.sizeMap)
		{
			if (bat.Capacity <= keyValuePair.Key)
			{
				num = keyValuePair.Value;
				break;
			}
		}
		this.batteryBG.sprite = ((bat.Capacity < 40000f) ? this.regularBatteryBG : this.bigBatteryBG);
		float num2 = 25f;
		component.sizeDelta = new Vector2(num, num2);
		BuildingEnabledButton component2 = bat.GetComponent<BuildingEnabledButton>();
		Color color;
		if (component2 != null && !component2.IsEnabled)
		{
			color = Color.gray;
		}
		else
		{
			color = ((bat.PercentFull < bat.PreviousPercentFull) ? this.energyDecreaseColor : this.energyIncreaseColor);
		}
		this.batteryMeter.color = color;
		this.batteryBG.color = color;
		float num3 = this.batteryBG.GetComponent<RectTransform>().rect.height * bat.PercentFull;
		this.batteryMeter.GetComponent<RectTransform>().sizeDelta = new Vector2(num - 5.5f, num3 - 5.5f);
		color.a = 1f;
		if (this.currentKJLabel.color != color)
		{
			this.currentKJLabel.color = color;
			this.unitLabel.color = color;
		}
		this.currentKJLabel.text = bat.JoulesAvailable.ToString("F0");
	}

	private const float UIUnit = 10f;

	[SerializeField]
	private LocText currentKJLabel;

	[SerializeField]
	private Image batteryBG;

	[SerializeField]
	private Image batteryMeter;

	[SerializeField]
	private Sprite regularBatteryBG;

	[SerializeField]
	private Sprite bigBatteryBG;

	[SerializeField]
	private Color energyIncreaseColor = Color.green;

	[SerializeField]
	private Color energyDecreaseColor = Color.red;

	private LocText unitLabel;

	private Dictionary<float, float> sizeMap;
}
