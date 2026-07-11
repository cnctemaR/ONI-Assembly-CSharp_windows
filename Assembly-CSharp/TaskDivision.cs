using System;

internal class TaskDivision<Task, SharedData> where Task : DivisibleTask<SharedData>, new()
{
	public TaskDivision(int taskCount)
	{
		this.tasks = new Task[taskCount];
		for (int num = 0; num != this.tasks.Length; num++)
		{
			this.tasks[num] = new Task();
		}
	}

	public TaskDivision()
		: this(CPUBudget.coreCount)
	{
	}

	public void Initialize(int count)
	{
		int num = count / this.tasks.Length;
		for (int num2 = 0; num2 != this.tasks.Length; num2++)
		{
			this.tasks[num2].start = num2 * num;
			this.tasks[num2].end = this.tasks[num2].start + num;
		}
		DebugUtil.Assert(this.tasks[this.tasks.Length - 1].end + count % this.tasks.Length == count);
		this.tasks[this.tasks.Length - 1].end = count;
	}

	public void Run(SharedData sharedData)
	{
		Task[] array = this.tasks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Run(sharedData);
		}
	}

	public Task[] tasks;
}
