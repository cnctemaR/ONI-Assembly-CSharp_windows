using System;

public class StandardOffsetTracker : OffsetTracker
{
	public StandardOffsetTracker(CellOffset[] offsets)
	{
		this.offsets = offsets;
	}
}
