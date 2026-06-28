using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DoorToggleSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.buttonList = new List<DoorToggleSideScreen.DoorButtonInfo>
		{
			new DoorToggleSideScreen.DoorButtonInfo
			{
				button = this.openButton,
				state = Door.ControlState.Opened,
				currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN,
				pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN_PENDING
			},
			new DoorToggleSideScreen.DoorButtonInfo
			{
				button = this.autoButton,
				state = Door.ControlState.Auto,
				currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO,
				pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO_PENDING
			},
			new DoorToggleSideScreen.DoorButtonInfo
			{
				button = this.closeButton,
				state = Door.ControlState.Closed,
				currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE,
				pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE_PENDING
			}
		};
		foreach (DoorToggleSideScreen.DoorButtonInfo doorButtonInfo in this.buttonList)
		{
			doorButtonInfo.button.onClick += this.CreateCallback(doorButtonInfo.state);
		}
	}

	public override void SetTarget(GameObject target)
	{
		if (this.target != null)
		{
			this.ClearTarget();
		}
		base.SetTarget(target);
		this.target = target.GetComponent<Door>();
		this.accessTarget = target.GetComponent<AccessControl>();
		if (this.target == null)
		{
			return;
		}
		target.Subscribe(1734268753, new Action<object>(this.OnDoorStateChanged));
		target.Subscribe(-1525636549, new Action<object>(this.OnAccessControlChanged));
		this.Refresh();
		base.gameObject.SetActive(true);
	}

	public override void ClearTarget()
	{
		if (this.target != null)
		{
			this.target.Unsubscribe(1734268753, new Action<object>(this.OnDoorStateChanged));
			this.target.Unsubscribe(-1525636549, new Action<object>(this.OnAccessControlChanged));
		}
		this.target = null;
	}

	private void Refresh()
	{
		string text = null;
		string text2 = null;
		foreach (DoorToggleSideScreen.DoorButtonInfo doorButtonInfo in this.buttonList)
		{
			if (this.target.CurrentState == doorButtonInfo.state)
			{
				doorButtonInfo.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
				doorButtonInfo.button.isOn = true;
				text = doorButtonInfo.currentString;
			}
			else if (this.target.RequestedState == doorButtonInfo.state)
			{
				doorButtonInfo.button.GetComponent<ImageToggleStateThrobber>().enabled = true;
				doorButtonInfo.button.isOn = true;
				text2 = doorButtonInfo.pendingString;
			}
			else
			{
				doorButtonInfo.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
				doorButtonInfo.button.isOn = false;
			}
		}
		string text3 = text;
		if (text2 != null)
		{
			text3 = string.Format(UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.PENDING_FORMAT, text3, text2);
		}
		if (this.accessTarget != null && !this.accessTarget.Online)
		{
			text3 = string.Format(UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.ACCESS_FORMAT, text3, UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.ACCESS_OFFLINE);
		}
		this.description.SetText(text3);
	}

	private global::System.Action CreateCallback(Door.ControlState state)
	{
		return delegate
		{
			this.target.QueueStateChange(state);
			this.Refresh();
		};
	}

	private void OnDoorStateChanged(object data)
	{
		this.Refresh();
	}

	private void OnAccessControlChanged(object data)
	{
		this.Refresh();
	}

	[SerializeField]
	private KToggle openButton;

	[SerializeField]
	private KToggle autoButton;

	[SerializeField]
	private KToggle closeButton;

	[SerializeField]
	private LocText description;

	private Door target;

	private AccessControl accessTarget;

	private List<DoorToggleSideScreen.DoorButtonInfo> buttonList;

	private struct DoorButtonInfo
	{
		public KToggle button;

		public Door.ControlState state;

		public string currentString;

		public string pendingString;
	}
}
