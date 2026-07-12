using System;

internal abstract class DivisibleTask<SharedData> : IWorkItem<SharedData>
{
	public void Run(SharedData sharedData, int threadIndex)
	{
		this.RunDivision(sharedData);
	}

	protected DivisibleTask(string name)
	{
		this.name = name;
	}

	protected abstract void RunDivision(SharedData sharedData);

	public string name;

	public int start;

	public int end;
}
