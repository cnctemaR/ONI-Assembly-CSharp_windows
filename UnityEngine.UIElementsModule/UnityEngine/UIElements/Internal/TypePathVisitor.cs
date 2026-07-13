using System;
using System.Collections.Generic;
using Unity.Properties;

namespace UnityEngine.UIElements.Internal
{
	internal class TypePathVisitor : ITypeVisitor, IPropertyBagVisitor, IPropertyVisitor
	{
		public PropertyPath Path { get; set; }

		public Type resolvedType { get; private set; }

		public VisitReturnCode ReturnCode { get; internal set; }

		public int PathIndex
		{
			get
			{
				return this.m_PathIndex;
			}
		}

		public void Reset()
		{
			this.resolvedType = null;
			this.m_LastType = null;
			this.Path = default(PropertyPath);
			this.ReturnCode = VisitReturnCode.Ok;
			this.m_PathIndex = 0;
		}

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
					foreach (IProperty<TContainer> property2 in properties.GetProperties())
					{
						bool flag2 = property2.Name == propertyPathPart.Name;
						if (flag2)
						{
							Type type = (this.m_LastType = property2.DeclaredValueType());
							IPropertyBag propertyBag = PropertyBag.GetPropertyBag(type);
							if (propertyBag != null)
							{
								propertyBag.Accept(this);
							}
							return;
						}
					}
					this.ReturnCode = VisitReturnCode.InvalidPath;
				}
				return;
			}
			case PropertyPathPartKind.Index:
			{
				IIndexedProperties<TContainer> indexedProperties = properties as IIndexedProperties<TContainer>;
				IProperty<TContainer> property;
				bool flag3 = indexedProperties != null && indexedProperties.TryGetProperty(ref container, propertyPathPart.Index, out property);
				if (flag3)
				{
					property.Accept(this, ref container);
				}
				else
				{
					Type elementType = TypePathVisitor.GetElementType(typeof(TContainer));
					bool flag4 = elementType != null;
					if (flag4)
					{
						IPropertyBag propertyBag2 = PropertyBag.GetPropertyBag(elementType);
						if (propertyBag2 != null)
						{
							propertyBag2.Accept(this);
						}
					}
					else
					{
						this.ReturnCode = VisitReturnCode.InvalidPath;
					}
				}
				return;
			}
			}
			this.ReturnCode = VisitReturnCode.InvalidPath;
		}

		void IPropertyVisitor.Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
		{
			TValue value = property.GetValue(ref container);
			bool flag = this.m_PathIndex >= this.Path.Length;
			if (flag)
			{
				this.resolvedType = property.DeclaredValueType();
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
						IPropertyBag propertyBag2 = PropertyBag.GetPropertyBag(property.DeclaredValueType());
						if (propertyBag2 != null)
						{
							propertyBag2.Accept(this);
						}
					}
					else
					{
						PropertyContainer.Accept<TValue>(this, ref value, default(VisitParameters));
					}
				}
				else
				{
					this.ReturnCode = VisitReturnCode.InvalidPath;
				}
			}
		}

		void ITypeVisitor.Visit<TContainer>()
		{
			bool flag = this.IsLastPartReached();
			if (!flag)
			{
				PropertyPath propertyPath = this.Path;
				int num = this.m_PathIndex;
				this.m_PathIndex = num + 1;
				PropertyPathPart propertyPathPart = propertyPath[num];
				this.m_LastType = null;
				PropertyPathPartKind kind = propertyPathPart.Kind;
				PropertyPathPartKind propertyPathPartKind = kind;
				if (propertyPathPartKind != PropertyPathPartKind.Name)
				{
					if (propertyPathPartKind == PropertyPathPartKind.Index)
					{
						Type typeFromHandle = typeof(TContainer);
						Type elementType = TypePathVisitor.GetElementType(typeFromHandle);
						bool flag2 = elementType != null;
						if (flag2)
						{
							this.m_LastType = elementType;
							IPropertyBag propertyBag = PropertyBag.GetPropertyBag(elementType);
							bool flag3 = propertyBag != null;
							if (flag3)
							{
								propertyBag.Accept(this);
								return;
							}
						}
					}
				}
				else
				{
					IPropertyBag<TContainer> propertyBag2 = PropertyBag.GetPropertyBag<TContainer>();
					bool flag4 = propertyBag2 == null;
					if (flag4)
					{
						return;
					}
					foreach (IProperty<TContainer> property in propertyBag2.GetProperties())
					{
						bool flag5 = property.Name != propertyPathPart.Name;
						if (!flag5)
						{
							Type type = (this.m_LastType = property.DeclaredValueType());
							IPropertyBag propertyBag3 = PropertyBag.GetPropertyBag(type);
							bool flag6 = propertyBag3 != null;
							if (flag6)
							{
								propertyBag3.Accept(this);
								return;
							}
							Type elementType2 = TypePathVisitor.GetElementType(type);
							bool flag7 = elementType2 != null;
							if (flag7)
							{
								bool flag8 = this.IsLastPartReached();
								if (flag8)
								{
									return;
								}
								propertyPath = this.Path;
								num = this.m_PathIndex;
								this.m_PathIndex = num + 1;
								bool isIndex = propertyPath[num].IsIndex;
								if (isIndex)
								{
									this.m_LastType = elementType2;
									IPropertyBag propertyBag4 = PropertyBag.GetPropertyBag(elementType2);
									if (propertyBag4 != null)
									{
										propertyBag4.Accept(this);
									}
									return;
								}
							}
							break;
						}
					}
				}
				bool flag9 = this.IsLastPartReached();
				if (!flag9)
				{
					bool flag10 = this.ReturnCode == VisitReturnCode.Ok;
					if (flag10)
					{
						this.ReturnCode = VisitReturnCode.InvalidPath;
					}
				}
			}
		}

		private bool IsLastPartReached()
		{
			bool flag = this.m_PathIndex < this.Path.Length;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.m_LastType == null;
				if (flag3)
				{
					this.ReturnCode = VisitReturnCode.InvalidPath;
				}
				this.resolvedType = this.m_LastType;
				flag2 = true;
			}
			return flag2;
		}

		private static Type GetElementType(Type type)
		{
			Type type2 = null;
			bool flag = type.IsArray && type.GetArrayRank() == 1;
			if (flag)
			{
				type2 = type.GetElementType();
			}
			else
			{
				bool flag2 = type.IsGenericType && (type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)));
				if (flag2)
				{
					type2 = type.GenericTypeArguments[0];
				}
			}
			return type2;
		}

		private Type m_LastType;

		private int m_PathIndex;
	}
}
