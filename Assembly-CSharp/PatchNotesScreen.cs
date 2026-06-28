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
		this.okButton.onClick += this.MarkAsReadAndClose;
		this.Deactivate();
	}

	private void MarkAsReadAndClose()
	{
		PlayerPrefs.SetInt("BuildVersion", 208689);
		this.Deactivate();
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
	private LocText changesLabel;
}
