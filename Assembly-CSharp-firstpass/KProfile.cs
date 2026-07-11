using System;

public class KProfile : IDisposable
{
	public KProfile(string name, string group = "Game")
	{
		this.name = name;
	}

	public void Dispose()
	{
	}

	private string name;
}
