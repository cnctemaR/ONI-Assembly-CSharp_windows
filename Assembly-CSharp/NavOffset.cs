using System;

public struct NavOffset
{
	public NavOffset(NavType nav_type, int x, int y)
	{
		this.navType = nav_type;
		this.offset.x = x;
		this.offset.y = y;
	}

	public NavType navType;

	public CellOffset offset;
}
