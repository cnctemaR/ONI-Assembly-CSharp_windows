using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BuildWatermark : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		BuildWatermark.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RefreshText();
	}

	public void RefreshText()
	{
		string text = "CS-";
		bool flag = true;
		bool flag2 = DistributionPlatform.Initialized && DistributionPlatform.Inst.IsArchiveBranch;
		this.button.ClearOnClick();
		if (Application.isEditor)
		{
			text += "<EDITOR>";
		}
		else
		{
			text += 442154U.ToString();
			if (DebugHandler.enabled)
			{
				text += "-D";
			}
		}
		if (flag)
		{
			this.textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.WATERMARK, text));
			this.toolTip.ClearMultiStringTooltip();
		}
		else
		{
			this.textDisplay.SetText(string.Format(UI.DEVELOPMENTBUILDS.TESTING_WATERMARK, text));
			this.toolTip.SetSimpleTooltip(UI.DEVELOPMENTBUILDS.TESTING_TOOLTIP);
			this.button.onClick += this.ShowTestingMessage;
		}
		foreach (GameObject gameObject in this.archiveIcons)
		{
			gameObject.SetActive(flag && flag2);
		}
	}

	private void ShowTestingMessage()
	{
		Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas, true).PopupConfirmDialog(UI.DEVELOPMENTBUILDS.TESTING_MESSAGE, delegate
		{
			Application.OpenURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
		}, delegate
		{
		}, null, null, UI.DEVELOPMENTBUILDS.TESTING_MESSAGE_TITLE, UI.DEVELOPMENTBUILDS.TESTING_MORE_INFO, null, null);
	}

	public LocText textDisplay;

	public ToolTip toolTip;

	public KButton button;

	public List<GameObject> archiveIcons;

	public static BuildWatermark Instance;
}
