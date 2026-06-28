using System;

public class VisibleAreaUpdater
{
	public VisibleAreaUpdater(Action<int> outside_view_first_time_cb, Action<int> inside_view_first_time_cb, Action<int> inside_view_second_time_cb, Action<int> inside_view_repeat_cb, string name)
	{
		this.OutsideViewFirstTimeCallback = outside_view_first_time_cb;
		this.InsideViewFirstTimeCallback = inside_view_first_time_cb;
		this.InsideViewSecondTimeCallback = inside_view_second_time_cb;
		this.UpdateCallback = new Action<int>(this.InternalUpdateCell);
		this.Name = name;
	}

	public void Update()
	{
		if (CameraController.Instance != null && this.VisibleArea == null)
		{
			this.VisibleArea = CameraController.Instance.VisibleArea;
			this.VisibleArea.AddCallback(this.Name, new global::System.Action(this.OnVisibleAreaUpdate));
			this.VisibleArea.Run(this.InsideViewFirstTimeCallback);
			this.VisibleArea.Run(this.InsideViewRepeatCallback);
		}
	}

	private void OnVisibleAreaUpdate()
	{
		if (this.VisibleArea != null)
		{
			this.VisibleArea.Run(this.OutsideViewFirstTimeCallback, this.InsideViewFirstTimeCallback, this.InsideViewSecondTimeCallback);
		}
	}

	private void InternalUpdateCell(int cell)
	{
		this.OutsideViewFirstTimeCallback(cell);
		this.InsideViewFirstTimeCallback(cell);
	}

	public void UpdateCell(int cell)
	{
		if (this.VisibleArea != null)
		{
			this.VisibleArea.RunIfVisible(cell, this.UpdateCallback);
		}
	}

	private GridVisibleArea VisibleArea;

	private Action<int> OutsideViewFirstTimeCallback;

	private Action<int> InsideViewFirstTimeCallback;

	private Action<int> InsideViewSecondTimeCallback;

	private Action<int> InsideViewRepeatCallback = null;

	private Action<int> UpdateCallback;

	private string Name;
}
