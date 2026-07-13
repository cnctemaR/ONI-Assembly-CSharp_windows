using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DevQuickActionTargetFollower : MonoBehaviour
{
	public new RectTransform transform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	public bool IsToggleOn
	{
		get
		{
			return this.toggle.isOn;
		}
	}

	private void Awake()
	{
		this.toggleOffColorBlock = this.toggle.colors;
		this.toggleOnColorBlock = this.toggle.colors;
		this.toggleOnColorBlock.normalColor = this.toggleOffColorBlock.pressedColor;
		this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
		this.toggle.SetIsOnWithoutNotify(true);
		this.RefreshToggleVisuals();
	}

	public void ManualToggle(bool val)
	{
		this.toggle.isOn = val;
	}

	public void OnToggleValueChanged(bool newValue)
	{
		this.RefreshToggleVisuals();
		Action<bool> onToggleChanged = this.OnToggleChanged;
		if (onToggleChanged == null)
		{
			return;
		}
		onToggleChanged(newValue);
	}

	public void RefreshToggleVisuals()
	{
		this.toggle.colors = (this.toggle.isOn ? this.toggleOnColorBlock : this.toggleOffColorBlock);
	}

	public void SetTarget(GameObject target)
	{
		this.Target = target;
	}

	private void Update()
	{
		this.Refresh();
	}

	public void Refresh()
	{
		if (this.Target != null)
		{
			Vector3 vector = CameraController.Instance.overlayCamera.WorldToScreenPoint(this.Target.transform.position);
			this.targetPivot.transform.SetPosition(vector);
			Vector3 localPosition = this.targetPivot.localPosition;
			localPosition.z = 0f;
			this.targetPivot.localPosition = localPosition;
			Vector3 vector2 = this.transform.position - vector;
			vector2.z = 0f;
			Vector3 vector3 = Vector3.Cross(Vector3.forward, vector2.normalized);
			this.line.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
			Vector2 sizeDelta = this.line.sizeDelta;
			sizeDelta.x = this.targetPivot.localPosition.magnitude;
			this.line.sizeDelta = sizeDelta;
		}
	}

	public void SetVisibleState(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	public Toggle toggle;

	public RectTransform targetPivot;

	public RectTransform line;

	private ColorBlock toggleOnColorBlock;

	private ColorBlock toggleOffColorBlock;

	private GameObject Target;

	public Action<bool> OnToggleChanged;
}
