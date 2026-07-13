using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal abstract class UxmlAsset : IUxmlAttributes
	{
		public UxmlAsset(string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		{
			this.m_FullTypeName = fullTypeName;
			this.m_XmlNamespace = xmlNamespace;
		}

		public string fullTypeName
		{
			get
			{
				return this.m_FullTypeName;
			}
			set
			{
				this.m_FullTypeName = value;
			}
		}

		public UxmlNamespaceDefinition xmlNamespace
		{
			get
			{
				return this.m_XmlNamespace;
			}
			set
			{
				this.m_XmlNamespace = value;
			}
		}

		public int id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				this.m_Id = value;
			}
		}

		public bool isNull
		{
			get
			{
				return this.fullTypeName == "null";
			}
		}

		public bool isRoot
		{
			get
			{
				return this.fullTypeName.Equals("UXML", StringComparison.Ordinal) || this.fullTypeName.Equals("UnityEngine.UIElements.UXML", StringComparison.Ordinal) || this.fullTypeName.EndsWith(".UXML", StringComparison.Ordinal);
			}
		}

		public UxmlAsset parentAsset
		{
			get
			{
				return this.m_Parent;
			}
		}

		internal VisualTreeAsset visualTreeAsset
		{
			get
			{
				return this.m_VisualTreeAsset;
			}
		}

		public int childCount
		{
			get
			{
				List<UxmlAsset> children = this.m_Children;
				return (children != null) ? children.Count : 0;
			}
		}

		public UxmlAsset this[int index]
		{
			get
			{
				return this.m_Children[index];
			}
		}

		public List<UxmlNamespaceDefinition> namespaceDefinitions
		{
			get
			{
				List<UxmlNamespaceDefinition> list;
				if ((list = this.m_NamespaceDefinitions) == null)
				{
					list = (this.m_NamespaceDefinitions = new List<UxmlNamespaceDefinition>());
				}
				return list;
			}
		}

		public List<UxmlProperty> properties
		{
			get
			{
				return this.m_Properties;
			}
		}

		public void GetChildren(List<UxmlAsset> children)
		{
			children.Clear();
			for (int i = 0; i < this.childCount; i++)
			{
				children.Add(this[i]);
			}
		}

		public void GetChildrenUxmlObjectAssets(List<UxmlObjectAsset> children)
		{
			children.Clear();
			for (int i = 0; i < this.childCount; i++)
			{
				UxmlObjectAsset uxmlObjectAsset = this[i] as UxmlObjectAsset;
				bool flag = uxmlObjectAsset != null;
				if (flag)
				{
					children.Add(uxmlObjectAsset);
				}
			}
		}

		public bool HasAnyUxmlObjectAsset()
		{
			for (int i = 0; i < this.childCount; i++)
			{
				bool flag = this[i] is UxmlObjectAsset;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public UxmlObjectAsset GetField(string fieldName)
		{
			for (int i = 0; i < this.childCount; i++)
			{
				UxmlObjectAsset uxmlObjectAsset = this[i] as UxmlObjectAsset;
				bool flag = uxmlObjectAsset == null;
				if (!flag)
				{
					bool flag2 = uxmlObjectAsset.isField && uxmlObjectAsset.fullTypeName == fieldName;
					if (flag2)
					{
						return uxmlObjectAsset;
					}
				}
			}
			return null;
		}

		private void RemoveNonFields()
		{
			for (int i = this.childCount - 1; i >= 0; i--)
			{
				UxmlObjectAsset uxmlObjectAsset = this[i] as UxmlObjectAsset;
				bool flag = uxmlObjectAsset == null;
				if (!flag)
				{
					bool flag2 = !uxmlObjectAsset.isField;
					if (flag2)
					{
						uxmlObjectAsset.RemoveFromHierarchy();
					}
				}
			}
		}

		public void RemoveUxmlObjectAssetChildren()
		{
			for (int i = this.childCount - 1; i >= 0; i--)
			{
				UxmlObjectAsset uxmlObjectAsset = this[i] as UxmlObjectAsset;
				bool flag = uxmlObjectAsset == null;
				if (!flag)
				{
					uxmlObjectAsset.RemoveFromHierarchy();
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetUxmlObjectAssets(string fieldName, List<UxmlObjectAsset> entries)
		{
			bool flag = !string.IsNullOrEmpty(fieldName);
			if (flag)
			{
				UxmlObjectAsset field = this.GetField(fieldName);
				if (field != null)
				{
					field.SetUxmlObjectAssets(null, entries);
				}
			}
			else
			{
				this.RemoveNonFields();
				foreach (UxmlObjectAsset uxmlObjectAsset in entries)
				{
					this.Add(uxmlObjectAsset);
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void CollectUxmlObjectAssets(string fieldName, List<UxmlObjectAsset> foundEntries)
		{
			for (int i = 0; i < this.childCount; i++)
			{
				UxmlObjectAsset uxmlObjectAsset = this[i] as UxmlObjectAsset;
				bool flag = uxmlObjectAsset == null;
				if (!flag)
				{
					bool flag2 = !string.IsNullOrEmpty(fieldName) && uxmlObjectAsset.isField && uxmlObjectAsset.fullTypeName == fieldName;
					if (flag2)
					{
						uxmlObjectAsset.CollectUxmlObjectAssets(null, foundEntries);
						break;
					}
					bool isField = uxmlObjectAsset.isField;
					if (!isField)
					{
						foundEntries.Add(uxmlObjectAsset);
					}
				}
			}
		}

		public virtual void GetExportTypename(out string typename, out UxmlNamespaceDefinition resolvedNamespace)
		{
			bool flag = this.xmlNamespace != UxmlNamespaceDefinition.Empty;
			if (flag)
			{
				UxmlNamespaceDefinition uxmlNamespaceDefinition = this.m_VisualTreeAsset.FindUxmlNamespaceDefinitionFromPrefix(this, this.xmlNamespace.prefix);
				bool flag2 = uxmlNamespaceDefinition != this.xmlNamespace;
				if (flag2)
				{
					this.xmlNamespace = this.m_VisualTreeAsset.FindUxmlNamespaceDefinitionForTypeName(this, this.fullTypeName);
				}
			}
			bool flag3 = string.IsNullOrEmpty(this.xmlNamespace.prefix);
			if (flag3)
			{
				bool flag4 = string.IsNullOrEmpty(this.xmlNamespace.resolvedNamespace);
				if (flag4)
				{
					typename = this.fullTypeName;
					resolvedNamespace = this.xmlNamespace;
				}
				else
				{
					string text = this.fullTypeName.Substring(this.xmlNamespace.resolvedNamespace.Length + 1);
					typename = text;
					resolvedNamespace = this.xmlNamespace;
				}
			}
			else
			{
				string text2 = this.fullTypeName.Substring(this.xmlNamespace.resolvedNamespace.Length + 1);
				typename = text2;
				resolvedNamespace = this.xmlNamespace;
			}
		}

		internal void SetVisualTreeAssetWithOutNotify(VisualTreeAsset vta)
		{
			this.m_VisualTreeAsset = vta;
		}

		internal void SetVisualTreeAsset(VisualTreeAsset vta)
		{
			VisualTreeAsset visualTreeAsset = this.visualTreeAsset;
			this.SetVisualTreeAssetWithOutNotify(vta);
			bool flag = visualTreeAsset != this.visualTreeAsset;
			if (flag)
			{
				this.OnVisualTreeAssetChanged(visualTreeAsset, this.visualTreeAsset);
			}
			bool flag2 = this.m_Children == null;
			if (!flag2)
			{
				foreach (UxmlAsset uxmlAsset in this.m_Children)
				{
					uxmlAsset.SetVisualTreeAsset(vta);
				}
			}
		}

		public void Add(UxmlAsset asset)
		{
			bool flag = asset == null;
			if (flag)
			{
				throw new ArgumentNullException("asset");
			}
			this.Insert(this.childCount, asset);
		}

		public void Insert(int index, UxmlAsset asset)
		{
			bool flag = asset == null;
			if (flag)
			{
				throw new ArgumentNullException("asset");
			}
			bool flag2 = index < 0 || index > this.childCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("Index out of range: " + index.ToString());
			}
			bool flag3 = asset == this;
			if (flag3)
			{
				throw new ArgumentException("Cannot insert element as its own child.");
			}
			bool flag4 = asset.IsAncestorOf(this);
			if (flag4)
			{
				throw new ArgumentException("Cannot insert element as a child because it is an ancestor.");
			}
			string text;
			bool flag5 = !this.Accepts(asset, out text);
			if (flag5)
			{
				throw new InvalidOperationException(text);
			}
			bool flag6 = asset.parentAsset == this;
			if (flag6)
			{
				int num = this.m_Children.IndexOf(asset);
				bool flag7 = num == index;
				if (!flag7)
				{
					bool flag8 = index == this.childCount;
					this.m_Children.RemoveAt(num);
					this.m_Children.Insert(flag8 ? this.childCount : index, asset);
				}
			}
			else
			{
				this.InsertInChildren(index, asset);
				asset.SetParent(this);
			}
		}

		public bool Remove(UxmlAsset asset)
		{
			bool flag = asset == null;
			if (flag)
			{
				throw new ArgumentNullException("asset");
			}
			bool flag2 = asset == this;
			if (flag2)
			{
				throw new ArgumentException("Cannot remove element from itself.");
			}
			bool flag3 = asset.m_Parent != this;
			bool flag4;
			if (flag3)
			{
				flag4 = false;
			}
			else
			{
				this.RemoveAt(this.m_Children.IndexOf(asset));
				flag4 = true;
			}
			return flag4;
		}

		public void RemoveAt(int index)
		{
			bool flag = index < 0 || index > this.childCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("Index out of range: " + index.ToString());
			}
			UxmlAsset uxmlAsset = this.m_Children[index];
			uxmlAsset.SetParent(null);
		}

		private void InsertInChildren(int index, UxmlAsset asset)
		{
			if (this.m_Children == null)
			{
				this.m_Children = new List<UxmlAsset>();
			}
			this.m_Children.Insert(index, asset);
		}

		private void RemoveFromChildren(UxmlAsset child)
		{
			this.RemoveFromChildren(this.IndexOf(child));
		}

		private void RemoveFromChildren(int index)
		{
			this.m_Children.RemoveAt(index);
		}

		private void SetParent(UxmlAsset parent)
		{
			UxmlAsset parent2 = this.m_Parent;
			if (parent2 != null)
			{
				parent2.RemoveFromChildren(this);
			}
			this.m_Parent = parent;
			this.SetVisualTreeAsset((parent != null) ? parent.visualTreeAsset : null);
		}

		private protected virtual void OnVisualTreeAssetChanged(VisualTreeAsset previousVta, VisualTreeAsset newVta)
		{
		}

		public int IndexOf(UxmlAsset asset)
		{
			return this.m_Children.IndexOf(asset);
		}

		public int SiblingIndex()
		{
			UxmlAsset parentAsset = this.parentAsset;
			return (parentAsset != null) ? parentAsset.IndexOf(this) : (-1);
		}

		public void RemoveFromHierarchy()
		{
			UxmlAsset parentAsset = this.parentAsset;
			if (parentAsset != null)
			{
				parentAsset.Remove(this);
			}
		}

		public bool IsAncestorOf(UxmlAsset other)
		{
			HashSet<UxmlAsset> hashSet;
			bool flag3;
			using (CollectionPool<HashSet<UxmlAsset>, UxmlAsset>.Get(out hashSet))
			{
				for (UxmlAsset uxmlAsset = other; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
				{
					bool flag = !hashSet.Add(uxmlAsset);
					if (flag)
					{
						throw new InvalidOperationException("Recursion Detected");
					}
					bool flag2 = this == uxmlAsset.parentAsset;
					if (flag2)
					{
						return true;
					}
				}
				flag3 = false;
			}
			return flag3;
		}

		public virtual bool HasParent()
		{
			return this.m_Parent != null;
		}

		public bool HasAttribute(string attributeName)
		{
			List<UxmlProperty> properties = this.m_Properties;
			bool flag = properties == null || properties.Count <= 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < this.m_Properties.Count; i++)
				{
					bool flag3 = string.CompareOrdinal(this.m_Properties[i].name, attributeName) == 0;
					if (flag3)
					{
						return true;
					}
				}
				flag2 = false;
			}
			return flag2;
		}

		public string GetAttributeValue(string attributeName)
		{
			string text;
			this.TryGetAttributeValue(attributeName, out text);
			return text;
		}

		public bool TryGetAttributeValue(string propertyName, out string value)
		{
			bool flag = this.m_Properties == null;
			bool flag2;
			if (flag)
			{
				value = null;
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < this.m_Properties.Count; i++)
				{
					UxmlProperty uxmlProperty = this.m_Properties[i];
					bool flag3 = string.CompareOrdinal(uxmlProperty.name, propertyName) == 0;
					if (flag3)
					{
						value = uxmlProperty.value;
						return true;
					}
				}
				value = null;
				flag2 = false;
			}
			return flag2;
		}

		public void AddUxmlNamespace(string prefix, string resolvedNamespace)
		{
			this.namespaceDefinitions.Add(new UxmlNamespaceDefinition
			{
				prefix = prefix,
				resolvedNamespace = resolvedNamespace
			});
		}

		public void SetAttribute(string name, string value)
		{
			this.SetOrAddProperty(name, value);
		}

		public void RemoveAttribute(string attributeName)
		{
			bool flag = this.m_Properties == null || this.m_Properties.Count <= 0;
			if (!flag)
			{
				for (int i = 0; i < this.m_Properties.Count; i++)
				{
					UxmlProperty uxmlProperty = this.m_Properties[i];
					bool flag2 = string.CompareOrdinal(uxmlProperty.name, attributeName) != 0;
					if (!flag2)
					{
						this.m_Properties.RemoveAt(i);
						break;
					}
				}
			}
		}

		private void SetOrAddProperty(string propertyName, string propertyValue)
		{
			if (this.m_Properties == null)
			{
				this.m_Properties = new List<UxmlProperty>();
			}
			for (int i = 0; i < this.m_Properties.Count; i++)
			{
				UxmlProperty uxmlProperty = this.m_Properties[i];
				bool flag = string.CompareOrdinal(uxmlProperty.name, propertyName) == 0;
				if (flag)
				{
					uxmlProperty.value = propertyValue;
					this.m_Properties[i] = uxmlProperty;
					return;
				}
			}
			this.m_Properties.Add(new UxmlProperty
			{
				name = propertyName,
				value = propertyValue
			});
		}

		internal abstract bool Accepts(UxmlAsset asset, out string errorMessage);

		public override string ToString()
		{
			return string.Format("{0}(id:{1})", this.fullTypeName, this.id);
		}

		public const string NullNodeType = "null";

		[SerializeField]
		private string m_FullTypeName;

		[SerializeField]
		private UxmlNamespaceDefinition m_XmlNamespace;

		[SerializeField]
		private int m_Id;

		[SerializeReference]
		[HideInInspector]
		private UxmlAsset m_Parent;

		[SerializeReference]
		private List<UxmlAsset> m_Children;

		[SerializeField]
		private VisualTreeAsset m_VisualTreeAsset;

		[SerializeField]
		private List<UxmlNamespaceDefinition> m_NamespaceDefinitions;

		[SerializeField]
		protected List<UxmlProperty> m_Properties;
	}
}
