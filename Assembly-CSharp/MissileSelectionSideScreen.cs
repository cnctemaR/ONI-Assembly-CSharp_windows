using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MissileSelectionSideScreen : SideScreenContent
{
	public override int GetSideScreenSortOrder()
	{
		return 500;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<MissileLauncher.Instance>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetMissileLauncher = target.GetSMI<MissileLauncher.Instance>();
		this.Build();
	}

	private void Build()
	{
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.rows.Clear();
		this.UpdateLongRangeMissiles();
		foreach (Tag tag in this.ammunitiontags)
		{
			GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.listContainer, false);
			gameObject.gameObject.name = tag.ProperName();
			this.rows.Add(tag, gameObject);
		}
		this.Refresh();
	}

	private void UpdateLongRangeMissiles()
	{
		if (DlcManager.IsExpansion1Active())
		{
			if (!this.ammunitiontags.Contains("MissileLongRange"))
			{
				this.ammunitiontags.Add("MissileLongRange");
				return;
			}
		}
		else
		{
			if (GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.IdHash, -1) == null)
			{
				this.ammunitiontags.Remove("MissileLongRange");
				return;
			}
			if (!this.ammunitiontags.Contains("MissileLongRange"))
			{
				this.ammunitiontags.Add("MissileLongRange");
			}
		}
	}

	private void Refresh()
	{
		using (Dictionary<Tag, GameObject>.Enumerator enumerator = this.rows.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<Tag, GameObject> kvp = enumerator.Current;
				kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(kvp.Key.ProperNameStripLink());
				kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite(kvp.Key, "ui", false).first;
				kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Def.GetUISprite(kvp.Key, "ui", false).second;
				kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = delegate
				{
					this.targetMissileLauncher.ChangeAmmunition(kvp.Key, !this.targetMissileLauncher.AmmunitionIsAllowed(kvp.Key));
					ClusterDestinationSelector component = this.targetMissileLauncher.GetComponent<ClusterDestinationSelector>();
					if (component != null)
					{
						component.assignable = this.targetMissileLauncher.AmmunitionIsAllowed("MissileLongRange");
					}
					this.targetMissileLauncher.GetComponent<FlatTagFilterable>().currentlyUserAssignable = this.targetMissileLauncher.AmmunitionIsAllowed("MissileBasic");
					DetailsScreen.Instance.Refresh(SelectTool.Instance.selected.gameObject);
					this.Refresh();
				};
				kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(this.targetMissileLauncher.AmmunitionIsAllowed(kvp.Key) ? 1 : 0);
				kvp.Value.SetActive(true);
			}
		}
	}

	public override string GetTitle()
	{
		return UI.UISIDESCREENS.MISSILESELECTIONSIDESCREEN.TITLE;
	}

	private MissileLauncher.Instance targetMissileLauncher;

	[SerializeField]
	private GameObject rowPrefab;

	[SerializeField]
	private GameObject listContainer;

	[SerializeField]
	private LocText headerLabel;

	private List<Tag> ammunitiontags = new List<Tag> { "MissileBasic" };

	private Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();
}
