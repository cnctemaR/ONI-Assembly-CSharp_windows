using System;
using UnityEngine;

public class PluginAssets : MonoBehaviour
{
	private void Awake()
	{
		PluginAssets.Instance = this;
	}

	public static PluginAssets Instance;

	public TextStyleSetting defaultTextStyleSetting;
}
