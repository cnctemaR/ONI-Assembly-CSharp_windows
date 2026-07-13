using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlObject]
	public class SortColumnDescriptions : ICollection<SortColumnDescription>, IEnumerable<SortColumnDescription>, IEnumerable
	{
		private IList<SortColumnDescription> sortColumnDescriptions
		{
			get
			{
				return this.m_Descriptions;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action changed;

		public IEnumerator<SortColumnDescription> GetEnumerator()
		{
			return this.m_Descriptions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(SortColumnDescription item)
		{
			this.Insert(this.m_Descriptions.Count, item);
		}

		public void Clear()
		{
			while (this.m_Descriptions.Count > 0)
			{
				this.Remove(this.m_Descriptions[0]);
			}
		}

		public bool Contains(SortColumnDescription item)
		{
			return this.m_Descriptions.Contains(item);
		}

		public void CopyTo(SortColumnDescription[] array, int arrayIndex)
		{
			this.m_Descriptions.CopyTo(array, arrayIndex);
		}

		public bool Remove(SortColumnDescription desc)
		{
			bool flag = desc == null;
			if (flag)
			{
				throw new ArgumentException("Cannot remove null description");
			}
			bool flag2 = this.m_Descriptions.Remove(desc);
			bool flag3;
			if (flag2)
			{
				desc.column = null;
				desc.changed -= this.OnDescriptionChanged;
				Action action = this.changed;
				if (action != null)
				{
					action();
				}
				flag3 = true;
			}
			else
			{
				flag3 = false;
			}
			return flag3;
		}

		private void OnDescriptionChanged(SortColumnDescription desc)
		{
			Action action = this.changed;
			if (action != null)
			{
				action();
			}
		}

		public int Count
		{
			get
			{
				return this.m_Descriptions.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_Descriptions.IsReadOnly;
			}
		}

		public int IndexOf(SortColumnDescription desc)
		{
			return this.m_Descriptions.IndexOf(desc);
		}

		public void Insert(int index, SortColumnDescription desc)
		{
			bool flag = desc == null;
			if (flag)
			{
				throw new ArgumentException("Cannot insert null description");
			}
			bool flag2 = this.Contains(desc);
			if (flag2)
			{
				throw new ArgumentException("Already contains this description");
			}
			this.m_Descriptions.Insert(index, desc);
			desc.changed += this.OnDescriptionChanged;
			Action action = this.changed;
			if (action != null)
			{
				action();
			}
		}

		public void RemoveAt(int index)
		{
			this.Remove(this.m_Descriptions[index]);
		}

		public SortColumnDescription this[int index]
		{
			get
			{
				return this.m_Descriptions[index];
			}
		}

		[SerializeField]
		private readonly IList<SortColumnDescription> m_Descriptions = new List<SortColumnDescription>();

		[ExcludeFromDocs]
		[Serializable]
		public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(SortColumnDescriptions.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("sortColumnDescriptions", "sort-column-descriptions", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new SortColumnDescriptions();
			}

			public override void Deserialize(object obj)
			{
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.sortColumnDescriptions_UxmlAttributeFlags) && this.sortColumnDescriptions != null;
				if (flag)
				{
					SortColumnDescriptions sortColumnDescriptions = (SortColumnDescriptions)obj;
					foreach (SortColumnDescription.UxmlSerializedData uxmlSerializedData in this.sortColumnDescriptions)
					{
						SortColumnDescription sortColumnDescription = (SortColumnDescription)uxmlSerializedData.CreateInstance();
						uxmlSerializedData.Deserialize(sortColumnDescription);
						sortColumnDescriptions.Add(sortColumnDescription);
					}
				}
			}

			[SerializeReference]
			[UxmlObjectReference]
			private List<SortColumnDescription.UxmlSerializedData> sortColumnDescriptions;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags sortColumnDescriptions_UxmlAttributeFlags;
		}

		[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory<T> : UxmlObjectFactory<T, SortColumnDescriptions.UxmlObjectTraits<T>> where T : SortColumnDescriptions, new()
		{
		}

		[Obsolete("UxmlObjectFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory : SortColumnDescriptions.UxmlObjectFactory<SortColumnDescriptions>
		{
		}

		[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectTraits<T> : UnityEngine.UIElements.UxmlObjectTraits<T> where T : SortColumnDescriptions
		{
			public override void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ref obj, bag, cc);
				List<SortColumnDescription> valueFromBag = this.m_SortColumnDescriptions.GetValueFromBag(bag, cc);
				bool flag = valueFromBag != null;
				if (flag)
				{
					foreach (SortColumnDescription sortColumnDescription in valueFromBag)
					{
						obj.Add(sortColumnDescription);
					}
				}
			}

			private readonly UxmlObjectListAttributeDescription<SortColumnDescription> m_SortColumnDescriptions = new UxmlObjectListAttributeDescription<SortColumnDescription>();
		}
	}
}
