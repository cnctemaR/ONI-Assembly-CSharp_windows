using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	[DebuggerDisplay("{amount.Name} {value} ({deltaAttribute.value}/{minAttribute.value}/{maxAttribute.value})")]
	public class AmountInstance : ModifierInstance<Amount>, ISaveLoadable, ISim200ms
	{
		public Amount amount
		{
			get
			{
				return this.modifier;
			}
		}

		public bool paused
		{
			get
			{
				return this._paused;
			}
			set
			{
				this._paused = this.paused;
				if (this._paused)
				{
					this.Deactivate();
					return;
				}
				this.Activate();
			}
		}

		public float GetMin()
		{
			return this.minAttribute.GetTotalValue();
		}

		public float GetMax()
		{
			return this.maxAttribute.GetTotalValue();
		}

		public float GetDelta()
		{
			return this.deltaAttribute.GetTotalValue();
		}

		public AmountInstance(Amount amount, GameObject game_object)
			: base(game_object, amount)
		{
			Attributes attributes = game_object.GetAttributes();
			this.minAttribute = attributes.Add(amount.minAttribute);
			this.maxAttribute = attributes.Add(amount.maxAttribute);
			this.deltaAttribute = attributes.Add(amount.deltaAttribute);
		}

		public float SetValue(float value)
		{
			this.value = Mathf.Min(Mathf.Max(value, this.GetMin()), this.GetMax());
			return this.value;
		}

		public void Publish(float delta, float previous_value)
		{
			if (this.OnDelta != null)
			{
				this.OnDelta(delta);
			}
			if (this.OnMaxValueReached != null && previous_value < this.GetMax() && this.value >= this.GetMax())
			{
				this.OnMaxValueReached();
			}
		}

		public float ApplyDelta(float delta)
		{
			float num = this.value;
			this.SetValue(this.value + delta);
			this.Publish(delta, num);
			return this.value;
		}

		public string GetValueString()
		{
			return this.amount.GetValueString(this);
		}

		public string GetDescription()
		{
			return this.amount.GetDescription(this);
		}

		public string GetTooltip()
		{
			return this.amount.GetTooltip(this);
		}

		public void Activate()
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}

		public void Sim200ms(float dt)
		{
		}

		public static void BatchUpdate(List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances, float time_delta)
		{
			if (time_delta == 0f)
			{
				return;
			}
			AmountInstance.BatchUpdateContext batchUpdateContext = new AmountInstance.BatchUpdateContext(amount_instances, time_delta);
			AmountInstance.batch_update_job.Reset(batchUpdateContext);
			int num = 512;
			for (int i = 0; i < amount_instances.Count; i += num)
			{
				int num2 = i + num;
				if (amount_instances.Count < num2)
				{
					num2 = amount_instances.Count;
				}
				AmountInstance.batch_update_job.Add(new AmountInstance.BatchUpdateTask(i, num2));
			}
			GlobalJobManager.Run(AmountInstance.batch_update_job);
			batchUpdateContext.Finish();
			AmountInstance.batch_update_job.Reset(null);
		}

		public void Deactivate()
		{
			SimAndRenderScheduler.instance.Remove(this);
		}

		[Serialize]
		public float value;

		public AttributeInstance minAttribute;

		public AttributeInstance maxAttribute;

		public AttributeInstance deltaAttribute;

		public Action<float> OnDelta;

		public global::System.Action OnMaxValueReached;

		public bool hide;

		private bool _paused;

		private static WorkItemCollection<AmountInstance.BatchUpdateTask, AmountInstance.BatchUpdateContext> batch_update_job = new WorkItemCollection<AmountInstance.BatchUpdateTask, AmountInstance.BatchUpdateContext>();

		private class BatchUpdateContext
		{
			public BatchUpdateContext(List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances, float time_delta)
			{
				for (int num = 0; num != amount_instances.Count; num++)
				{
					UpdateBucketWithUpdater<ISim200ms>.Entry entry = amount_instances[num];
					entry.lastUpdateTime = 0f;
					amount_instances[num] = entry;
				}
				this.amount_instances = amount_instances;
				this.time_delta = time_delta;
				this.results = ListPool<AmountInstance.BatchUpdateContext.Result, AmountInstance>.Allocate();
				this.results.Capacity = this.amount_instances.Count;
			}

			public void Finish()
			{
				foreach (AmountInstance.BatchUpdateContext.Result result in this.results)
				{
					result.amount_instance.Publish(result.delta, result.previous);
				}
				this.results.Recycle();
			}

			public List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances;

			public float time_delta;

			public ListPool<AmountInstance.BatchUpdateContext.Result, AmountInstance>.PooledList results;

			public struct Result
			{
				public AmountInstance amount_instance;

				public float previous;

				public float delta;
			}
		}

		private struct BatchUpdateTask : IWorkItem<AmountInstance.BatchUpdateContext>
		{
			public BatchUpdateTask(int start, int end)
			{
				this.start = start;
				this.end = end;
			}

			public void Run(AmountInstance.BatchUpdateContext context)
			{
				for (int num = this.start; num != this.end; num++)
				{
					AmountInstance amountInstance = (AmountInstance)context.amount_instances[num].data;
					float num2 = amountInstance.GetDelta() * context.time_delta;
					if (num2 != 0f)
					{
						context.results.Add(new AmountInstance.BatchUpdateContext.Result
						{
							amount_instance = amountInstance,
							previous = amountInstance.value,
							delta = num2
						});
						amountInstance.SetValue(amountInstance.value + num2);
					}
				}
			}

			private int start;

			private int end;
		}
	}
}
