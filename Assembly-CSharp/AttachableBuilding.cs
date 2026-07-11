using System;
using System.Collections.Generic;
using UnityEngine;

public class AttachableBuilding : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.RegisterWithAttachPoint(true);
		Components.AttachableBuildings.Add(this);
		base.Subscribe<AttachableBuilding>(486707561, AttachableBuilding.AttachmentNetworkChangedDelegate);
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this))
		{
			gameObject.Trigger(486707561, this);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private void AttachmentNetworkChanged(object attachableBuilding)
	{
		if (this.onAttachmentNetworkChanged != null)
		{
			this.onAttachmentNetworkChanged((AttachableBuilding)attachableBuilding);
		}
	}

	public void RegisterWithAttachPoint(bool register)
	{
		int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), Assets.GetBuildingDef(base.GetComponent<KPrefabID>().PrefabID().Name).attachablePosition);
		bool flag = false;
		int num2 = 0;
		while (!flag && num2 < Components.BuildingAttachPoints.Count)
		{
			for (int i = 0; i < Components.BuildingAttachPoints[num2].points.Length; i++)
			{
				if (num == Grid.OffsetCell(Grid.PosToCell(Components.BuildingAttachPoints[num2]), Components.BuildingAttachPoints[num2].points[i].position))
				{
					Components.BuildingAttachPoints[num2].points[i].attachedBuilding = (register ? this : null);
					flag = true;
					break;
				}
			}
			num2++;
		}
	}

	public static List<GameObject> GetAttachedNetwork(AttachableBuilding tip)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(tip.gameObject);
		AttachableBuilding attachableBuilding = tip;
		while (attachableBuilding != null)
		{
			BuildingAttachPoint attachedTo = attachableBuilding.GetAttachedTo();
			attachableBuilding = null;
			if (attachedTo != null)
			{
				list.Add(attachedTo.gameObject);
				attachableBuilding = attachedTo.GetComponent<AttachableBuilding>();
			}
		}
		BuildingAttachPoint buildingAttachPoint = tip.GetComponent<BuildingAttachPoint>();
		while (buildingAttachPoint != null)
		{
			bool flag = false;
			foreach (BuildingAttachPoint.HardPoint hardPoint in buildingAttachPoint.points)
			{
				if (flag)
				{
					break;
				}
				if (hardPoint.attachedBuilding != null)
				{
					foreach (object obj in Components.AttachableBuildings)
					{
						AttachableBuilding attachableBuilding2 = (AttachableBuilding)obj;
						if (attachableBuilding2 == hardPoint.attachedBuilding)
						{
							list.Add(attachableBuilding2.gameObject);
							buildingAttachPoint = attachableBuilding2.GetComponent<BuildingAttachPoint>();
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				buildingAttachPoint = null;
			}
		}
		return list;
	}

	public BuildingAttachPoint GetAttachedTo()
	{
		for (int i = 0; i < Components.BuildingAttachPoints.Count; i++)
		{
			for (int j = 0; j < Components.BuildingAttachPoints[i].points.Length; j++)
			{
				if (Components.BuildingAttachPoints[i].points[j].attachedBuilding == this)
				{
					return Components.BuildingAttachPoints[i];
				}
			}
		}
		return null;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.RegisterWithAttachPoint(false);
		Components.AttachableBuildings.Remove(this);
	}

	public Tag attachableToTag;

	public Action<AttachableBuilding> onAttachmentNetworkChanged;

	private static readonly EventSystem.IntraObjectHandler<AttachableBuilding> AttachmentNetworkChangedDelegate = new EventSystem.IntraObjectHandler<AttachableBuilding>(delegate(AttachableBuilding component, object data)
	{
		component.AttachmentNetworkChanged(data);
	});
}
