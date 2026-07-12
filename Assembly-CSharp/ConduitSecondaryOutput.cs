using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitSecondaryOutput")]
public class ConduitSecondaryOutput : KMonoBehaviour, ISecondaryOutput
{
	public bool HasSecondaryConduitType(ConduitType type)
	{
		return this.portInfo.conduitType == type;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (type == this.portInfo.conduitType)
		{
			return this.portInfo.offset;
		}
		return CellOffset.none;
	}

	[SerializeField]
	public ConduitPortInfo portInfo;
}
