using System;
using System.Collections.Generic;
using Unity.Properties;

namespace UnityEngine.UIElements.Internal
{
	internal class AutoCompletePathVisitor : ITypeVisitor, IPropertyVisitor, IPropertyBagVisitor, IListPropertyVisitor
	{
		public List<PropertyPathInfo> propertyPathList
		{
			set
			{
				this.m_VisitContext.propertyPathInfos = value;
			}
		}

		public int maxDepth { get; set; }

		private bool HasReachedEnd(Type containerType)
		{
			return this.m_VisitContext.currentDepth >= this.maxDepth || this.m_VisitContext.types.Contains(containerType);
		}

		public void Reset()
		{
			this.m_VisitContext.current = default(PropertyPath);
			this.m_VisitContext.propertyPathInfos = null;
			this.m_VisitContext.types.Clear();
			this.m_VisitContext.currentDepth = 0;
		}

		void ITypeVisitor.Visit<TContainer>()
		{
			bool flag = this.HasReachedEnd(typeof(TContainer));
			if (!flag)
			{
				using (new AutoCompletePathVisitor.InspectedTypeScope<TContainer>(this.m_VisitContext))
				{
					IPropertyBag<TContainer> propertyBag = PropertyBag.GetPropertyBag<TContainer>();
					bool flag2 = propertyBag == null;
					if (!flag2)
					{
						foreach (IProperty<TContainer> property in propertyBag.GetProperties())
						{
							using (new AutoCompletePathVisitor.VisitedPropertyScope(this.m_VisitContext, property))
							{
								this.VisitPropertyType(property.DeclaredValueType());
							}
						}
					}
				}
			}
		}

		void IPropertyBagVisitor.Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
		{
			bool flag = this.HasReachedEnd(typeof(TContainer));
			if (!flag)
			{
				using (new AutoCompletePathVisitor.InspectedTypeScope<TContainer>(this.m_VisitContext))
				{
					IIndexedProperties<TContainer> indexedProperties = properties as IIndexedProperties<TContainer>;
					if (indexedProperties == null)
					{
						if (!(properties is IKeyedProperties<TContainer, object>))
						{
							foreach (IProperty<TContainer> property in properties.GetProperties(ref container))
							{
								using (new AutoCompletePathVisitor.VisitedPropertyScope(this.m_VisitContext, property))
								{
									property.Accept(this, ref container);
								}
							}
						}
					}
					else
					{
						IProperty<TContainer> property2;
						bool flag2 = indexedProperties.TryGetProperty(ref container, 0, out property2);
						if (flag2)
						{
							using (new AutoCompletePathVisitor.VisitedPropertyScope(this.m_VisitContext, 0, property2.DeclaredValueType()))
							{
								property2.Accept(this, ref container);
							}
						}
						else
						{
							this.VisitPropertyType(typeof(TContainer));
						}
					}
				}
			}
		}

		void IPropertyVisitor.Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
		{
			bool flag = !TypeTraits.IsContainer(typeof(TValue));
			if (!flag)
			{
				TValue value = property.GetValue(ref container);
				IPropertyBag propertyBag;
				bool flag2 = (!TypeTraits<TValue>.CanBeNull || !EqualityComparer<TValue>.Default.Equals(value, default(TValue))) && PropertyBag.TryGetPropertyBagForValue<TValue>(ref value, out propertyBag);
				if (flag2)
				{
					IPropertyBag propertyBag2 = propertyBag;
					IPropertyBag propertyBag3 = propertyBag2;
					IListPropertyAccept<TValue> listPropertyAccept = propertyBag3 as IListPropertyAccept<TValue>;
					if (listPropertyAccept == null)
					{
						PropertyContainer.TryAccept<TValue>(this, ref value, default(VisitParameters));
					}
					else
					{
						listPropertyAccept.Accept<TContainer>(this, property, ref container, ref value);
					}
				}
				else
				{
					this.VisitPropertyType(property.DeclaredValueType());
				}
			}
		}

		void IListPropertyVisitor.Visit<TContainer, TList, TElement>(Property<TContainer, TList> property, ref TContainer container, ref TList list)
		{
			PropertyContainer.TryAccept<TList>(this, ref list, default(VisitParameters));
		}

