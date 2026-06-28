using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class SimFailedLoadScreen : KScreen
{
	private bool IsRuntimeInstalled()
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.bodyText.key = "STRINGS.UI.FRONTEND.MINSPECSCREEN.SIMFAILEDTOLOAD";
		this.okButton.onClick.AddListener(new UnityAction(this.OnClickQuit));
		if (this.IsRuntimeInstalled())
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
		if (this.IsRuntimeInstalled())
		{
			this.Deactivate();
		}
	}

	[SerializeField]
	private Button okButton;

	[SerializeField]
	private LocText bodyText;
}
