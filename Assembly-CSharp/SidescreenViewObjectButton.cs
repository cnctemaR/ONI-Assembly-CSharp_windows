using System;
using UnityEngine;

public class SidescreenViewObjectButton : KMonoBehaviour, ISidescreenButtonControl
{
	public bool IsValid()
	{
		SidescreenViewObjectButton.Mode trackMode = this.TrackMode;
		if (trackMode != SidescreenViewObjectButton.Mode.Target)
		{
			return trackMode == SidescreenViewObjectButton.Mode.Cell && Grid.IsValidCell(this.TargetCell);
		}
		return this.Target != null;
	}

	public string SidescreenButtonText
	{
		get
		{
			return this.Text;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			return this.Tooltip;
		}
	}

	public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
	{
		throw new NotImplementedException();
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public bool SidescreenButtonInteractable()
	{
		return this.IsValid();
	}

	public int HorizontalGroupID()
	{
		return this.horizontalGroupID;
	}

	public void OnSidescreenButtonPressed()
	{
		if (this.IsValid())
		{
			SidescreenViewObjectButton.Mode trackMode = this.TrackMode;
			if (trackMode == SidescreenViewObjectButton.Mode.Target)
			{
				GameUtil.FocusCamera(this.Target.transform.GetPosition(), 2f, true, true);
				return;
			}
			if (trackMode == SidescreenViewObjectButton.Mode.Cell)
			{
				GameUtil.FocusCamera(Grid.CellToPos(this.TargetCell), 2f, true, true);
				return;
			}
		}
		else
		{
			base.gameObject.Trigger(1980521255, null);
		}
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	public string Text;

	public string Tooltip;

	public SidescreenViewObjectButton.Mode TrackMode;

	public GameObject Target;

	public int TargetCell;

	public int horizontalGroupID = -1;

	public enum Mode
	{
		Target,
		Cell
	}
}
