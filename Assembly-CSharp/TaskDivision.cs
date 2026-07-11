using System;
using UnityEngine;

internal class TaskDivision<Task, SharedData> where Task : DivisibleTask<SharedData>, new()
{
	public TaskDivision()
	{
		this.tasks = new Task[Math.Max(1, SystemInfo.processorCount - 1)];
		for (int num = 0; num != this.tasks.Length; num++)
		{
			this.tasks[num] = new Task();
			Task task = this.tasks[num];
			task.name += string.Format("{0}", num);
		}
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
		foreach (Task task in this.tasks)
		{
			task.Run(sharedData);
		}
	}

	public Task[] tasks;
}
