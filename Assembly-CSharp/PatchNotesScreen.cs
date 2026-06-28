using System;
using STRINGS;
using UnityEngine;

public class PatchNotesScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.changesLabel.text = string.Format(UI.FRONTEND.PATCHNOTESSCREEN.BODY, UI.FRONTEND.PATCHNOTESSCREEN.PATCHNOTES);
		this.closeButton.onClick += this.MarkAsReadAndClose;
		this.closeButton.soundPlayer.widget_sound_events()[0].OverrideAssetName = "HUD_Click_Close";
		this.okButton.onClick += this.MarkAsReadAndClose;
		this.fullPatchNotes.onClick += delegate
		{
			Application.OpenURL("http://forums.kleientertainment.com/forum/137-oxygen-not-included-latest-content-update/");
		};
		this.previousVersion.onClick += delegate
		{
			Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
		};
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

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.MarkAsReadAndClose();
		}
		else
		{
			base.OnKeyDown(e);
		}
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

	private static int PatchNotesVersion = 5;
}
