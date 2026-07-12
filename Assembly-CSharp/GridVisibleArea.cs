using System;
using System.Collections.Generic;
using UnityEngine;

public class GridVisibleArea
{
	public GridArea CurrentArea
	{
		get
		{
			return this.Areas[0];
		}
	}

	public GridArea PreviousArea
	{
		get
		{
			return this.Areas[1];
		}
	}

	public GridArea PreviousPreviousArea
	{
		get
		{
			return this.Areas[2];
		}
	}

	public void Update()
	{
		this.Areas[2] = this.Areas[1];
		this.Areas[1] = this.Areas[0];
		this.Areas[0] = GridVisibleArea.GetVisibleArea();
		foreach (GridVisibleArea.Callback callback in this.Callbacks)
		{
			callback.OnUpdate();
		}
	}

	public void AddCallback(string name, global::System.Action on_update)
	{
		GridVisibleArea.Callback callback = new GridVisibleArea.Callback
		{
			Name = name,
			OnUpdate = on_update
		};
		this.Callbacks.Add(callback);
	}

	public void Run(Action<int> in_view)
	{
		if (in_view != null)
		{
			this.CurrentArea.Run(in_view);
		}
	}

	public void Run(Action<int> outside_view, Action<int> inside_view, Action<int> inside_view_second_time)
	{
		if (outside_view != null)
		{
			this.PreviousArea.RunOnDifference(this.CurrentArea, outside_view);
		}
		if (inside_view != null)
		{
			this.CurrentArea.RunOnDifference(this.PreviousArea, inside_view);
		}
		if (inside_view_second_time != null)
		{
			this.PreviousArea.RunOnDifference(this.PreviousPreviousArea, inside_view_second_time);
		}
	}

	public void RunIfVisible(int cell, Action<int> action)
	{
		this.CurrentArea.RunIfInside(cell, action);
	}

	public static GridArea GetVisibleArea()
	{
		GridArea gridArea = default(GridArea);
		Camera mainCamera = Game.Instance.MainCamera;
		if (mainCamera != null)
		{
			Vector3 vector = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, mainCamera.transform.GetPosition().z));
			Vector3 vector2 = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, mainCamera.transform.GetPosition().z));
			if (CameraController.Instance != null)
			{
				Vector2I vector2I;
				Vector2I vector2I2;
				CameraController.Instance.GetWorldCamera(out vector2I, out vector2I2);
				gridArea.SetExtents(Math.Max((int)(vector2.x - 0.5f), vector2I.x), Math.Max((int)(vector2.y - 0.5f), vector2I.y), Math.Min((int)(vector.x + 1.5f), vector2I2.x + vector2I.x), Math.Min((int)(vector.y + 1.5f), vector2I2.y + vector2I.y));
			}
			else
			{
				gridArea.SetExtents(Math.Max((int)(vector2.x - 0.5f), 0), Math.Max((int)(vector2.y - 0.5f), 0), Math.Min((int)(vector.x + 1.5f), Grid.WidthInCells), Math.Min((int)(vector.y + 1.5f), Grid.HeightInCells));
			}
		}
		return gridArea;
	}

	private GridArea[] Areas = new GridArea[3];

	private List<GridVisibleArea.Callback> Callbacks = new List<GridVisibleArea.Callback>();

	public struct Callback
	{
		public global::System.Action OnUpdate;

		public string Name;
	}
}
