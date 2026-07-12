using System;

public interface ISecondaryOutput
{
	bool HasSecondaryConduitType(ConduitType type);

	CellOffset GetSecondaryConduitOffset(ConduitType type);
}
