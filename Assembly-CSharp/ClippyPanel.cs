using System;
using UnityEngine;
using UnityEngine.UI;

public class ClippyPanel : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SpeedControlScreen.Instance.Pause(true);
		Game.Instance.Trigger(1634669191, null);
	}

	public void OnOk()
	{
		SpeedControlScreen.Instance.Unpause(true);
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public Text title;

	public Text detailText;

	public Text flavorText;

	public Image topicIcon;

	private KButton okButton;
}
