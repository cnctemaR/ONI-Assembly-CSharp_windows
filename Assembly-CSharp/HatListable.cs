using System;

public class HatListable : IListableOption
{
	public HatListable(string name, string hat)
	{
		this.name = name;
		this.hat = hat;
	}

	public string name { get; private set; }

	public string hat { get; private set; }

	public string GetProperName()
	{
		return this.name;
	}
}
