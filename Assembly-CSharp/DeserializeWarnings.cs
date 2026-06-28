using System;
using UnityEngine;

public class DeserializeWarnings : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		DeserializeWarnings.Instance = this;
	}

	public DeserializeWarnings.Warning BuildingTemeperatureIsZeroKelvin;

	public DeserializeWarnings.Warning PipeContentsTemperatureIsNan;

	public DeserializeWarnings.Warning PrimaryElementTemperatureIsNan;

	public DeserializeWarnings.Warning PrimaryElementHasNoElement;

	public static DeserializeWarnings Instance;

	public struct Warning
	{
		public void Warn(string message, GameObject obj = null)
		{
			if (!this.isSet)
			{
				Output.LogCriticalWarning(new object[] { message, obj });
				this.isSet = true;
			}
		}

		private bool isSet;
	}
}
