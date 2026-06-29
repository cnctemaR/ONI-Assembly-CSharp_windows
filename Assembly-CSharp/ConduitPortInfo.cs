using System;

[Serializable]
public class ConduitPortInfo
{
	public ConduitPortInfo(ConduitType type, CellOffset offset)
	{
		this.conduitType = type;
		this.offset = offset;
	}

	public ConduitType conduitType;

	public CellOffset offset;
}
