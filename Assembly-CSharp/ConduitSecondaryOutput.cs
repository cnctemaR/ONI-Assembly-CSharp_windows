using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitSecondaryOutput")]
public class ConduitSecondaryOutput : KMonoBehaviour, ISecondaryOutput
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
