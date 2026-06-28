using System;

public abstract class Logger
{
	public Logger(string name)
	{
		this.name = name;
	}

	public bool enableConsoleLogging { get; set; }

	public bool breakOnLog { get; set; }

	public abstract int Count { get; }

	public string GetName()
	{
		return this.name;
	}

	public void SetName(string name)
	{
		this.name = name;
	}

	public virtual void DebugDisplayLog()
	{
	}

	public static uint NextIdx;

	protected string name;
}
