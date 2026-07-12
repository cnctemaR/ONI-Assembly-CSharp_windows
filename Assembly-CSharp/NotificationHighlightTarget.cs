using System;

public class NotificationHighlightTarget : KMonoBehaviour
{
	protected void OnEnable()
	{
		this.controller = base.GetComponentInParent<NotificationHighlightController>();
		if (this.controller != null)
		{
			this.controller.AddTarget(this);
		}
	}

	protected override void OnDisable()
	{
		if (this.controller != null)
		{
			this.controller.RemoveTarget(this);
		}
	}

	public void View()
	{
		base.GetComponentInParent<NotificationHighlightController>().TargetViewed(this);
	}

	public string targetKey;

	private NotificationHighlightController controller;
}
