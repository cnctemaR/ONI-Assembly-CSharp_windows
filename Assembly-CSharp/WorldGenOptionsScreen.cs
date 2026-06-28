using System;
using STRINGS;
using UnityEngine;

public class WorldGenOptionsScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.TITLE);
		GameObject gameObject = this.enableButton.transform.GetChild(0).gameObject;
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.TOOLTIP);
		gameObject.transform.GetChild(0).gameObject.SetActive(ThreadedHttps<KleiMetrics>.Instance.enabled);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			this.Apply();
		};
		LocText component = this.enableButton.transform.GetChild(1).GetComponent<LocText>();
		component.SetText(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.ENABLE_BUTTON);
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		LocText component2 = this.dismissButton.transform.GetChild(0).GetComponent<LocText>();
		component2.SetText(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.DONE_BUTTON);
		this.closeButton.onClick += delegate
		{
			this.Deactivate();
		};
	}

	private void Apply()
	{
	}

	private void GetNewRandom()
	{
	}

	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public GameObject enableButton;
}
