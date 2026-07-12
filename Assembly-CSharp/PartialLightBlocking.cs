using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PartialLightBlocking : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.SetLightBlocking();
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		this.ClearLightBlocking();
		base.OnCleanUp();
	}

	public void SetLightBlocking()
	{
		int[] placementCells = base.GetComponent<Building>().PlacementCells;
		for (int i = 0; i < placementCells.Length; i++)
		{
			SimMessages.SetCellProperties(placementCells[i], 48);
		}
	}

	public void ClearLightBlocking()
	{
		int[] placementCells = base.GetComponent<Building>().PlacementCells;
		for (int i = 0; i < placementCells.Length; i++)
		{
			SimMessages.ClearCellProperties(placementCells[i], 48);
		}
	}

	private const byte PartialLightBlockingProperties = 48;
}
