using System;
using KSerialization;
using UnityEngine;

public class Reconstructable : KMonoBehaviour
{
	public bool AllowReconstruct
	{
		get
		{
			return this.deconstructable.allowDeconstruction && (this.building.Def.ShowInBuildMenu || SelectModuleSideScreen.moduleButtonSortOrder.Contains(this.building.Def.PrefabID));
		}
	}

	public Tag PrimarySelectedElementTag
	{
		get
		{
			return this.selectedElementsTags[0];
		}
	}

	public bool ReconstructRequested
	{
		get
		{
			return this.reconstructRequested;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void RequestReconstruct(Tag newElement)
	{
		if (!this.deconstructable.allowDeconstruction)
		{
			return;
		}
		this.reconstructRequested = !this.reconstructRequested;
		if (this.reconstructRequested)
		{
			this.deconstructable.QueueDeconstruction(false);
			this.selectedElementsTags = new Tag[] { newElement };
		}
		else
		{
			this.deconstructable.CancelDeconstruction();
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void CancelReconstructOrder()
	{
		this.reconstructRequested = false;
		this.deconstructable.CancelDeconstruction();
		base.Trigger(954267658, null);
	}

	public void TryCommenceReconstruct()
	{
		if (!this.deconstructable.allowDeconstruction)
		{
			return;
		}
		if (!this.reconstructRequested)
		{
			return;
		}
		string facadeID = this.building.GetComponent<BuildingFacade>().CurrentFacade;
		Vector3 position = this.building.transform.position;
		Orientation orientation = this.building.Orientation;
		GameScheduler.Instance.ScheduleNextFrame("Reconstruct", delegate(object data)
		{
			this.building.Def.TryPlace(null, position, orientation, this.selectedElementsTags, facadeID, false, 0);
		}, null, null);
	}

	[MyCmpReq]
	private Deconstructable deconstructable;

	[MyCmpReq]
	private Building building;

	[Serialize]
	private Tag[] selectedElementsTags;

	[Serialize]
	private bool reconstructRequested;
}
