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
		ToolTip toolTip = this.leftButton.GetComponent<ToolTip>();
		toolTip.SetSimpleTooltip((!this.leftButton.isOn) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_DISABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_ENABLED);
		toolTip = this.rightButton.GetComponent<ToolTip>();
		toolTip.SetSimpleTooltip((!this.rightButton.isOn) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_DISABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_ENABLED);
	}

	public void SetContent(AccessControl.Permission permission, Action<MinionIdentity, AccessControl.Permission> onPermissionChange)
	{
		this.permissionChangedCallback = onPermissionChange;
		this.leftButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoLeft;
		this.rightButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoRight;
		this.UpdateButtonStates(false);
	}

	public KToggle leftButton;

	public KToggle rightButton;

	private Action<MinionIdentity, AccessControl.Permission> permissionChangedCallback;

	protected MinionIdentity targetIdentity;
}
