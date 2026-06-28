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

	private string name;

	public float min_value;

	public float max_value;

	private LocText name_label;

	public int guide_frequency;
}
