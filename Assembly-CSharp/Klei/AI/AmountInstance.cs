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
			AmountInstance.AmmountInstanceBatchUpdateDispatcher.Instance.Reset(batchUpdateContext);
			GlobalJobManager.Run(AmountInstance.AmmountInstanceBatchUpdateDispatcher.Instance);
			AmountInstance.AmmountInstanceBatchUpdateDispatcher.Instance.Finish();
			AmountInstance.AmmountInstanceBatchUpdateDispatcher.Instance.Reset(AmountInstance.BatchUpdateContext.EmptyContext);
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

		private struct BatchUpdateContext
		{
			public BatchUpdateContext(List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances, float time_delta)
			{
				this.amount_instances = amount_instances;
				this.time_delta = time_delta;
			}

			public List<UpdateBucketWithUpdater<ISim200ms>.Entry> amount_instances;

			public float time_delta;

			public static AmountInstance.BatchUpdateContext EmptyContext = new AmountInstance.BatchUpdateContext(null, 0f);

			public struct Result
			{
				public AmountInstance amount_instance;

				public float previous;

				public float delta;
			}
		}

		private class AmmountInstanceBatchUpdateDispatcher : WorkItemCollectionWithThreadContex<AmountInstance.BatchUpdateContext, List<AmountInstance.BatchUpdateContext.Result>>
		{
			public static AmountInstance.AmmountInstanceBatchUpdateDispatcher Instance
			{
				get
				{
					if (AmountInstance.AmmountInstanceBatchUpdateDispatcher.instance == null || AmountInstance.AmmountInstanceBatchUpdateDispatcher.instance.threadContexts.Count != GlobalJobManager.ThreadCount)
					{
						AmountInstance.AmmountInstanceBatchUpdateDispatcher.instance = new AmountInstance.AmmountInstanceBatchUpdateDispatcher();
					}
					return AmountInstance.AmmountInstanceBatchUpdateDispatcher.instance;
				}
			}

			public AmmountInstanceBatchUpdateDispatcher()
			{
				this.threadContexts = new List<List<AmountInstance.BatchUpdateContext.Result>>(GlobalJobManager.ThreadCount);
				for (int i = 0; i < GlobalJobManager.ThreadCount; i++)
				{
					this.threadContexts.Add(new List<AmountInstance.BatchUpdateContext.Result>());
				}
			}

			public void Reset(AmountInstance.BatchUpdateContext context)
			{
				this.sharedData = context;
				if (context.amount_instances == null)
				{
					this.count = 0;
					return;
				}
				this.count = (context.amount_instances.Count + 512 - 1) / 512;
			}

			public override void RunItem(int item, ref AmountInstance.BatchUpdateContext shared_data, List<AmountInstance.BatchUpdateContext.Result> thread_context, int threadIndex)
			{
				int num = item * 512;
				int num2 = Mathf.Min(num + 512, shared_data.amount_instances.Count);
				for (int i = num; i < num2; i++)
				{
					AmountInstance amountInstance = (AmountInstance)shared_data.amount_instances[i].data;
					float num3 = amountInstance.GetDelta() * shared_data.time_delta;
					if (num3 != 0f)
					{
						thread_context.Add(new AmountInstance.BatchUpdateContext.Result
						{
							amount_instance = amountInstance,
							previous = amountInstance.value,
							delta = num3
						});
						amountInstance.SetValue(amountInstance.value + num3);
					}
				}
			}

			public void Finish()
			{
				foreach (List<AmountInstance.BatchUpdateContext.Result> list in this.threadContexts)
				{
					foreach (AmountInstance.BatchUpdateContext.Result result in list)
					{
						result.amount_instance.Publish(result.delta, result.previous);
					}
					list.Clear();
				}
			}

			private const int kBatchSize = 512;

			private static AmountInstance.AmmountInstanceBatchUpdateDispatcher instance;
		}
	}
}
