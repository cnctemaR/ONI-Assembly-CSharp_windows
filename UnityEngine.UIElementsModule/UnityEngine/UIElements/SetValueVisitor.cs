using System;
using Unity.Properties;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class SetValueVisitor<TSrcValue> : PathVisitor
	{
		public ConverterGroup group { get; set; }

		public override void Reset()
		{
			base.Reset();
			this.Value = default(TSrcValue);
			this.group = null;
		}

		protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
		{
			bool isReadOnly = property.IsReadOnly;
			if (isReadOnly)
			{
				base.ReturnCode = VisitReturnCode.AccessViolation;
			}
			else
			{
				TValue tvalue;
				bool flag = this.group != null && this.group.TryConvert<TSrcValue, TValue>(ref this.Value, out tvalue);
				if (flag)
				{
					property.SetValue(ref container, tvalue);
				}
				else
				{
					TValue tvalue2;
					bool flag2 = ConverterGroups.TryConvert<TSrcValue, TValue>(ref this.Value, out tvalue2);
					if (flag2)
					{
						property.SetValue(ref container, tvalue2);
					}
					else
					{
						base.ReturnCode = VisitReturnCode.InvalidCast;
					}
				}
			}
		}

		public static readonly ObjectPool<SetValueVisitor<TSrcValue>> Pool = new ObjectPool<SetValueVisitor<TSrcValue>>(() => new SetValueVisitor<TSrcValue>(), delegate(SetValueVisitor<TSrcValue> v)
		{
			v.Reset();
		}, null, null, true, 10, 10000);

		public TSrcValue Value;
	}
}
