using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class MinSpecScreen : KScreen
{
	private bool MeetsMinRequirements()
	{
		Dictionary<string, Dictionary<string, object>> systemInfo = Global.Instance.SystemInfo;
		if (systemInfo == null)
		{
			return true;
		}
		Dictionary<string, object> dictionary = systemInfo["CPU"];
		Dictionary<string, object> dictionary2 = systemInfo["GPU"];
		long num = (long)dictionary["clockSpeed"];
		long num2 = (long)dictionary["numCores"];
		long num3 = (long)systemInfo["RAM"]["megsOfRam"];
		long num4 = (long)dictionary2["megsOfRam"];
		string text = (string)dictionary2["name"];
		bool flag = true;
		flag = flag && num >= 2000L;
		flag = flag && num2 >= 2L;
		flag = flag && num3 > 2048L;
		flag = flag && num4 >= 512L;
		return flag && !this.IsGPUUnsupported(text);
	}

	private bool IsGPUUnsupported(string gpu_name)
	{
		foreach (Regex regex in MinSpecScreen.UnsupportedGPUs)
		{
			if (regex.IsMatch(gpu_name))
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.okButton.onClick.AddListener(new UnityAction(this.OnClickQuit));
		if (this.MeetsMinRequirements())
		{
			this.Deactivate();
		}
	}

	private void OnClickQuit()
	{
		this.Deactivate();
	}

	protected override void OnActivate()
	{
		if (this.MeetsMinRequirements())
		{
			this.Deactivate();
		}
	}

	[SerializeField]
	private Button okButton;

	[SerializeField]
	private LocText bodyText;

	private static readonly Regex[] UnsupportedGPUs = new Regex[]
	{
		new Regex(".*Intel(R) HD Graphics Family.*"),
		new Regex(".*Intel(R) HD Graphics\\s*"),
		new Regex(".*Intel(R) HD Graphics P3000.*"),
		new Regex(".*Intel(R) HD Graphics 3000*."),
		new Regex(".*Intel(R).*HD Graphics 3000.*"),
		new Regex(".*Intel(R) HD Graphics 4000.*"),
		new Regex(".*Intel(R) HD Graphics 4400.*")
	};
}
