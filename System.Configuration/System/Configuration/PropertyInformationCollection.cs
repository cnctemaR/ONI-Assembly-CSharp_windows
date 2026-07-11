using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Configuration
{
	[Serializable]
	public sealed class PropertyInformationCollection : NameObjectCollectionBase
	{
		internal PropertyInformationCollection()
			: base(StringComparer.Ordinal)
		{
		}

		public void CopyTo(PropertyInformation[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		public PropertyInformation this[string propertyName]
		{
			get
			{
				return (PropertyInformation)base.BaseGet(propertyName);
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return new PropertyInformationCollection.PropertyInformationEnumerator(this);
		}

		internal void Add(PropertyInformation pi)
		{
			base.BaseAdd(pi.Name, pi);
		}

		[MonoTODO]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		private class PropertyInformationEnumerator : IEnumerator
		{
			public PropertyInformationEnumerator(PropertyInformationCollection collection)
			{
				this.collection = collection;
				this.position = -1;
			}

			public object Current
			{
				get
				{
					if (this.position < this.collection.Count && this.position >= 0)
					{
						return this.collection.BaseGet(this.position);
					}
					throw new InvalidOperationException();
				}
			}

			public bool MoveNext()
			{
				int num = this.position + 1;
				this.position = num;
				return num < this.collection.Count;
			}

			public void Reset()
			{
				this.position = -1;
			}

			private PropertyInformationCollection collection;

			private int position;
		}
	}
}
