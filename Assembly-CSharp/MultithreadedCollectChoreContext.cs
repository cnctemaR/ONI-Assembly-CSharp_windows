using System;
using System.Collections.Generic;

public abstract class MultithreadedCollectChoreContext<ProviderType>
{
	public MultithreadedCollectChoreContext()
	{
	}

	public void Setup(ProviderType provider, ChoreConsumerState consumerState)
	{
		this.provider = provider;
		this.consumerState = consumerState;
		if (this.succeeded == null || this.succeeded.Length != GlobalJobManager.ThreadCount)
		{
			this.SetupThreadContext();
		}
	}

	private void SetupThreadContext()
	{
		if (this.succeeded != null)
		{
			this.TearDownThreadContext();
		}
		int threadCount = GlobalJobManager.ThreadCount;
		this.succeeded = new ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.PooledList[threadCount];
		this.failed = new ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.PooledList[threadCount];
		this.incomplete = new ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.PooledList[threadCount];
		for (int i = 0; i < threadCount; i++)
		{
			this.succeeded[i] = ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.Allocate();
			this.failed[i] = ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.Allocate();
			this.incomplete[i] = ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.Allocate();
		}
	}

	private void TearDownThreadContext()
	{
		int threadCount = GlobalJobManager.ThreadCount;
		for (int i = 0; i < threadCount; i++)
		{
			this.succeeded[i].Recycle();
			this.failed[i].Recycle();
			this.incomplete[i].Recycle();
		}
		this.succeeded = null;
		this.failed = null;
		this.incomplete = null;
	}

	public void Finish(List<Chore.Precondition.Context> pass, List<Chore.Precondition.Context> fail)
	{
		int threadCount = GlobalJobManager.ThreadCount;
		for (int i = 0; i < threadCount; i++)
		{
			pass.AddRange(this.succeeded[i]);
			this.succeeded[i].Clear();
			fail.AddRange(this.failed[i]);
			this.failed[i].Clear();
			foreach (Chore.Precondition.Context context in this.incomplete[i])
			{
				context.FinishPreconditions();
				if (context.IsSuccess())
				{
					pass.Add(context);
				}
				else
				{
					fail.Add(context);
				}
			}
			this.incomplete[i].Clear();
		}
	}

	public abstract void CollectChore(int index, List<Chore.Precondition.Context> succeed, List<Chore.Precondition.Context> incomplete, List<Chore.Precondition.Context> failed);

	public void DefaultCollectChore(int index, int threadIndex)
	{
		this.CollectChore(index, this.succeeded[threadIndex], this.incomplete[threadIndex], this.failed[threadIndex]);
	}

	public ProviderType provider;

	public ChoreConsumerState consumerState;

	public ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.PooledList[] succeeded;

	public ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.PooledList[] failed;

	public ListPool<Chore.Precondition.Context, MultithreadedCollectChoreContext<ProviderType>>.PooledList[] incomplete;

	public struct WorkBlock<Parent> : IWorkItem<Parent> where Parent : MultithreadedCollectChoreContext<ProviderType>
	{
		public WorkBlock(int start, int end)
		{
			this.start = start;
			this.end = end;
		}

		void IWorkItem<Parent>.Run(Parent shared_data, int threadIndex)
		{
			for (int i = this.start; i < this.end; i++)
			{
				shared_data.DefaultCollectChore(i, threadIndex);
			}
		}

		private int start;

		private int end;
	}
}
