using System;
using STRINGS;

public class AccessControlSideScreenDoor : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.leftButton.onClick += this.OnPermissionButtonClicked;
		this.rightButton.onClick += this.OnPermissionButtonClicked;
	}

	private void OnPermissionButtonClicked()
	{
		AccessControl.Permission permission;
		if (this.leftButton.isOn)
		{
			if (this.rightButton.isOn)
			{
				permission = AccessControl.Permission.Both;
			}
			else
			{
				permission = AccessControl.Permission.GoLeft;
			}
		}
		else if (this.rightButton.isOn)
		{
			permission = AccessControl.Permission.GoRight;
		}
		else
		{
			permission = AccessControl.Permission.Neither;
		}
		this.UpdateButtonStates(false);
		this.permissionChangedCallback(this.targetIdentity, permission);
	}

	protected virtual void UpdateButtonStates(bool isDefault)
	{
		ToolTip component = this.leftButton.GetComponent<ToolTip>();
		ToolTip component2 = this.rightButton.GetComponent<ToolTip>();
		if (this.isUpDown)
		{
			component.SetSimpleTooltip(this.leftButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_DISABLED);
			component2.SetSimpleTooltip(this.rightButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_DISABLED);
			return;
		}
		component.SetSimpleTooltip(this.leftButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_DISABLED);
		component2.SetSimpleTooltip(this.rightButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_DISABLED);
	}

	public void SetRotated(bool rotated)
	{
		this.isUpDown = rotated;
	}

	public void SetContent(AccessControl.Permission permission, Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange)
	{
		this.permissionChangedCallback = onPermissionChange;
		this.leftButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoLeft;
		this.rightButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoRight;
		this.UpdateButtonStates(false);
	}

	public KToggle leftButton;

	public KToggle rightButton;

	private Action<MinionAssignablesProxy, AccessControl.Permission> permissionChangedCallback;

	private bool isUpDown;

	protected MinionAssignablesProxy targetIdentity;
}
