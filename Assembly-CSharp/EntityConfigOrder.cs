using System;

public class EntityConfigOrder : Attribute
{
	public EntityConfigOrder(int sort_order)
	{
		this.sortOrder = sort_order;
	}

	public int sortOrder;
}
