using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PreventFOWRevealTracker : KMonoBehaviour
{
	[OnSerializing]
	private void OnSerialize()
	{
		this.preventFOWRevealCells.Clear();
		for (int i = 0; i < Grid.PreventFogOfWarReveal.Length; i++)
		{
			if (Grid.PreventFogOfWarReveal[i])
			{
				this.preventFOWRevealCells.Add(i);
			}
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		foreach (int num in this.preventFOWRevealCells)
		{
			Grid.PreventFogOfWarReveal[num] = true;
		}
	}

	[Serialize]
	public List<int> preventFOWRevealCells;
}
