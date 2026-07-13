using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Internal;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	public interface IAnalytic
	{
		bool TryGatherData(out IAnalytic.IData data, [NotNullWhen(false)] out Exception error);

		public interface IData
		{
		}

		public struct DataList<T> : IEnumerable, IAnalytic.IData where T : struct
		{
			public DataList(T[] datas)
			{
				this.m_UsageData = datas;
			}

			public IEnumerator GetEnumerator()
			{
				return this.m_UsageData.GetEnumerator();
			}

			private readonly T[] m_UsageData;
		}
	}
}
