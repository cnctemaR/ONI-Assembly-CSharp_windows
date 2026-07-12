using System;
using UnityEngine;
using UnityEngine.UI;

public class SpecialCargoBayClusterSideScreen : ReceptacleSideScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<SpecialCargoBayClusterReceptacle>() != null;
	}

	protected override bool RequiresAvailableAmountToDeposit()
	{
		return false;
	}

	protected override void UpdateState(object data)
	{
		base.UpdateState(data);
		this.SetDescriptionSidescreenFoldState(this.targetReceptacle != null && this.targetReceptacle.Occupant == null);
	}

	protected override void SetResultDescriptions(GameObject go)
	{
		base.SetResultDescriptions(go);
		if (this.targetReceptacle != null && this.targetReceptacle.Occupant != null)
		{
			this.descriptionLabel.SetText("");
			this.SetDescriptionSidescreenFoldState(false);
			return;
		}
		this.SetDescriptionSidescreenFoldState(true);
	}

	public void SetDescriptionSidescreenFoldState(bool visible)
	{
		this.descriptionContent.minHeight = (visible ? this.descriptionLayoutDefaultSize : 0f);
	}

	public LayoutElement descriptionContent;

	public float descriptionLayoutDefaultSize = -1f;
}
