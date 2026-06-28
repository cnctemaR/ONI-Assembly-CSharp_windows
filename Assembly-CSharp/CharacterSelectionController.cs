using System;
using System.Collections.Generic;
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

	protected virtual void OnCharacterAdded()
	{
	}

	protected virtual void OnCharacterRemoved()
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
		if (this.containers == null || this.containers.Count <= 0)
		{
			this.containers = new List<CharacterContainer>();
			for (int i = 0; i < this.availableCharCount; i++)
			{
				CharacterContainer characterContainer = Util.KInstantiateUI<CharacterContainer>(this.containerPrefab.gameObject, this.containerParent, false);
				characterContainer.SetController(this);
				this.containers.Add(characterContainer);
			}
			this.startingStats = new List<MinionStartingStats>();
		}
	}

	public void RemoveLast()
	{
		if (this.startingStats != null && this.startingStats.Count != 0)
		{
			MinionStartingStats minionStartingStats = this.startingStats[this.startingStats.Count - 1];
			if (this.OnReplacedEvent != null)
			{
				this.OnReplacedEvent(minionStartingStats);
			}
		}
	}

	public void AddCharacter(MinionStartingStats charStats)
	{
		if (this.startingStats.Contains(charStats))
		{
			global::Debug.Log("Tried to add the same minion twice.", null);
		}
		else if (this.startingStats.Count >= this.selectableCharCount)
		{
			global::Debug.LogError("Tried to add minions beyond the allowed limit", null);
		}
		else
		{
			this.startingStats.Add(charStats);
			this.OnCharacterAdded();
			if (this.startingStats.Count == this.selectableCharCount)
			{
				this.EnableProceedButton();
				if (this.OnLimitReachedEvent != null)
				{
					this.OnLimitReachedEvent();
				}
				this.OnLimitReached();
			}
		}
	}

	public void RemoveCharacter(MinionStartingStats charStats)
	{
		bool flag = this.startingStats.Count >= this.selectableCharCount;
		this.startingStats.Remove(charStats);
		this.OnCharacterRemoved();
		if (flag && this.startingStats.Count < this.selectableCharCount)
		{
			this.DisableProceedButton();
			if (this.OnLimitUnreachedEvent != null)
			{
				this.OnLimitUnreachedEvent();
			}
			this.OnLimitUnreached();
		}
	}

	public bool IsSelected(MinionStartingStats charStats)
	{
		return this.startingStats.Contains(charStats);
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
	private GameObject containerParent;

	[SerializeField]
	protected KButton proceedButton;

	[SerializeField]
	protected int availableCharCount;

	[SerializeField]
	protected int selectableCharCount;

	[SerializeField]
	private bool allowsReplacing = false;

	protected List<MinionStartingStats> startingStats;

	protected List<CharacterContainer> containers;

	public global::System.Action OnLimitReachedEvent;

	public global::System.Action OnLimitUnreachedEvent;

	public Action<bool> OnReshuffleEvent;

	public Action<MinionStartingStats> OnReplacedEvent;

	public global::System.Action OnProceedEvent;
}
