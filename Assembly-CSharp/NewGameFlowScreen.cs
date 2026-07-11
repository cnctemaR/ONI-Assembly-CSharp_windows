using System;
using System.Diagnostics;

public abstract class NewGameFlowScreen : KModalScreen
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action OnNavigateForward;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
