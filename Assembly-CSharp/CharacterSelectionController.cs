using System;
using System.Collections.Generic;
using Klei.CustomSettings;
using UnityEngine;

public class CharacterSelectionController : KModalScreen
{
	public bool IsStarterMinion { get; set; }

	public bool AllowsReplacing
	{
		get
		{
			return this.allowsReplacing;
		}
	}

	protected virtual void OnProceed()
	{
	}

	protected virtual void OnDeliverableAdded()
	{
	}

	protected virtual void OnDeliverableRemoved()
	{
	}

	protected virtual void OnLimitReached()
	{
	}

	protected virtual void OnLimitUnreached()
	{
	}

	protected virtual void InitializeContainers()
	{
		this.DisableProceedButton();
		if (this.containers != null && this.containers.Count > 0)
		{
			return;
		}
		this.OnReplacedEvent = null;
		this.containers = new List<ITelepadDeliverableContainer>();
		if (this.IsStarterMinion || CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CarePackages).id != "Enabled")
		{
			this.numberOfDuplicantOptions = 3;
			this.numberOfCarePackageOptions = 0;
		}
		else
		{
			this.numberOfCarePackageOptions = ((global::UnityEngine.Random.Range(0, 101) <= 70) ? 1 : 2);
			this.numberOfDuplicantOptions = 4 - this.numberOfCarePackageOptions;
		}
		for (int i = 0; i < this.numberOfDuplicantOptions; i++)
		{
			CharacterContainer characterContainer = Util.KInstantiateUI<CharacterContainer>(this.containerPrefab.gameObject, this.containerParent, false);
			characterContainer.SetController(this);
			this.containers.Add(characterContainer);
		}
		for (int j = 0; j < this.numberOfCarePackageOptions; j++)
		{
			CarePackageContainer carePackageContainer = Util.KInstantiateUI<CarePackageContainer>(this.carePackageContainerPrefab.gameObject, this.containerParent, false);
			carePackageContainer.SetController(this);
			this.containers.Add(carePackageContainer);
			carePackageContainer.gameObject.transform.SetSiblingIndex(global::UnityEngine.Random.Range(0, carePackageContainer.transform.parent.childCount));
		}
		this.selectedDeliverables = new List<ITelepadDeliverable>();
	}

	public virtual void OnPressBack()
	{
		foreach (ITelepadDeliverableContainer telepadDeliverableContainer in this.containers)
		{
			CharacterContainer characterContainer = telepadDeliverableContainer as CharacterContainer;
			if (characterContainer != null)
			{
				characterContainer.ForceStopEditingTitle();
			}
		}
		base.Show(false);
	}

	public void RemoveLast()
	{
		if (this.selectedDeliverables == null || this.selectedDeliverables.Count == 0)
		{
			return;
		}
		ITelepadDeliverable telepadDeliverable = this.selectedDeliverables[this.selectedDeliverables.Count - 1];
		if (this.OnReplacedEvent != null)
		{
			this.OnReplacedEvent(telepadDeliverable);
		}
	}

	public void AddDeliverable(ITelepadDeliverable deliverable)
	{
		if (this.selectedDeliverables.Contains(deliverable))
		{
			global::Debug.Log("Tried to add the same minion twice.");
			return;
		}
		if (this.selectedDeliverables.Count >= this.selectableCount)
		{
			global::Debug.LogError("Tried to add minions beyond the allowed limit");
			return;
		}
		this.selectedDeliverables.Add(deliverable);
		this.OnDeliverableAdded();
		if (this.selectedDeliverables.Count == this.selectableCount)
		{
			this.EnableProceedButton();
			if (this.OnLimitReachedEvent != null)
			{
				this.OnLimitReachedEvent();
			}
			this.OnLimitReached();
		}
	}

	public void RemoveDeliverable(ITelepadDeliverable deliverable)
	{
		bool flag = this.selectedDeliverables.Count >= this.selectableCount;
		this.selectedDeliverables.Remove(deliverable);
		this.OnDeliverableRemoved();
		if (flag && this.selectedDeliverables.Count < this.selectableCount)
		{
			this.DisableProceedButton();
			if (this.OnLimitUnreachedEvent != null)
			{
				this.OnLimitUnreachedEvent();
			}
			this.OnLimitUnreached();
		}
	}

	public bool IsSelected(ITelepadDeliverable deliverable)
	{
		return this.selectedDeliverables.Contains(deliverable);
	}

	protected void EnableProceedButton()
	{
		this.proceedButton.isInteractable = true;
		this.proceedButton.ClearOnClick();
		this.proceedButton.onClick += delegate
		{
			this.OnProceed();
		};
	}

	protected void DisableProceedButton()
	{
		this.proceedButton.ClearOnClick();
		this.proceedButton.isInteractable = false;
		this.proceedButton.onClick += delegate
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		};
	}

	[SerializeField]
	private CharacterContainer containerPrefab;

	[SerializeField]
	private CarePackageContainer carePackageContainerPrefab;

	[SerializeField]
	private GameObject containerParent;

	[SerializeField]
	protected KButton proceedButton;

	protected int numberOfDuplicantOptions = 3;

	protected int numberOfCarePackageOptions;

	[SerializeField]
	protected int selectableCount;

	[SerializeField]
	private bool allowsReplacing;

	protected List<ITelepadDeliverable> selectedDeliverables;

	protected List<ITelepadDeliverableContainer> containers;

	public global::System.Action OnLimitReachedEvent;

	public global::System.Action OnLimitUnreachedEvent;

	public Action<bool> OnReshuffleEvent;

	public Action<ITelepadDeliverable> OnReplacedEvent;

	public global::System.Action OnProceedEvent;
}
