using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DoorToggleSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.InitButtons();
	}

	private void InitButtons()
	{
		this.buttonList.Add(new DoorToggleSideScreen.DoorButtonInfo
		{
			button = this.openButton,
			state = Door.ControlState.Opened,
			currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN,
			pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN_PENDING
		});
		this.buttonList.Add(new DoorToggleSideScreen.DoorButtonInfo
		{
			button = this.autoButton,
			state = Door.ControlState.Auto,
			currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO,
			pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO_PENDING
		});
		this.buttonList.Add(new DoorToggleSideScreen.DoorButtonInfo
		{
			button = this.closeButton,
			state = Door.ControlState.Closed,
			currentString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE,
			pendingString = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE_PENDING
		});
		using (List<DoorToggleSideScreen.DoorButtonInfo>.Enumerator enumerator = this.buttonList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				DoorToggleSideScreen.DoorButtonInfo info = enumerator.Current;
				DoorToggleSideScreen $this = this;
				info.button.onClick += delegate
				{
					$this.target.QueueStateChange(info.state);
					$this.Refresh();
				};
			}
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Door>() != null;
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
		if (this.buttonList == null || this.buttonList.Count == 0)
		{
			this.InitButtons();
		}
		foreach (DoorToggleSideScreen.DoorButtonInfo doorButtonInfo in this.buttonList)
		{
			if (this.target.CurrentState == doorButtonInfo.state && this.target.RequestedState == doorButtonInfo.state)
			{
				doorButtonInfo.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
				doorButtonInfo.button.isOn = true;
				foreach (KImage kimage in doorButtonInfo.button.GetComponentsInChildren<KImage>())
				{
					kimage.ColorState = KImage.ColorSelector.Active;
				}
				foreach (ImageToggleState imageToggleState in doorButtonInfo.button.GetComponentsInChildren<ImageToggleState>())
				{
					imageToggleState.SetActive();
					imageToggleState.SetActive();
				}
				text = doorButtonInfo.currentString;
			}
			else if (this.target.RequestedState == doorButtonInfo.state)
			{
				doorButtonInfo.button.GetComponent<ImageToggleStateThrobber>().enabled = true;
				doorButtonInfo.button.isOn = true;
				text2 = doorButtonInfo.pendingString;
				foreach (KImage kimage2 in doorButtonInfo.button.GetComponentsInChildren<KImage>())
				{
					kimage2.ColorState = KImage.ColorSelector.Active;
				}
			}
			else
			{
				doorButtonInfo.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
				foreach (KImage kimage3 in doorButtonInfo.button.GetComponentsInChildren<KImage>())
				{
					kimage3.ColorState = KImage.ColorSelector.Inactive;
				}
				doorButtonInfo.button.isOn = false;
				foreach (ImageToggleState imageToggleState2 in doorButtonInfo.button.GetComponentsInChildren<ImageToggleState>())
				{
					imageToggleState2.SetInactive();
					imageToggleState2.SetInactive();
				}
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
		if (this.target.building.Def.PrefabID == POIDoorInternalConfig.ID)
		{
			text3 = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.POI_INTERNAL;
			foreach (DoorToggleSideScreen.DoorButtonInfo doorButtonInfo2 in this.buttonList)
			{
				doorButtonInfo2.button.gameObject.SetActive(false);
			}
		}
		else
		{
			foreach (DoorToggleSideScreen.DoorButtonInfo doorButtonInfo3 in this.buttonList)
			{
				doorButtonInfo3.button.gameObject.SetActive(true);
			}
		}
		this.description.text = text3;
		this.description.gameObject.SetActive(!string.IsNullOrEmpty(text3));
		this.ContentContainer.SetActive(!this.target.isSealed);
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

	private List<DoorToggleSideScreen.DoorButtonInfo> buttonList = new List<DoorToggleSideScreen.DoorButtonInfo>();

	private struct DoorButtonInfo
	{
		public KToggle button;

		public Door.ControlState state;

		public string currentString;

		public string pendingString;
	}
}
