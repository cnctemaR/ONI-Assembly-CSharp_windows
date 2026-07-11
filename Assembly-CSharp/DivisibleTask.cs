using System;

internal abstract class DivisibleTask<SharedData> : IWorkItem<SharedData>
{
	protected DivisibleTask(string name)
	{
		this.name = name;
	}

	public void Run(SharedData sharedData)
	{
		this.RunDivision(sharedData);
	}

	protected abstract void RunDivision(SharedData sharedData);

	public string name;

	public int start;

	public int end;
}
