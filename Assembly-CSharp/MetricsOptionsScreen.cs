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
		this.enableButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Text").SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.ENABLE_BUTTON);
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.closeButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.descriptionButton.onClick.AddListener(delegate
		{
			App.OpenWebURL("https://www.kleientertainment.com/privacy-policy");
		});
	}

	private void OnClickToggle()
	{
		KPrivacyPrefs.instance.disableDataCollection = !KPrivacyPrefs.instance.disableDataCollection;
		KPrivacyPrefs.Save();
		KPlayerPrefs.SetString("DisableDataCollection", KPrivacyPrefs.instance.disableDataCollection ? "yes" : "no");
		KPlayerPrefs.Save();
		ThreadedHttps<KleiMetrics>.Instance.SetEnabled(!KPrivacyPrefs.instance.disableDataCollection);
		this.enableButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(ThreadedHttps<KleiMetrics>.Instance.enabled);
	}

	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public GameObject enableButton;

	public Button descriptionButton;
}