		private void VisitPropertyType(Type type)
		{
			bool flag = this.HasReachedEnd(type);
			if (!flag)
			{
				bool isArray = type.IsArray;
				if (isArray)
				{
					bool flag2 = type.GetArrayRank() != 1;
					if (!flag2)
					{
						Type elementType = type.GetElementType();
						IPropertyBag propertyBag = PropertyBag.GetPropertyBag(elementType);
						using (new AutoCompletePathVisitor.VisitedPropertyScope(this.m_VisitContext, 0, elementType))
						{
							if (propertyBag != null)
							{
								propertyBag.Accept(this);
							}
						}
					}
				}
				else
				{
					bool isGenericType = type.IsGenericType;
					if (isGenericType)
					{
						bool flag3 = type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>));
						if (flag3)
						{
							Type type2 = type.GenericTypeArguments[0];
							IPropertyBag propertyBag2 = PropertyBag.GetPropertyBag(type2);
							using (new AutoCompletePathVisitor.VisitedPropertyScope(this.m_VisitContext, 0, type2))
							{
								if (propertyBag2 != null)
								{
									propertyBag2.Accept(this);
								}
							}
						}
					}
					else
					{
						IPropertyBag propertyBag3 = PropertyBag.GetPropertyBag(type);
						if (propertyBag3 != null)
						{
							propertyBag3.Accept(this);
						}
					}
				}
			}
		}

		private AutoCompletePathVisitor.VisitContext m_VisitContext = new AutoCompletePathVisitor.VisitContext();

		private class VisitContext
		{
			public List<PropertyPathInfo> propertyPathInfos { get; set; }

			public HashSet<Type> types { get; } = new HashSet<Type>();

			public PropertyPath current { get; set; }

			public int currentDepth { get; set; }
		}

		private struct InspectedTypeScope<TContainer> : IDisposable
		{
			public InspectedTypeScope(AutoCompletePathVisitor.VisitContext context)
			{
				this.m_VisitContext = context;
				this.m_VisitContext.types.Add(typeof(TContainer));
			}

			public void Dispose()
			{
				this.m_VisitContext.types.Remove(typeof(TContainer));
			}

			private AutoCompletePathVisitor.VisitContext m_VisitContext;
		}

		private struct VisitedPropertyScope : IDisposable
		{
			public VisitedPropertyScope(AutoCompletePathVisitor.VisitContext context, IProperty property)
			{
				this.m_VisitContext = context;
				AutoCompletePathVisitor.VisitContext visitContext = this.m_VisitContext;
				PropertyPath propertyPath = this.m_VisitContext.current;
				visitContext.current = PropertyPath.AppendProperty(in propertyPath, property);
				propertyPath = this.m_VisitContext.current;
				PropertyPathInfo propertyPathInfo = new PropertyPathInfo(in propertyPath, property.DeclaredValueType());
				List<PropertyPathInfo> propertyPathInfos = this.m_VisitContext.propertyPathInfos;
				if (propertyPathInfos != null)
				{
					propertyPathInfos.Add(propertyPathInfo);
				}
				AutoCompletePathVisitor.VisitContext visitContext2 = this.m_VisitContext;
				int currentDepth = visitContext2.currentDepth;
				visitContext2.currentDepth = currentDepth + 1;
			}

			public VisitedPropertyScope(AutoCompletePathVisitor.VisitContext context, int index, Type type)
			{
				this.m_VisitContext = context;
				AutoCompletePathVisitor.VisitContext visitContext = this.m_VisitContext;
				PropertyPath propertyPath = this.m_VisitContext.current;
				visitContext.current = PropertyPath.AppendIndex(in propertyPath, index);
				propertyPath = this.m_VisitContext.current;
				PropertyPathInfo propertyPathInfo = new PropertyPathInfo(in propertyPath, type);
				List<PropertyPathInfo> propertyPathInfos = this.m_VisitContext.propertyPathInfos;
				if (propertyPathInfos != null)
				{
					propertyPathInfos.Add(propertyPathInfo);
				}
				AutoCompletePathVisitor.VisitContext visitContext2 = this.m_VisitContext;
				int currentDepth = visitContext2.currentDepth;
				visitContext2.currentDepth = currentDepth + 1;
			}

			public void Dispose()
			{
				AutoCompletePathVisitor.VisitContext visitContext = this.m_VisitContext;
				PropertyPath current = this.m_VisitContext.current;
				visitContext.current = PropertyPath.Pop(in current);
				AutoCompletePathVisitor.VisitContext visitContext2 = this.m_VisitContext;
				int currentDepth = visitContext2.currentDepth;
				visitContext2.currentDepth = currentDepth - 1;
			}

			private AutoCompletePathVisitor.VisitContext m_VisitContext;
		}
	}
}
