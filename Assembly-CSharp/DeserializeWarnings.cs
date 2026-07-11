using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DeserializeWarnings")]
public class DeserializeWarnings : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		DeserializeWarnings.Instance = null;
	}

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
				global::Debug.LogWarning(message, obj);
				this.isSet = true;
			}
		}

		private bool isSet;
	}
}
