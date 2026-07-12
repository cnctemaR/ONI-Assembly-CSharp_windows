using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AttachableBuilding")]
public class AttachableBuilding : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.RegisterWithAttachPoint(true);
		Components.AttachableBuildings.Add(this);
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this))
		{
			AttachableBuilding component = gameObject.GetComponent<AttachableBuilding>();
			if (component != null && component.onAttachmentNetworkChanged != null)
			{
				component.onAttachmentNetworkChanged(this);
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void RegisterWithAttachPoint(bool register)
	{
		BuildingDef buildingDef = null;
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		BuildingUnderConstruction component2 = base.GetComponent<BuildingUnderConstruction>();
		if (component != null)
		{
			buildingDef = component.Def;
		}
		else if (component2 != null)
		{
			buildingDef = component2.Def;
		}
		int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), buildingDef.attachablePosition);
		bool flag = false;
		int num2 = 0;
		while (!flag && num2 < Components.BuildingAttachPoints.Count)
		{
			for (int i = 0; i < Components.BuildingAttachPoints[num2].points.Length; i++)
			{
				if (num == Grid.OffsetCell(Grid.PosToCell(Components.BuildingAttachPoints[num2]), Components.BuildingAttachPoints[num2].points[i].position))
				{
					if (register)
					{
						Components.BuildingAttachPoints[num2].points[i].attachedBuilding = this;
					}
					else if (Components.BuildingAttachPoints[num2].points[i].attachedBuilding == this)
					{
						Components.BuildingAttachPoints[num2].points[i].attachedBuilding = null;
					}
					flag = true;
					break;
				}
			}
			num2++;
		}
	}

	public static void GetAttachedBelow(AttachableBuilding searchStart, ref List<GameObject> buildings)
	{
		AttachableBuilding attachableBuilding = searchStart;
		while (attachableBuilding != null)
		{
			BuildingAttachPoint attachedTo = attachableBuilding.GetAttachedTo();
			attachableBuilding = null;
			if (attachedTo != null)
			{
				buildings.Add(attachedTo.gameObject);
				attachableBuilding = attachedTo.GetComponent<AttachableBuilding>();
			}
		}
	}

	public static int CountAttachedBelow(AttachableBuilding searchStart)
	{
		int num = 0;
		AttachableBuilding attachableBuilding = searchStart;
		while (attachableBuilding != null)
		{
			BuildingAttachPoint attachedTo = attachableBuilding.GetAttachedTo();
			attachableBuilding = null;
			if (attachedTo != null)
			{
				num++;
				attachableBuilding = attachedTo.GetComponent<AttachableBuilding>();
			}
		}
		return num;
	}

	public static void GetAttachedAbove(AttachableBuilding searchStart, ref List<GameObject> buildings)
	{
		BuildingAttachPoint buildingAttachPoint = searchStart.GetComponent<BuildingAttachPoint>();
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
						AttachableBuilding attachableBuilding = (AttachableBuilding)obj;
						if (attachableBuilding == hardPoint.attachedBuilding)
						{
							buildings.Add(attachableBuilding.gameObject);
							buildingAttachPoint = attachableBuilding.GetComponent<BuildingAttachPoint>();
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
	}

	public static void NotifyBuildingsNetworkChanged(List<GameObject> buildings, AttachableBuilding attachable = null)
	{
		foreach (GameObject gameObject in buildings)
		{
			AttachableBuilding component = gameObject.GetComponent<AttachableBuilding>();
			if (component != null && component.onAttachmentNetworkChanged != null)
			{
				component.onAttachmentNetworkChanged(attachable);
			}
		}
	}

	public static List<GameObject> GetAttachedNetwork(AttachableBuilding searchStart)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(searchStart.gameObject);
		AttachableBuilding.GetAttachedAbove(searchStart, ref list);
		AttachableBuilding.GetAttachedBelow(searchStart, ref list);
		return list;
	}

	public BuildingAttachPoint GetAttachedTo()
	{
		for (int i = 0; i < Components.BuildingAttachPoints.Count; i++)
		{
			for (int j = 0; j < Components.BuildingAttachPoints[i].points.Length; j++)
			{
				if (Components.BuildingAttachPoints[i].points[j].attachedBuilding == this && (Components.BuildingAttachPoints[i].points[j].attachedBuilding.GetComponent<Deconstructable>() == null || !Components.BuildingAttachPoints[i].points[j].attachedBuilding.GetComponent<Deconstructable>().HasBeenDestroyed))
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
		AttachableBuilding.NotifyBuildingsNetworkChanged(AttachableBuilding.GetAttachedNetwork(this), this);
		this.RegisterWithAttachPoint(false);
		Components.AttachableBuildings.Remove(this);
	}

	public Tag attachableToTag;

	public Action<object> onAttachmentNetworkChanged;
}
