using System;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;

public class ActiveRangeSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.activateValueLabel.text = "100";
		this.deactivateValueLabel.text = "100";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.activateValueSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnActivateValueChanged));
		this.deactivateValueSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnDeactivateValueChanged));
	}

	private void OnActivateValueChanged(float new_value)
	{
		this.activateValueLabel.text = new_value.ToString();
		this.target.ActivateValue = new_value;
		if (this.target.ActivateValue < this.target.DeactivateValue)
		{
			this.activateValueSlider.value = this.deactivateValueSlider.value;
		}
		this.RefreshTooltips();
	}

	private void OnDeactivateValueChanged(float new_value)
	{
		this.deactivateValueLabel.text = new_value.ToString();
		this.target.DeactivateValue = new_value;
		if (this.target.DeactivateValue > this.target.ActivateValue)
		{
			this.deactivateValueSlider.value = this.activateValueSlider.value;
		}
		this.RefreshTooltips();
	}

	private void RefreshTooltips()
	{
		this.activateValueSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.target.ActivateTooltip, this.activateValueSlider.value));
		this.deactivateValueSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.target.DeactivateTooltip, this.deactivateValueSlider.value));
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received", null);
			return;
		}
		this.target = new_target.GetComponent<IActivationRangeTarget>();
		if (this.target == null)
		{
			global::Debug.LogError("The gameObject received does not contain a IActivationRangeTarget component", null);
			return;
		}
		this.activateLabel.text = this.target.ActivateSliderLabelText;
		this.deactivateLabel.text = this.target.DeactivateSliderLabelText;
		this.activateValueSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnActivateValueChanged));
		this.activateValueSlider.minValue = this.target.MinValue;
		this.activateValueSlider.maxValue = this.target.MaxValue;
		this.activateValueSlider.value = this.target.ActivateValue;
		this.activateValueSlider.wholeNumbers = this.target.UseWholeNumbers;
		this.activateValueLabel.text = this.target.ActivateValue.ToString();
		this.activateValueSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnActivateValueChanged));
		this.deactivateValueSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnDeactivateValueChanged));
		this.deactivateValueSlider.minValue = this.target.MinValue;
		this.deactivateValueSlider.maxValue = this.target.MaxValue;
		this.deactivateValueSlider.value = this.target.DeactivateValue;
		this.deactivateValueSlider.wholeNumbers = this.target.UseWholeNumbers;
		this.deactivateValueLabel.text = this.target.DeactivateValue.ToString();
		this.deactivateValueSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnDeactivateValueChanged));
		this.RefreshTooltips();
	}

	public override string GetTitle()
	{
		if (this.target != null)
		{
			return this.target.ActivationRangeTitleText;
		}
		return UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.NAME;
	}

	private IActivationRangeTarget target;

	[SerializeField]
	private KSlider activateValueSlider;

	[SerializeField]
	private KSlider deactivateValueSlider;

	[SerializeField]
	private LocText activateLabel;

	[SerializeField]
	private LocText deactivateLabel;

	[SerializeField]
	private LocText activateValueLabel;

	[SerializeField]
	private LocText deactivateValueLabel;
}
