using System;
using STRINGS;
using UnityEngine;

public class AccessControlSideScreenRow : AccessControlSideScreenDoor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.defaultButton.onValueChanged += this.OnDefaultButtonChanged;
	}

	private void OnDefaultButtonChanged(bool state)
	{
		this.UpdateButtonStates(!state);
		if (this.defaultClickedCallback != null)
		{
			this.defaultClickedCallback(this.targetIdentity, !state);
		}
	}

	protected override void UpdateButtonStates(bool isDefault)
	{
		base.UpdateButtonStates(isDefault);
		this.defaultButton.GetComponent<ToolTip>().SetSimpleTooltip(isDefault ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_CUSTOM : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_DEFAULT);
		this.defaultControls.SetActive(isDefault);
		this.customControls.SetActive(!isDefault);
	}

	public void SetMinionContent(MinionAssignablesProxy identity, AccessControl.Permission permission, bool isDefault, Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange, Action<MinionAssignablesProxy, bool> onDefaultClick)
	{
		base.SetContent(permission, onPermissionChange);
		if (identity == null)
		{
			global::Debug.LogError("Invalid data received.");
			return;
		}
		if (this.portraitInstance == null)
		{
			this.portraitInstance = Util.KInstantiateUI<CrewPortrait>(this.crewPortraitPrefab.gameObject, this.defaultButton.gameObject, false);
			this.portraitInstance.SetAlpha(1f);
		}
		this.targetIdentity = identity;
		this.portraitInstance.SetIdentityObject(identity, false);
		this.portraitInstance.SetSubTitle(isDefault ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_DEFAULT : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_CUSTOM);
		this.defaultClickedCallback = null;
		this.defaultButton.isOn = !isDefault;
		this.defaultClickedCallback = onDefaultClick;
	}

	[SerializeField]
	private CrewPortrait crewPortraitPrefab;

	private CrewPortrait portraitInstance;

	public KToggle defaultButton;

	public GameObject defaultControls;

	public GameObject customControls;

	private Action<MinionAssignablesProxy, bool> defaultClickedCallback;
}
