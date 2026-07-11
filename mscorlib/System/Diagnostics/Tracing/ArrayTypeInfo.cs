using System;

namespace System.Diagnostics.Tracing
{
	internal sealed class ArrayTypeInfo<ElementType> : TraceLoggingTypeInfo<ElementType[]>
	{
		public ArrayTypeInfo(TraceLoggingTypeInfo<ElementType> elementInfo)
		{
			this.elementInfo = elementInfo;
		}

		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.BeginBufferedArray();
			this.elementInfo.WriteMetadata(collector, name, format);
			collector.EndBufferedArray();
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref ElementType[] value)
		{
			int num = collector.BeginBufferedArray();
			int num2 = 0;
			if (value != null)
			{
				num2 = value.Length;
				for (int i = 0; i < value.Length; i++)
				{
					this.elementInfo.WriteData(collector, ref value[i]);
				}
			}
			collector.EndBufferedArray(num, num2);
		}

		public override object GetData(object value)
		{
			ElementType[] array = (ElementType[])value;
			object[] array2 = new object[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = this.elementInfo.GetData(array[i]);
			}
			return array2;
		}

		private readonly TraceLoggingTypeInfo<ElementType> elementInfo;
	}
}
