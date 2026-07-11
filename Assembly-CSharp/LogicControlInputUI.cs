using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonDisplayUI")]
public class LogicControlInputUI : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.colourOn = GlobalAssets.Instance.colorSet.logicOn;
		this.colourOff = GlobalAssets.Instance.colorSet.logicOff;
		this.colourOn.a = (this.colourOff.a = byte.MaxValue);
		this.colourDisconnected = GlobalAssets.Instance.colorSet.logicDisconnected;
		this.icon.raycastTarget = false;
		this.border.raycastTarget = false;
	}

	public void SetContent(LogicCircuitNetwork network)
	{
		Color32 color = ((network == null) ? GlobalAssets.Instance.colorSet.logicDisconnected : (network.IsBitActive(0) ? this.colourOn : this.colourOff));
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
