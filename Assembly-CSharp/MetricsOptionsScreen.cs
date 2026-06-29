using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MetricsOptionsScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.TITLE);
		GameObject gameObject = this.enableButton.GetComponent<HierarchyReferences>().GetReference("Button").gameObject;
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.METRICS_OPTIONS_SCREEN.TOOLTIP);
		gameObject.transform.GetChild(0).gameObject.SetActive(!KPrivacyPrefs.instance.disableDataCollection);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			this.OnClickToggle();
		};
		LocText reference = this.enableButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Text");
		reference.SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.ENABLE_BUTTON);
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		LocText reference2 = this.dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Text");
		reference2.SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.DONE_BUTTON);
		this.closeButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.descriptionButton.onClick.AddListener(delegate
		{
			Application.OpenURL("https://www.kleientertainment.com/privacy-policy");
		});
	}

	private void OnClickToggle()
	{
		KPrivacyPrefs.instance.disableDataCollection = !KPrivacyPrefs.instance.disableDataCollection;
		KPrivacyPrefs.Save();
		ThreadedHttps<KleiMetrics>.Instance.SetEnabled(!KPrivacyPrefs.instance.disableDataCollection);
		this.enableButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(ThreadedHttps<KleiMetrics>.Instance.enabled);
	}

	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public GameObject enableButton;

	public Button descriptionButton;
}
