using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonDisplayUI")]
public class LogicControlInputUI : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.colourOn = this.uiAsset.colourOn;
		this.colourOff = this.uiAsset.colourOff;
		this.colourOn.a = (this.colourOff.a = byte.MaxValue);
		this.colourDisconnected = this.uiAsset.colourDisconnected;
		this.icon.raycastTarget = false;
		this.border.raycastTarget = false;
	}

	public void SetContent(LogicCircuitNetwork network)
	{
		Color32 color = ((network == null) ? this.uiAsset.colourDisconnected : (network.IsBitActive(0) ? this.colourOn : this.colourOff));
		this.icon.color = color;
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image border;

	[SerializeField]
	private LogicModeUI uiAsset;

	private Color32 colourOn;

	private Color32 colourOff;

	private Color32 colourDisconnected;
}
