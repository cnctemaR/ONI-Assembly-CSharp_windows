using System;
using Steamworks;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.FEEDBACK_SCREEN.TITLE);
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.closeButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.bugForumsButton.onClick += delegate
		{
			App.OpenWebURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
		};
		this.suggestionForumsButton.onClick += delegate
		{
			App.OpenWebURL("https://forums.kleientertainment.com/forums/forum/133-oxygen-not-included-suggestions-and-feedback/");
		};
		this.logsDirectoryButton.onClick += delegate
		{
			App.OpenWebURL(Util.LogsFolder());
		};
		this.saveFilesDirectoryButton.onClick += delegate
		{
			App.OpenWebURL(SaveLoader.GetSavePrefix());
		};
		if (SteamUtils.IsSteamRunningOnSteamDeck())
		{
			this.logsDirectoryButton.GetComponentInParent<VerticalLayoutGroup>().padding = new RectOffset(0, 0, 0, 0);
			this.saveFilesDirectoryButton.gameObject.SetActive(false);
			this.logsDirectoryButton.gameObject.SetActive(false);
		}
	}

	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton bugForumsButton;

	public KButton suggestionForumsButton;

	public KButton logsDirectoryButton;

	public KButton saveFilesDirectoryButton;
}
