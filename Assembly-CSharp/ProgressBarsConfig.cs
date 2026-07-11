using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarsConfig : ScriptableObject
{
	public static void DestroyInstance()
	{
		ProgressBarsConfig.instance = null;
	}

	public static ProgressBarsConfig Instance
	{
		get
		{
			if (ProgressBarsConfig.instance == null)
			{
				ProgressBarsConfig.instance = Resources.Load<ProgressBarsConfig>("ProgressBarsConfig");
				ProgressBarsConfig.instance.Initialize();
			}
			return ProgressBarsConfig.instance;
		}
	}

	public void Initialize()
	{
		foreach (ProgressBarsConfig.BarData barData in this.barColorDataList)
		{
			this.barColorMap.Add(barData.barName, barData);
		}
	}

	public string GetBarDescription(string barName)
	{
		string text = "";
		if (this.IsBarNameValid(barName))
		{
			text = Strings.Get(this.barColorMap[barName].barDescriptionKey);
		}
		return text;
	}

	public Color GetBarColor(string barName)
	{
		Color color = Color.clear;
		if (this.IsBarNameValid(barName))
		{
			color = this.barColorMap[barName].barColor;
		}
		return color;
	}

	public bool IsBarNameValid(string barName)
	{
		if (string.IsNullOrEmpty(barName))
		{
			global::Debug.LogError("The barName provided was null or empty. Don't do that.");
			return false;
		}
		if (!this.barColorMap.ContainsKey(barName))
		{
			global::Debug.LogError(string.Format("No BarData found for the entry [ {0} ]", barName));
			return false;
		}
		return true;
	}

	public GameObject progressBarPrefab;

	public GameObject progressBarUIPrefab;

	public GameObject healthBarPrefab;

	public List<ProgressBarsConfig.BarData> barColorDataList = new List<ProgressBarsConfig.BarData>();

	public Dictionary<string, ProgressBarsConfig.BarData> barColorMap = new Dictionary<string, ProgressBarsConfig.BarData>();

	private static ProgressBarsConfig instance;

	[Serializable]
	public struct BarData
	{
		public string barName;

		public Color barColor;

		public string barDescriptionKey;
	}
}
