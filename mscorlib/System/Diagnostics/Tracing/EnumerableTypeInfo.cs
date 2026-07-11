using System;
using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
	internal sealed class EnumerableTypeInfo<IterableType, ElementType> : TraceLoggingTypeInfo<IterableType> where IterableType : IEnumerable<ElementType>
	{
		public EnumerableTypeInfo(TraceLoggingTypeInfo<ElementType> elementInfo)
		{
			this.elementInfo = elementInfo;
		}

		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.BeginBufferedArray();
			this.elementInfo.WriteMetadata(collector, name, format);
			collector.EndBufferedArray();
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref IterableType value)
		{
			int num = collector.BeginBufferedArray();
			int num2 = 0;
			if (value != null)
			{
				foreach (ElementType elementType in value)
				{
					this.elementInfo.WriteData(collector, ref elementType);
					num2++;
				}
			}
			collector.EndBufferedArray(num, num2);
		}

		public override object GetData(object value)
		{
			IterableType iterableType = (IterableType)((object)value);
			List<object> list = new List<object>();
			foreach (ElementType elementType in iterableType)
			{
				list.Add(this.elementInfo.GetData(elementType));
			}
			return list.ToArray();
		}

		private readonly TraceLoggingTypeInfo<ElementType> elementInfo;
	}
}
