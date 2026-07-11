using System;

namespace UnityEngine.Experimental.UIElements
{
	public class VisualContainer : VisualElement
	{
		[Obsolete("VisualContainer.AddChild will be removed. Use VisualElement.Add or VisualElement.shadow.Add instead", false)]
		public void AddChild(VisualElement child)
		{
			base.shadow.Add(child);
		}

		[Obsolete("VisualContainer.InsertChild will be removed. Use VisualElement.Insert or VisualElement.shadow.Insert instead", false)]
		public void InsertChild(int index, VisualElement child)
		{
			base.shadow.Insert(index, child);
		}

		[Obsolete("VisualContainer.RemoveChild will be removed. Use VisualElement.Remove or VisualElement.shadow.Remove instead", false)]
		public void RemoveChild(VisualElement child)
		{
			base.shadow.Remove(child);
		}

		[Obsolete("VisualContainer.RemoveChildAt will be removed. Use VisualElement.RemoveAt or VisualElement.shadow.RemoveAt instead", false)]
		public void RemoveChildAt(int index)
		{
			base.shadow.RemoveAt(index);
		}

		[Obsolete("VisualContainer.ClearChildren will be removed. Use VisualElement.Clear or VisualElement.shadow.Clear instead", false)]
		public void ClearChildren()
		{
			base.shadow.Clear();
		}

		[Obsolete("VisualContainer.GetChildAt will be removed. Use VisualElement.ElementAt or VisualElement.shadow.ElementAt instead", false)]
		public VisualElement GetChildAt(int index)
		{
			return base.shadow[index];
		}

		public new class UxmlFactory : VisualElement.UxmlFactory
		{
			public override string uxmlName
			{
				get
				{
					return typeof(VisualContainer).Name;
				}
			}

			public override string uxmlNamespace
			{
				get
				{
					return typeof(VisualContainer).Namespace;
				}
			}

			public override string uxmlQualifiedName
			{
				get
				{
					return typeof(VisualContainer).FullName;
				}
			}

			public override string substituteForTypeName
			{
				get
				{
					return typeof(VisualElement).Name;
				}
			}

			public override string substituteForTypeNamespace
			{
				get
				{
					return typeof(VisualElement).Namespace;
				}
			}

			public override string substituteForTypeQualifiedName
			{
				get
				{
					return typeof(VisualElement).FullName;
				}
			}
		}
	}
}
