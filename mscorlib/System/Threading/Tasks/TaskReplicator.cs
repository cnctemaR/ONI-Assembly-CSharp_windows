using System;
using System.Collections.Concurrent;

namespace System.Threading.Tasks
{
	internal class TaskReplicator
	{
		private TaskReplicator(ParallelOptions options, bool stopOnFirstFailure)
		{
			this._scheduler = options.TaskScheduler ?? TaskScheduler.Current;
			this._stopOnFirstFailure = stopOnFirstFailure;
		}

		public static void Run<TState>(TaskReplicator.ReplicatableUserAction<TState> action, ParallelOptions options, bool stopOnFirstFailure)
		{
			int num = ((options.EffectiveMaxConcurrencyLevel > 0) ? options.EffectiveMaxConcurrencyLevel : int.MaxValue);
			TaskReplicator taskReplicator = new TaskReplicator(options, stopOnFirstFailure);
			new TaskReplicator.Replica<TState>(taskReplicator, num, 1073741823, action).Start();
			TaskReplicator.Replica replica;
			while (taskReplicator._pendingReplicas.TryDequeue(out replica))
			{
				replica.Wait();
			}
			if (taskReplicator._exceptions != null)
			{
				throw new AggregateException(taskReplicator._exceptions);
			}
		}

		private static int GenerateCooperativeMultitaskingTaskTimeout()
		{
			int processorCount = PlatformHelper.ProcessorCount;
			int tickCount = Environment.TickCount;
			return 100 + tickCount % processorCount * 50;
		}

		private readonly TaskScheduler _scheduler;

		private readonly bool _stopOnFirstFailure;

		private readonly ConcurrentQueue<TaskReplicator.Replica> _pendingReplicas = new ConcurrentQueue<TaskReplicator.Replica>();

		private ConcurrentQueue<Exception> _exceptions;

		private bool _stopReplicating;

		private const int CooperativeMultitaskingTaskTimeout_Min = 100;

		private const int CooperativeMultitaskingTaskTimeout_Increment = 50;

		private const int CooperativeMultitaskingTaskTimeout_RootTask = 1073741823;

		public delegate void ReplicatableUserAction<TState>(ref TState replicaState, int timeout, out bool yieldedBeforeCompletion);

		private abstract class Replica
		{
			protected Replica(TaskReplicator replicator, int maxConcurrency, int timeout)
			{
				this._replicator = replicator;
				this._timeout = timeout;
				this._remainingConcurrency = maxConcurrency - 1;
				this._pendingTask = new Task(delegate(object s)
				{
					((TaskReplicator.Replica)s).Execute();
				}, this);
				this._replicator._pendingReplicas.Enqueue(this);
			}

			public void Start()
			{
				this._pendingTask.RunSynchronously(this._replicator._scheduler);
			}

			public void Wait()
			{
				Task pendingTask;
				while ((pendingTask = this._pendingTask) != null)
				{
					pendingTask.Wait();
				}
			}

			public void Execute()
			{
				try
				{
					if (!this._replicator._stopReplicating && this._remainingConcurrency > 0)
					{
						this.CreateNewReplica();
						this._remainingConcurrency = 0;
					}
					bool flag;
					this.ExecuteAction(out flag);
					if (flag)
					{
						this._pendingTask = new Task(delegate(object s)
						{
							((TaskReplicator.Replica)s).Execute();
						}, this, CancellationToken.None, TaskCreationOptions.None);
						this._pendingTask.Start(this._replicator._scheduler);
					}
					else
					{
						this._replicator._stopReplicating = true;
						this._pendingTask = null;
					}
				}
				catch (Exception ex)
				{
					LazyInitializer.EnsureInitialized<ConcurrentQueue<Exception>>(ref this._replicator._exceptions).Enqueue(ex);
					if (this._replicator._stopOnFirstFailure)
					{
						this._replicator._stopReplicating = true;
					}
					this._pendingTask = null;
				}
			}

			protected abstract void CreateNewReplica();

			protected abstract void ExecuteAction(out bool yieldedBeforeCompletion);

			protected readonly TaskReplicator _replicator;

			protected readonly int _timeout;

			protected int _remainingConcurrency;

			protected volatile Task _pendingTask;
		}

		private sealed class Replica<TState> : TaskReplicator.Replica
		{
			public Replica(TaskReplicator replicator, int maxConcurrency, int timeout, TaskReplicator.ReplicatableUserAction<TState> action)
				: base(replicator, maxConcurrency, timeout)
			{
				this._action = action;
			}

			protected override void CreateNewReplica()
			{
				new TaskReplicator.Replica<TState>(this._replicator, this._remainingConcurrency, TaskReplicator.GenerateCooperativeMultitaskingTaskTimeout(), this._action)._pendingTask.Start(this._replicator._scheduler);
			}

			protected override void ExecuteAction(out bool yieldedBeforeCompletion)
			{
				this._action(ref this._state, this._timeout, out yieldedBeforeCompletion);
			}

			private readonly TaskReplicator.ReplicatableUserAction<TState> _action;

			private TState _state;
		}
	}
}
