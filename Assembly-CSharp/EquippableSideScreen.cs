using System;
using System.Collections.Generic;
using UnityEngine;

public class EquippableSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		this.rowPool = new UIPool<EquippableSideScreenRow>(this.rowPrefab);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.ConsumeMouseScroll = true;
		base.OnSpawn();
	}

	public override void SetTarget(GameObject target)
	{
		if (this.targetAssignable != null && this.targetAssignable.RequiresRegion != null)
		{
			if (this.targetAssignable.RequiresRegion.OwnerRegion != null)
			{
				this.targetAssignable.RequiresRegion.OwnerRegion.OnValidStateChanged -= this.OnValidStateChanged;
			}
			this.targetAssignable.RequiresRegion.OnOwnerRegionSet -= this.OnRegionChanged;
		}
		this.targetAssignable = target.GetComponent<Equippable>();
		if (this.targetAssignable == null)
		{
			Debug.LogError("Object selected has no Assignable component.");
			return;
		}
		if (this.targetAssignable.RequiresRegion != null)
		{
			if (this.targetAssignable.RequiresRegion.OwnerRegion != null)
			{
				this.targetAssignable.RequiresRegion.OwnerRegion.OnValidStateChanged += this.OnValidStateChanged;
			}
			this.targetAssignable.RequiresRegion.OnOwnerRegionSet += this.OnRegionChanged;
		}
		base.gameObject.SetActive(true);
		bool flag = this.targetAssignable == null || (this.targetAssignable.RequiresRegion != null && this.targetAssignable.RequiresRegion.IsRegionValid(false));
		this.Refresh(flag);
	}

	private void OnRegionChanged(Region newRegion)
	{
		if (this.targetAssignable.RequiresRegion.OwnerRegion != null)
		{
			this.targetAssignable.RequiresRegion.OwnerRegion.OnValidStateChanged -= this.OnValidStateChanged;
		}
		newRegion.OnValidStateChanged += this.OnValidStateChanged;
	}

	private void OnValidStateChanged(bool state)
	{
		if (base.gameObject.activeInHierarchy)
		{
			this.Refresh(state);
		}
	}

	private void Update()
	{
		if (this.shouldUpdate)
		{
			this.UpdatePortraitAlphas();
		}
	}

	private void Refresh(bool isValid)
	{
		this.ClearContent();
		this.shouldUpdate = true;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
		{
			this.rowPool.GetFreeElement(this.grid, true).SetIdentity(minionIdentity, this.targetAssignable);
		}
	}

	private void ClearContent()
	{
		foreach (KButton kbutton in this.assignmentMap.Keys)
		{
			global::UnityEngine.Object.Destroy(kbutton.gameObject);
		}
		this.assignmentMap.Clear();
		this.portraitList.Clear();
		this.rowPool.ClearAll();
	}

	private void OnToggleClicked(KButton toggle)
	{
		if (!this.assignmentMap.ContainsKey(toggle))
		{
			Debug.LogError("Something went wrong when choosing the assignment character.");
			return;
		}
		if (this.targetAssignable.assignee == this.assignmentMap[toggle])
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
			this.targetAssignable.Assign(null);
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
			this.targetAssignable.ClickAssign(this.assignmentMap[toggle]);
		}
		bool flag = this.targetAssignable == null || (this.targetAssignable.RequiresRegion != null && this.targetAssignable.RequiresRegion.IsRegionValid(false));
		this.Refresh(flag);
	}

	private void UpdatePortraitAlphas()
	{
		float num = this.scrollRectTransform.rectTransform().rect.yMax + this.borderPadding_top;
		float num2 = this.scrollRectTransform.rectTransform().rect.yMin + this.borderPadding_bottom;
		foreach (CrewPortrait crewPortrait in this.portraitList)
		{
			float num3 = 0f;
			if (crewPortrait.transform.position.y > num)
			{
				num3 = Mathf.Abs(crewPortrait.transform.position.y - num);
			}
			else if (crewPortrait.transform.position.y < num2)
			{
				num3 = num2 - crewPortrait.transform.position.y;
			}
			float num4 = Mathf.Lerp(1f, 0.01f, num3 / this.fadeLength);
			crewPortrait.SetAlpha(num4);
		}
	}

	[SerializeField]
	private Transform scrollRectTransform;

	[SerializeField]
	private GameObject assignBtnPrefab;

	[SerializeField]
	private GameObject grid;

	[SerializeField]
	private float borderPadding_top = 200f;

	[SerializeField]
	private float borderPadding_bottom = 360f;

	[SerializeField]
	private float fadeLength = 75f;

	private Equippable targetAssignable;

	private Dictionary<KButton, Assignables> assignmentMap = new Dictionary<KButton, Assignables>();

	private List<CrewPortrait> portraitList = new List<CrewPortrait>();

	private bool shouldUpdate = true;

	[SerializeField]
	private EquippableSideScreenRow rowPrefab;

	private UIPool<EquippableSideScreenRow> rowPool;
}
