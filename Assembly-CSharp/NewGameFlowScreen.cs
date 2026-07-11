using System;

public abstract class NewGameFlowScreen : KModalScreen
{
	public event global::System.Action OnNavigateForward;

	public event global::System.Action OnNavigateBackward;

	protected void NavigateBackward()
	{
		this.OnNavigateBackward();
	}

	protected void NavigateForward()
	{
		this.OnNavigateForward();
	}
}
