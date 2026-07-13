using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MetricsOptionsScreen : KModalScreen
{
	private bool IsSettingsDirty()
	{
		return this.disableDataCollection != KPrivacyPrefs.instance.disableDataCollection;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if ((e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight)) && !this.IsSettingsDirty())
		{
			this.Show(false);
		}
		base.OnKeyDown(e);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.disableDataCollection = KPrivacyPrefs.instance.disableDataCollection;
		this.title.SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.TITLE);
		GameObject gameObject = this.enableButton.GetComponent<HierarchyReferences>().GetReference("Button").gameObject;
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.METRICS_OPTIONS_SCREEN.TOOLTIP);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			this.OnClickToggle();
		};
		this.enableButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Text").SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.ENABLE_BUTTON);
		this.dismissButton.onClick += delegate
		{
			if (this.IsSettingsDirty())
			{
				this.ApplySettingsAndDoRestart();
				return;
			}
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
		this.openKleiAccountButton.onClick += this.OpenKleiAccount;
		this.Refresh();
	}

	private void OnClickToggle()
	{
		this.disableDataCollection = !this.disableDataCollection;
		this.enableButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(this.disableDataCollection);
		this.Refresh();
	}

	private void ApplySettingsAndDoRestart()
	{
		KPrivacyPrefs.instance.disableDataCollection = this.disableDataCollection;
		KPrivacyPrefs.Save();
		KPlayerPrefs.SetString("DisableDataCollection", KPrivacyPrefs.instance.disableDataCollection ? "yes" : "no");
		KPlayerPrefs.Save();
		ThreadedHttps<KleiMetrics>.Instance.SetEnabled(!KPrivacyPrefs.instance.disableDataCollection);
		this.enableButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(ThreadedHttps<KleiMetrics>.Instance.enabled);
		App.instance.Restart();
	}

	private void Refresh()
	{
		this.enableButton.GetComponent<HierarchyReferences>().GetReference("Button").transform.GetChild(0).gameObject.SetActive(!this.disableDataCollection);
		this.closeButton.isInteractable = !this.IsSettingsDirty();
		this.restartWarningText.gameObject.SetActive(this.IsSettingsDirty());
		if (this.IsSettingsDirty())
		{
			this.dismissButton.GetComponentInChildren<LocText>().text = UI.FRONTEND.METRICS_OPTIONS_SCREEN.RESTART_BUTTON;
			return;
		}
		this.dismissButton.GetComponentInChildren<LocText>().text = UI.FRONTEND.METRICS_OPTIONS_SCREEN.DONE_BUTTON;
	}

	private void OpenKleiAccount()
	{
		App.OpenWebURL("https://accounts.klei.com/login/auto?Game=ONI&ClientToken=" + KleiAccount.KleiToken);
	}

	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public GameObject enableButton;

	public Button descriptionButton;

	public LocText restartWarningText;

	private bool disableDataCollection;

	public KButton openKleiAccountButton;
}
