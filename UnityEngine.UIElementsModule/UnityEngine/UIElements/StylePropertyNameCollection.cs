using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public struct StylePropertyNameCollection : IEnumerable<StylePropertyName>, IEnumerable
	{
		internal StylePropertyNameCollection(List<StylePropertyName> list)
		{
			this.propertiesList = list;
		}

		public StylePropertyNameCollection.Enumerator GetEnumerator()
		{
			return new StylePropertyNameCollection.Enumerator(this.propertiesList.GetEnumerator());
		}

		IEnumerator<StylePropertyName> IEnumerable<StylePropertyName>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Contains(StylePropertyName stylePropertyName)
		{
			bool flag2;
			using (List<StylePropertyName>.Enumerator enumerator = this.propertiesList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StylePropertyName stylePropertyName2 = enumerator.Current;
					bool flag = stylePropertyName2 == stylePropertyName;
					if (flag)
					{
						return true;
					}
				}
				flag2 = false;
			}
			return flag2;
		}

		internal List<StylePropertyName> propertiesList;

		public struct Enumerator : IEnumerator<StylePropertyName>, IEnumerator, IDisposable
		{
			internal Enumerator(List<StylePropertyName>.Enumerator enumerator)
			{
				this.m_Enumerator = enumerator;
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public StylePropertyName Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Reset()
			{
			}

			public void Dispose()
			{
				this.m_Enumerator.Dispose();
			}

			private List<StylePropertyName>.Enumerator m_Enumerator;
		}
	}
}
