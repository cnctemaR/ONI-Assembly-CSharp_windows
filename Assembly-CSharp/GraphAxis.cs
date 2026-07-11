using System;

[Serializable]
public struct GraphAxis
{
	public float range
	{
		get
		{
			return this.max_value - this.min_value;
		}
	}

	public string name;

	public float min_value;

	public float max_value;

	public float guide_frequency;
}
