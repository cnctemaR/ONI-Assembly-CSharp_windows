using System;

public class BuildingAttachPoint : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.BuildingAttachPoints.Add(this);
		this.TryAttachEmptyHardpoints();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private void TryAttachEmptyHardpoints()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			if (!(this.points[i].attachedBuilding != null))
			{
				bool flag = false;
				int num = 0;
				while (num < Components.AttachableBuildings.Count && !flag)
				{
					if (Components.AttachableBuildings[num].attachableToTag == this.points[i].attachableType && Grid.OffsetCell(Grid.PosToCell(base.gameObject), this.points[i].position) == Grid.PosToCell(Components.AttachableBuildings[num]))
					{
						this.points[i].attachedBuilding = Components.AttachableBuildings[num];
						flag = true;
					}
					num++;
				}
			}
		}
	}

	public bool AcceptsAttachment(Tag type, int cell)
	{
		int num = Grid.PosToCell(base.gameObject);
		for (int i = 0; i < this.points.Length; i++)
		{
			if (Grid.OffsetCell(num, this.points[i].position) == cell && this.points[i].attachableType == type)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.BuildingAttachPoints.Remove(this);
	}

	public BuildingAttachPoint.HardPoint[] points = new BuildingAttachPoint.HardPoint[0];

	[Serializable]
	public struct HardPoint
	{
		public HardPoint(CellOffset position, Tag attachableType, AttachableBuilding attachedBuilding)
		{
			this.position = position;
			this.attachableType = attachableType;
			this.attachedBuilding = attachedBuilding;
		}

		public CellOffset position;

		public Tag attachableType;

		public AttachableBuilding attachedBuilding;
	}
}
