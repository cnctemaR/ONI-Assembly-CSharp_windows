using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class CellEventInstance : EventInstanceBase, ISaveLoadableJson
{
	public CellEventInstance(int cell, int data, int data2, CellEvent ev)
		: base(ev)
	{
		this.cell = cell;
		this.data = data;
		this.data2 = data2;
	}

	[Serialize]
	public int cell;

	[Serialize]
	public int data;

	[Serialize]
	public int data2;
}
