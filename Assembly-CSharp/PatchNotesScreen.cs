using System;
using UnityEngine;

public class PatchNotesScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.changesLabel.text = this.m_patchNotesText;
		this.closeButton.onClick += this.MarkAsReadAndClose;
		this.closeButton.soundPlayer.widget_sound_events()[0].OverrideAssetName = "HUD_Click_Close";
		this.okButton.onClick += this.MarkAsReadAndClose;
		this.previousVersion.onClick += delegate
		{
			Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
		};
		this.fullPatchNotes.onClick += this.OnPatchNotesClick;
	}

	public static bool ShouldShowScreen()
	{
		return KPlayerPrefs.GetInt("PatchNotesVersion") < PatchNotesScreen.PatchNotesVersion;
	}

	private void MarkAsReadAndClose()
	{
		KPlayerPrefs.SetInt("PatchNotesVersion", PatchNotesScreen.PatchNotesVersion);
		base.gameObject.SetActive(false);
	}

	public void UpdatePatchNotes(string patchNotesSummary, string url)
	{
		this.m_patchNotesUrl = url;
		this.m_patchNotesText = patchNotesSummary;
		this.changesLabel.text = this.m_patchNotesText;
	}

	private void OnPatchNotesClick()
	{
		Application.OpenURL(this.m_patchNotesUrl);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.MarkAsReadAndClose();
			return;
		}
		base.OnKeyDown(e);
	}

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton okButton;

	[SerializeField]
	private KButton fullPatchNotes;

	[SerializeField]
	private KButton previousVersion;

	[SerializeField]
	private LocText changesLabel;

	private string m_patchNotesUrl;

	private string m_patchNotesText;

	private static int PatchNotesVersion = 9;
}
