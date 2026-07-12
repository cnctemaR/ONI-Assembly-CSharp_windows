using System;

public class CustomGameSettingWidget : KMonoBehaviour
{
	public event Action<CustomGameSettingWidget> onSettingChanged;

	public event global::System.Action onRefresh;

	public event global::System.Action onDestroy;

	public virtual void Refresh()
	{
		if (this.onRefresh != null)
		{
			this.onRefresh();
		}
	}

	public void Notify()
	{
		if (this.onSettingChanged != null)
		{
			this.onSettingChanged(this);
		}
	}

	protected override void OnForcedCleanUp()
	{
		base.OnForcedCleanUp();
		if (this.onDestroy != null)
		{
			this.onDestroy();
		}
	}
}
