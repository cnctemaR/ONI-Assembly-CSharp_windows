using System;

internal struct EffectorEntryDecibel
{
	public EffectorEntryDecibel(string name, float value)
	{
		this.name = name;
		this.value = value;
		this.count = 1;
	}

	public string name;

	public int count;

	public float value;
}
