using System;

public interface ISecondaryOutput
{
	ConduitType GetSecondaryConduitType();

	CellOffset GetSecondaryConduitOffset();
}
