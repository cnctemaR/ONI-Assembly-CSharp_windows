using System;

public class DemoOverScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Init();
		PlayerController.Instance.ActivateTool(SelectTool.Instance);
		SelectTool.Instance.Select(null, false);
	}

	private void Init()
	{
		this.QuitButton.onClick += delegate
		{
			this.Quit();
		};
	}

	private void Quit()
	{
		PauseScreen.TriggerQuitGame();
	}

	public KButton QuitButton;
}
