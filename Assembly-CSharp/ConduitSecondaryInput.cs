using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitSecondaryInput")]
public class ConduitSecondaryInput : KMonoBehaviour, ISecondaryInput
{
	public ConduitType GetSecondaryConduitType()
	{
		return this.portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return this.portInfo.offset;
	}

	[SerializeField]
	public ConduitPortInfo portInfo;
}
