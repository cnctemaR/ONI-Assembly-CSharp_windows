using System;

public class GameOverScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Init();
	}

	private void Init()
	{
		if (this.QuitButton)
		{
			this.QuitButton.onClick += delegate
			{
				this.Quit();
			};
		}
		if (this.DismissButton)
		{
			this.DismissButton.onClick += delegate
			{
				this.Dismiss();
			};
		}
	}

	private void Quit()
	{
		PauseScreen.TriggerQuitGame();
	}

	private void Dismiss()
	{
		base.Show(false);
	}

	public KButton DismissButton;

	public KButton QuitButton;
}
