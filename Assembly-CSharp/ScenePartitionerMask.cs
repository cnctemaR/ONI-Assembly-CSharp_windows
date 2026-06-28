using System;

public class ScenePartitionerMask
{
	public ScenePartitionerMask(HashedString name, int mask)
	{
		this.name = name;
		this.mask = mask;
	}

	public HashedString name;

	public int mask;
}
