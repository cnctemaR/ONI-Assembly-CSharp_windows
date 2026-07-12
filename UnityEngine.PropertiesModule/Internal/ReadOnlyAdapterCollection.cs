using System;
using System.Collections.Generic;

namespace Unity.Properties.Internal
{
	internal readonly struct ReadOnlyAdapterCollection
	{
		public ReadOnlyAdapterCollection(List<IPropertyVisitorAdapter> adapters)
		{
			this.m_Adapters = adapters;
		}

		public ReadOnlyAdapterCollection.Enumerator GetEnumerator()
		{
			return new ReadOnlyAdapterCollection.Enumerator(this);
		}

		private readonly List<IPropertyVisitorAdapter> m_Adapters;

		public struct Enumerator
		{
			public IPropertyVisitorAdapter Current { readonly get; private set; }

			public Enumerator(ReadOnlyAdapterCollection collection)
			{
				this.m_Adapters = collection.m_Adapters;
				this.m_Index = 0;
				this.Current = null;
			}

			public bool MoveNext()
			{
				bool flag = this.m_Adapters == null;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = this.m_Index >= this.m_Adapters.Count;
					if (flag3)
					{
						flag2 = false;
					}
					else
					{
						this.Current = this.m_Adapters[this.m_Index];
						this.m_Index++;
						flag2 = true;
					}
				}
				return flag2;
			}

			private void Reset()
			{
				this.m_Index = 0;
				this.Current = null;
			}

			private List<IPropertyVisitorAdapter> m_Adapters;

			private int m_Index;
		}
	}
}
