using System;
using STRINGS;

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
	}

	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton bugForumsButton;

	public KButton suggestionForumsButton;

	public KButton logsDirectoryButton;

	public KButton saveFilesDirectoryButton;
}
