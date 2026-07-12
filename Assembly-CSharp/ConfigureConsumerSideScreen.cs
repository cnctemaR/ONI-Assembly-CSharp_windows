using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureConsumerSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IConfigurableConsumer>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetProducer = target.GetComponent<IConfigurableConsumer>();
		if (this.settings == null)
		{
			this.settings = this.targetProducer.GetSettingOptions();
		}
		this.PopulateOptions();
	}

	private void ClearOldOptions()
	{
		if (this.descriptor != null)
		{
			this.descriptor.gameObject.SetActive(false);
		}
		for (int i = 0; i < this.settingToggles.Count; i++)
		{
			this.settingToggles[i].gameObject.SetActive(false);
		}
	}

	private void PopulateOptions()
	{
		this.ClearOldOptions();
		for (int i = this.settingToggles.Count; i < this.settings.Length; i++)
		{
			IConfigurableConsumerOption setting = this.settings[i];
			HierarchyReferences component = Util.KInstantiateUI(this.consumptionSettingTogglePrefab, this.consumptionSettingToggleContainer.gameObject, true).GetComponent<HierarchyReferences>();
			this.settingToggles.Add(component);
			component.GetReference<LocText>("Label").text = setting.GetName();
			component.GetReference<Image>("Image").sprite = setting.GetIcon();
			MultiToggle reference = component.GetReference<MultiToggle>("Toggle");
			reference.onClick = (global::System.Action)Delegate.Combine(reference.onClick, new global::System.Action(delegate
			{
				this.SelectOption(setting);
			}));
		}
		this.RefreshToggles();
		this.RefreshDetails();
	}

	private void SelectOption(IConfigurableConsumerOption option)
	{
		this.targetProducer.SetSelectedOption(option);
		this.RefreshToggles();
		this.RefreshDetails();
	}

	private void RefreshToggles()
	{
		for (int i = 0; i < this.settingToggles.Count; i++)
		{
			MultiToggle reference = this.settingToggles[i].GetReference<MultiToggle>("Toggle");
			reference.ChangeState((this.settings[i] == this.targetProducer.GetSelectedOption()) ? 1 : 0);
			reference.gameObject.SetActive(true);
		}
	}

	private void RefreshDetails()
	{
		if (this.descriptor == null)
		{
			GameObject gameObject = Util.KInstantiateUI(this.settingDescriptorPrefab, this.settingEffectRowsContainer.gameObject, true);
			this.descriptor = gameObject.GetComponent<LocText>();
		}
		IConfigurableConsumerOption selectedOption = this.targetProducer.GetSelectedOption();
		if (selectedOption != null)
		{
			this.descriptor.text = selectedOption.GetDetailedDescription();
			this.selectedOptionNameLabel.text = "<b>" + selectedOption.GetName() + "</b>";
			this.descriptor.gameObject.SetActive(true);
		}
	}

	public override int GetSideScreenSortOrder()
	{
		return 1;
	}

	[SerializeField]
	private RectTransform consumptionSettingToggleContainer;

	[SerializeField]
	private GameObject consumptionSettingTogglePrefab;

	[SerializeField]
	private RectTransform settingRequirementRowsContainer;

	[SerializeField]
	private RectTransform settingEffectRowsContainer;

	[SerializeField]
	private LocText selectedOptionNameLabel;

	[SerializeField]
	private GameObject settingDescriptorPrefab;

	private IConfigurableConsumer targetProducer;

	private IConfigurableConsumerOption[] settings;

	private LocText descriptor;

	private List<HierarchyReferences> settingToggles = new List<HierarchyReferences>();

	private List<GameObject> requirementRows = new List<GameObject>();
}
