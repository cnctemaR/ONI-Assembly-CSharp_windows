using System;

public interface ISecondaryInput
{
	ConduitType GetSecondaryConduitType();

	CellOffset GetSecondaryConduitOffset();
}
