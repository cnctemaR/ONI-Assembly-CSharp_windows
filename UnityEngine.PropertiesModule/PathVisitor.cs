using System;
using System.Collections.Generic;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	public abstract class PathVisitor : IPropertyBagVisitor, IPropertyVisitor
	{
		public PropertyPath Path { get; set; }

		public virtual void Reset()
		{
			this.m_PathIndex = 0;
			this.Path = default(PropertyPath);
			this.ReturnCode = VisitReturnCode.Ok;
			this.ReadonlyVisit = false;
		}

		private IProperty Property { get; set; }

		public bool ReadonlyVisit { get; set; }

		public VisitReturnCode ReturnCode { get; protected set; }

		void IPropertyBagVisitor.Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
		{
			PropertyPath path = this.Path;
			int pathIndex = this.m_PathIndex;
			this.m_PathIndex = pathIndex + 1;
			PropertyPathPart propertyPathPart = path[pathIndex];
			switch (propertyPathPart.Kind)
			{
			case PropertyPathPartKind.Name:
			{
				INamedProperties<TContainer> namedProperties = properties as INamedProperties<TContainer>;
				IProperty<TContainer> property;
				bool flag = namedProperties != null && namedProperties.TryGetProperty(ref container, propertyPathPart.Name, out property);
				if (flag)
				{
					property.Accept(this, ref container);
				}
				else
				{
					this.ReturnCode = VisitReturnCode.InvalidPath;
				}
				break;
			}
			case PropertyPathPartKind.Index:
			{
				IIndexedProperties<TContainer> indexedProperties = properties as IIndexedProperties<TContainer>;
				IProperty<TContainer> property;
				bool flag2 = indexedProperties != null && indexedProperties.TryGetProperty(ref container, propertyPathPart.Index, out property);
				if (flag2)
				{
					using ((property as IAttributes).CreateAttributesScope(this.Property as IAttributes))
					{
						property.Accept(this, ref container);
					}
				}
				else
				{
					this.ReturnCode = VisitReturnCode.InvalidPath;
				}
				break;
			}
			case PropertyPathPartKind.Key:
			{
				IKeyedProperties<TContainer, object> keyedProperties = properties as IKeyedProperties<TContainer, object>;
				IProperty<TContainer> property;
				bool flag3 = keyedProperties != null && keyedProperties.TryGetProperty(ref container, propertyPathPart.Key, out property);
				if (flag3)
				{
					using ((property as IAttributes).CreateAttributesScope(this.Property as IAttributes))
					{
						property.Accept(this, ref container);
					}
				}
				else
				{
					this.ReturnCode = VisitReturnCode.InvalidPath;
				}
				break;
			}
			default:
				this.ReturnCode = VisitReturnCode.InvalidPath;
				break;
			}
		}

		void IPropertyVisitor.Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
		{
			TValue value = property.GetValue(ref container);
			bool flag = this.m_PathIndex >= this.Path.Length;
			if (flag)
			{
				this.VisitPath<TContainer, TValue>(property, ref container, ref value);
			}
			else
			{
				IPropertyBag propertyBag;
				bool flag2 = PropertyBag.TryGetPropertyBagForValue<TValue>(ref value, out propertyBag);
				if (flag2)
				{
					bool flag3 = TypeTraits<TValue>.CanBeNull && EqualityComparer<TValue>.Default.Equals(value, default(TValue));
					if (flag3)
					{
						this.ReturnCode = VisitReturnCode.InvalidPath;
					}
					else
					{
						using (new PathVisitor.PropertyScope(this, property))
						{
							PropertyContainer.Accept<TValue>(this, ref value, default(VisitParameters));
						}
						bool flag4 = !property.IsReadOnly && !this.ReadonlyVisit;
						if (flag4)
						{
							property.SetValue(ref container, value);
						}
					}
				}
				else
				{
					this.ReturnCode = VisitReturnCode.InvalidPath;
				}
			}
		}

		protected virtual void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
		{
		}

		private int m_PathIndex;

		private readonly struct PropertyScope : IDisposable
		{
			public PropertyScope(PathVisitor visitor, IProperty property)
			{
				this.m_Visitor = visitor;
				this.m_Property = this.m_Visitor.Property;
				this.m_Visitor.Property = property;
			}

			public void Dispose()
			{
				this.m_Visitor.Property = this.m_Property;
			}

			private readonly PathVisitor m_Visitor;

			private readonly IProperty m_Property;
		}
	}
}
