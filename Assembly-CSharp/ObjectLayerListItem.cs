using System;
using UnityEngine;

public class ObjectLayerListItem
{
	public ObjectLayerListItem(GameObject gameObject, ObjectLayer layer, int new_cell)
	{
		this.gameObject = gameObject;
		this.layer = layer;
		this.Refresh(new_cell);
	}

	public ObjectLayerListItem previousItem { get; private set; }

	public ObjectLayerListItem nextItem { get; private set; }

	public GameObject gameObject { get; private set; }

	public void Clear()
	{
		this.Refresh(Grid.InvalidCell);
	}

	public bool Refresh(int new_cell)
	{
		bool flag;
		if (this.cell != new_cell)
		{
			if (this.cell != Grid.InvalidCell)
			{
				if (Grid.Objects[this.cell, (int)this.layer] == this.gameObject)
				{
					GameObject gameObject = null;
					if (this.nextItem != null && this.nextItem.gameObject != null)
					{
						gameObject = this.nextItem.gameObject;
					}
					Grid.Objects[this.cell, (int)this.layer] = gameObject;
				}
			}
			if (this.previousItem != null)
			{
				this.previousItem.nextItem = this.nextItem;
			}
			if (this.nextItem != null)
			{
				this.nextItem.previousItem = this.previousItem;
			}
			this.previousItem = null;
			this.nextItem = null;
			this.cell = new_cell;
			if (this.cell != Grid.InvalidCell)
			{
				GameObject gameObject2 = Grid.Objects[this.cell, (int)this.layer];
				if (gameObject2 != null && gameObject2 != this.gameObject)
				{
					ObjectLayerListItem objectLayerListItem = gameObject2.GetComponent<Pickupable>().objectLayerListItem;
					this.nextItem = objectLayerListItem;
					objectLayerListItem.previousItem = this;
				}
				Grid.Objects[this.cell, (int)this.layer] = this.gameObject;
			}
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public bool Update(int cell)
	{
		return this.Refresh(cell);
	}

	private int cell = Grid.InvalidCell;

	private ObjectLayer layer;
}
