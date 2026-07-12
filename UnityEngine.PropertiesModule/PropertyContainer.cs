using System;
using System.Collections.Generic;
using Unity.Properties.Internal;
using UnityEngine.Pool;

namespace Unity.Properties
{
	public static class PropertyContainer
	{
		public static void Accept<TContainer>(IPropertyBagVisitor visitor, TContainer container, VisitParameters parameters = default(VisitParameters))
		{
			VisitReturnCode visitReturnCode = VisitReturnCode.Ok;
			try
			{
				bool flag = PropertyContainer.TryAccept<TContainer>(visitor, ref container, out visitReturnCode, parameters);
				if (flag)
				{
					return;
				}
			}
			catch (Exception)
			{
				bool flag2 = (parameters.IgnoreExceptions & VisitExceptionKind.Visitor) == VisitExceptionKind.None;
				if (flag2)
				{
					throw;
				}
			}
			bool flag3 = (parameters.IgnoreExceptions & VisitExceptionKind.Internal) > VisitExceptionKind.None;
			if (!flag3)
			{
				switch (visitReturnCode)
				{
				case VisitReturnCode.Ok:
				case VisitReturnCode.InvalidContainerType:
					break;
				case VisitReturnCode.NullContainer:
					throw new ArgumentException("The given container was null. Visitation only works for valid non-null containers.");
				case VisitReturnCode.MissingPropertyBag:
					throw new MissingPropertyBagException(container.GetType());
				default:
					throw new Exception(string.Format("Unexpected {0}=[{1}]", "VisitReturnCode", visitReturnCode));
				}
			}
		}

		public static void Accept<TContainer>(IPropertyBagVisitor visitor, ref TContainer container, VisitParameters parameters = default(VisitParameters))
		{
			VisitReturnCode visitReturnCode = VisitReturnCode.Ok;
			try
			{
				bool flag = PropertyContainer.TryAccept<TContainer>(visitor, ref container, out visitReturnCode, parameters);
				if (flag)
				{
					return;
				}
			}
			catch (Exception)
			{
				bool flag2 = (parameters.IgnoreExceptions & VisitExceptionKind.Visitor) == VisitExceptionKind.None;
				if (flag2)
				{
					throw;
				}
			}
			bool flag3 = (parameters.IgnoreExceptions & VisitExceptionKind.Internal) > VisitExceptionKind.None;
			if (!flag3)
			{
				switch (visitReturnCode)
				{
				case VisitReturnCode.Ok:
				case VisitReturnCode.InvalidContainerType:
					break;
				case VisitReturnCode.NullContainer:
					throw new ArgumentException("The given container was null. Visitation only works for valid non-null containers.");
				case VisitReturnCode.MissingPropertyBag:
					throw new MissingPropertyBagException(container.GetType());
				default:
					throw new Exception(string.Format("Unexpected {0}=[{1}]", "VisitReturnCode", visitReturnCode));
				}
			}
		}

		public static bool TryAccept<TContainer>(IPropertyBagVisitor visitor, ref TContainer container, VisitParameters parameters = default(VisitParameters))
		{
			VisitReturnCode visitReturnCode;
			return PropertyContainer.TryAccept<TContainer>(visitor, ref container, out visitReturnCode, parameters);
		}

		public static bool TryAccept<TContainer>(IPropertyBagVisitor visitor, ref TContainer container, out VisitReturnCode returnCode, VisitParameters parameters = default(VisitParameters))
		{
			bool flag = !TypeTraits<TContainer>.IsContainer;
			bool flag2;
			if (flag)
			{
				returnCode = VisitReturnCode.InvalidContainerType;
				flag2 = false;
			}
			else
			{
				bool canBeNull = TypeTraits<TContainer>.CanBeNull;
				if (canBeNull)
				{
					bool flag3 = EqualityComparer<TContainer>.Default.Equals(container, default(TContainer));
					if (flag3)
					{
						returnCode = VisitReturnCode.NullContainer;
						return false;
					}
				}
				bool flag4 = !TypeTraits<TContainer>.IsValueType && typeof(TContainer) != container.GetType();
				if (flag4)
				{
					bool flag5 = !TypeTraits.IsContainer(container.GetType());
					if (flag5)
					{
						returnCode = VisitReturnCode.InvalidContainerType;
						return false;
					}
					IPropertyBag propertyBag = PropertyBagStore.GetPropertyBag(container.GetType());
					bool flag6 = propertyBag == null;
					if (flag6)
					{
						returnCode = VisitReturnCode.MissingPropertyBag;
						return false;
					}
					object obj = container;
					propertyBag.Accept(visitor, ref obj);
					container = (TContainer)((object)obj);
				}
				else
				{
					IPropertyBag<TContainer> propertyBag2 = PropertyBagStore.GetPropertyBag<TContainer>();
					bool flag7 = propertyBag2 == null;
					if (flag7)
					{
						returnCode = VisitReturnCode.MissingPropertyBag;
						return false;
					}
					PropertyBag.AcceptWithSpecializedVisitor<TContainer>(propertyBag2, visitor, ref container);
				}
				returnCode = VisitReturnCode.Ok;
				flag2 = true;
			}
			return flag2;
		}

		public static void Accept<TContainer>(IPropertyVisitor visitor, ref TContainer container, in PropertyPath path, VisitParameters parameters = default(VisitParameters))
		{
			PropertyContainer.ValueAtPathVisitor valueAtPathVisitor = PropertyContainer.ValueAtPathVisitor.Pool.Get();
			try
			{
				valueAtPathVisitor.Path = path;
				valueAtPathVisitor.Visitor = visitor;
				PropertyContainer.Accept<TContainer>(valueAtPathVisitor, ref container, parameters);
				bool flag = (parameters.IgnoreExceptions & VisitExceptionKind.Internal) == VisitExceptionKind.None;
				if (flag)
				{
					VisitReturnCode returnCode = valueAtPathVisitor.ReturnCode;
					VisitReturnCode visitReturnCode = returnCode;
					if (visitReturnCode != VisitReturnCode.Ok)
					{
						if (visitReturnCode != VisitReturnCode.InvalidPath)
						{
							throw new Exception(string.Format("Unexpected {0}=[{1}]", "VisitReturnCode", valueAtPathVisitor.ReturnCode));
						}
						throw new InvalidPathException(string.Format("Failed to Visit at Path=[{0}]", path));
					}
				}
			}
			finally
			{
				PropertyContainer.ValueAtPathVisitor.Pool.Release(valueAtPathVisitor);
			}
		}

		public static bool TryAccept<TContainer>(IPropertyVisitor visitor, ref TContainer container, in PropertyPath path, out VisitReturnCode returnCode, VisitParameters parameters = default(VisitParameters))
		{
			PropertyContainer.ValueAtPathVisitor valueAtPathVisitor = PropertyContainer.ValueAtPathVisitor.Pool.Get();
			bool flag;
			try
			{
				valueAtPathVisitor.Path = path;
				valueAtPathVisitor.Visitor = visitor;
				flag = PropertyContainer.TryAccept<TContainer>(valueAtPathVisitor, ref container, out returnCode, parameters);
			}
			finally
			{
				PropertyContainer.ValueAtPathVisitor.Pool.Release(valueAtPathVisitor);
			}
			return flag;
		}

		public static IProperty GetProperty<TContainer>(TContainer container, in PropertyPath path)
		{
			return PropertyContainer.GetProperty<TContainer>(ref container, in path);
		}

		public static IProperty GetProperty<TContainer>(ref TContainer container, in PropertyPath path)
		{
			IProperty property;
			VisitReturnCode visitReturnCode;
			bool flag = PropertyContainer.TryGetProperty<TContainer>(ref container, in path, out property, out visitReturnCode);
			if (flag)
			{
				return property;
			}
			switch (visitReturnCode)
			{
			case VisitReturnCode.NullContainer:
				throw new ArgumentNullException("container");
			case VisitReturnCode.InvalidContainerType:
				throw new InvalidContainerTypeException(container.GetType());
			case VisitReturnCode.MissingPropertyBag:
				throw new MissingPropertyBagException(container.GetType());
			case VisitReturnCode.InvalidPath:
				throw new ArgumentException(string.Format("Failed to get property for path=[{0}]", path));
			default:
				throw new Exception(string.Format("Unexpected {0}=[{1}]", "VisitReturnCode", visitReturnCode));
			}
		}

		public static bool TryGetProperty<TContainer>(TContainer container, in PropertyPath path, out IProperty property)
		{
			VisitReturnCode visitReturnCode;
			return PropertyContainer.TryGetProperty<TContainer>(ref container, in path, out property, out visitReturnCode);
		}

		public static bool TryGetProperty<TContainer>(ref TContainer container, in PropertyPath path, out IProperty property)
		{
			VisitReturnCode visitReturnCode;
			return PropertyContainer.TryGetProperty<TContainer>(ref container, in path, out property, out visitReturnCode);
		}

		public static bool TryGetProperty<TContainer>(ref TContainer container, in PropertyPath path, out IProperty property, out VisitReturnCode returnCode)
		{
			PropertyContainer.GetPropertyVisitor getPropertyVisitor = PropertyContainer.GetPropertyVisitor.Pool.Get();
			bool flag2;
			try
			{
				getPropertyVisitor.Path = path;
				bool flag = !PropertyContainer.TryAccept<TContainer>(getPropertyVisitor, ref container, out returnCode, default(VisitParameters));
				if (flag)
				{
					property = null;
					flag2 = false;
				}
				else
				{
					returnCode = getPropertyVisitor.ReturnCode;
					property = getPropertyVisitor.Property;
					flag2 = returnCode == VisitReturnCode.Ok;
				}
			}
			finally
			{
				PropertyContainer.GetPropertyVisitor.Pool.Release(getPropertyVisitor);
			}
			return flag2;
		}

		public static TValue GetValue<TContainer, TValue>(TContainer container, string name)
		{
			return PropertyContainer.GetValue<TContainer, TValue>(ref container, name);
		}

		public static TValue GetValue<TContainer, TValue>(ref TContainer container, string name)
		{
			PropertyPath propertyPath = new PropertyPath(name);
			return PropertyContainer.GetValue<TContainer, TValue>(ref container, in propertyPath);
		}

		public static TValue GetValue<TContainer, TValue>(TContainer container, in PropertyPath path)
		{
			return PropertyContainer.GetValue<TContainer, TValue>(ref container, in path);
		}

		public static TValue GetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path)
		{
			bool isEmpty = path.IsEmpty;
			if (isEmpty)
			{
				throw new InvalidPathException("The specified PropertyPath is empty.");
			}
			TValue tvalue;
			VisitReturnCode visitReturnCode;
			bool flag = PropertyContainer.TryGetValue<TContainer, TValue>(ref container, in path, out tvalue, out visitReturnCode);
			if (flag)
			{
				return tvalue;
			}
			switch (visitReturnCode)
			{
			case VisitReturnCode.NullContainer:
				throw new ArgumentNullException("container");
			case VisitReturnCode.InvalidContainerType:
				throw new InvalidContainerTypeException(container.GetType());
			case VisitReturnCode.MissingPropertyBag:
				throw new MissingPropertyBagException(container.GetType());
			case VisitReturnCode.InvalidPath:
				throw new InvalidPathException(string.Format("Failed to GetValue for property with Path=[{0}]", path));
			case VisitReturnCode.InvalidCast:
				throw new InvalidCastException(string.Format("Failed to GetValue of Type=[{0}] for property with path=[{1}]", typeof(TValue).Name, path));
			default:
				throw new Exception(string.Format("Unexpected {0}=[{1}]", "VisitReturnCode", visitReturnCode));
			}
		}

		public static bool TryGetValue<TContainer, TValue>(TContainer container, string name, out TValue value)
		{
			return PropertyContainer.TryGetValue<TContainer, TValue>(ref container, name, out value);
		}

		public static bool TryGetValue<TContainer, TValue>(ref TContainer container, string name, out TValue value)
		{
			PropertyPath propertyPath = new PropertyPath(name);
			VisitReturnCode visitReturnCode;
			return PropertyContainer.TryGetValue<TContainer, TValue>(ref container, in propertyPath, out value, out visitReturnCode);
		}

		public static bool TryGetValue<TContainer, TValue>(TContainer container, in PropertyPath path, out TValue value)
		{
			VisitReturnCode visitReturnCode;
			return PropertyContainer.TryGetValue<TContainer, TValue>(ref container, in path, out value, out visitReturnCode);
		}

		public static bool TryGetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, out TValue value)
		{
			VisitReturnCode visitReturnCode;
			return PropertyContainer.TryGetValue<TContainer, TValue>(ref container, in path, out value, out visitReturnCode);
		}

		public static bool TryGetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, out TValue value, out VisitReturnCode returnCode)
		{
			bool isEmpty = path.IsEmpty;
			bool flag;
			if (isEmpty)
			{
				returnCode = VisitReturnCode.InvalidPath;
				value = default(TValue);
				flag = false;
			}
			else
			{
				PropertyContainer.GetValueVisitor<TValue> getValueVisitor = PropertyContainer.GetValueVisitor<TValue>.Pool.Get();
				getValueVisitor.Path = path;
				getValueVisitor.ReadonlyVisit = true;
				try
				{
					bool flag2 = !PropertyContainer.TryAccept<TContainer>(getValueVisitor, ref container, out returnCode, default(VisitParameters));
					if (flag2)
					{
						value = default(TValue);
						return false;
					}
					value = getValueVisitor.Value;
					returnCode = getValueVisitor.ReturnCode;
				}
				finally
				{
					PropertyContainer.GetValueVisitor<TValue>.Pool.Release(getValueVisitor);
				}
				flag = returnCode == VisitReturnCode.Ok;
			}
			return flag;
		}

		public static bool IsPathValid<TContainer>(TContainer container, string path)
		{
			PropertyPath propertyPath = new PropertyPath(path);
			return PropertyContainer.IsPathValid<TContainer>(ref container, in propertyPath);
		}

		public static bool IsPathValid<TContainer>(TContainer container, in PropertyPath path)
		{
			return PropertyContainer.IsPathValid<TContainer>(ref container, in path);
		}

		public static bool IsPathValid<TContainer>(ref TContainer container, string path)
		{
			PropertyContainer.ExistsAtPathVisitor existsAtPathVisitor = PropertyContainer.ExistsAtPathVisitor.Pool.Get();
			bool exists;
			try
			{
				existsAtPathVisitor.Path = new PropertyPath(path);
				PropertyContainer.TryAccept<TContainer>(existsAtPathVisitor, ref container, default(VisitParameters));
				exists = existsAtPathVisitor.Exists;
			}
			finally
			{
				PropertyContainer.ExistsAtPathVisitor.Pool.Release(existsAtPathVisitor);
			}
			return exists;
		}

		public static bool IsPathValid<TContainer>(ref TContainer container, in PropertyPath path)
		{
			PropertyContainer.ExistsAtPathVisitor existsAtPathVisitor = PropertyContainer.ExistsAtPathVisitor.Pool.Get();
			bool exists;
			try
			{
				existsAtPathVisitor.Path = path;
				PropertyContainer.TryAccept<TContainer>(existsAtPathVisitor, ref container, default(VisitParameters));
				exists = existsAtPathVisitor.Exists;
			}
			finally
			{
				PropertyContainer.ExistsAtPathVisitor.Pool.Release(existsAtPathVisitor);
			}
			return exists;
		}

		public static void SetValue<TContainer, TValue>(TContainer container, string name, TValue value)
		{
			PropertyContainer.SetValue<TContainer, TValue>(ref container, name, value);
		}

		public static void SetValue<TContainer, TValue>(ref TContainer container, string name, TValue value)
		{
			PropertyPath propertyPath = new PropertyPath(name);
			PropertyContainer.SetValue<TContainer, TValue>(ref container, in propertyPath, value);
		}

		public static void SetValue<TContainer, TValue>(TContainer container, in PropertyPath path, TValue value)
		{
			PropertyContainer.SetValue<TContainer, TValue>(ref container, in path, value);
		}

		public static void SetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value)
		{
			bool flag = path.Length == 0;
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = path.Length <= 0;
			if (flag2)
			{
				throw new InvalidPathException("The specified PropertyPath is empty.");
			}
			VisitReturnCode visitReturnCode;
			bool flag3 = PropertyContainer.TrySetValue<TContainer, TValue>(ref container, in path, value, out visitReturnCode);
			if (flag3)
			{
				return;
			}
			switch (visitReturnCode)
			{
			case VisitReturnCode.NullContainer:
				throw new ArgumentNullException("container");
			case VisitReturnCode.InvalidContainerType:
				throw new InvalidContainerTypeException(container.GetType());
			case VisitReturnCode.MissingPropertyBag:
				throw new MissingPropertyBagException(container.GetType());
			case VisitReturnCode.InvalidPath:
				throw new InvalidPathException(string.Format("Failed to SetValue for property with Path=[{0}]", path));
			case VisitReturnCode.InvalidCast:
				throw new InvalidCastException(string.Format("Failed to SetValue of Type=[{0}] for property with path=[{1}]", typeof(TValue).Name, path));
			case VisitReturnCode.AccessViolation:
				throw new AccessViolationException(string.Format("Failed to SetValue for read-only property with Path=[{0}]", path));
			default:
				throw new Exception(string.Format("Unexpected {0}=[{1}]", "VisitReturnCode", visitReturnCode));
			}
		}

		public static bool TrySetValue<TContainer, TValue>(TContainer container, string name, TValue value)
		{
			return PropertyContainer.TrySetValue<TContainer, TValue>(ref container, name, value);
		}

		public static bool TrySetValue<TContainer, TValue>(ref TContainer container, string name, TValue value)
		{
			PropertyPath propertyPath = new PropertyPath(name);
			return PropertyContainer.TrySetValue<TContainer, TValue>(ref container, in propertyPath, value);
		}

		public static bool TrySetValue<TContainer, TValue>(TContainer container, in PropertyPath path, TValue value)
		{
			return PropertyContainer.TrySetValue<TContainer, TValue>(ref container, in path, value);
		}

		public static bool TrySetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value)
		{
			VisitReturnCode visitReturnCode;
			return PropertyContainer.TrySetValue<TContainer, TValue>(ref container, in path, value, out visitReturnCode);
		}

		public static bool TrySetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value, out VisitReturnCode returnCode)
		{
			bool isEmpty = path.IsEmpty;
			bool flag;
			if (isEmpty)
			{
				returnCode = VisitReturnCode.InvalidPath;
				flag = false;
			}
			else
			{
				PropertyContainer.SetValueVisitor<TValue> setValueVisitor = PropertyContainer.SetValueVisitor<TValue>.Pool.Get();
				setValueVisitor.Path = path;
				setValueVisitor.Value = value;
				try
				{
					bool flag2 = !PropertyContainer.TryAccept<TContainer>(setValueVisitor, ref container, out returnCode, default(VisitParameters));
					if (flag2)
					{
						return false;
					}
					returnCode = setValueVisitor.ReturnCode;
				}
				finally
				{
					PropertyContainer.SetValueVisitor<TValue>.Pool.Release(setValueVisitor);
				}
				flag = returnCode == VisitReturnCode.Ok;
			}
			return flag;
		}

		private class GetPropertyVisitor : PathVisitor
		{
			public override void Reset()
			{
				base.Reset();
				this.Property = null;
				base.ReadonlyVisit = true;
			}

			protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
			{
				this.Property = property;
			}

			public static readonly ObjectPool<PropertyContainer.GetPropertyVisitor> Pool = new ObjectPool<PropertyContainer.GetPropertyVisitor>(() => new PropertyContainer.GetPropertyVisitor(), null, delegate(PropertyContainer.GetPropertyVisitor v)
			{
				v.Reset();
			}, null, true, 10, 10000);

			public IProperty Property;
		}

		private class GetValueVisitor<TSrcValue> : PathVisitor
		{
			public override void Reset()
			{
				base.Reset();
				this.Value = default(TSrcValue);
				base.ReadonlyVisit = true;
			}

			protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
			{
				bool flag = !TypeConversion.TryConvert<TValue, TSrcValue>(ref value, out this.Value);
				if (flag)
				{
					base.ReturnCode = VisitReturnCode.InvalidCast;
				}
			}

			public static readonly ObjectPool<PropertyContainer.GetValueVisitor<TSrcValue>> Pool = new ObjectPool<PropertyContainer.GetValueVisitor<TSrcValue>>(() => new PropertyContainer.GetValueVisitor<TSrcValue>(), null, delegate(PropertyContainer.GetValueVisitor<TSrcValue> v)
			{
				v.Reset();
			}, null, true, 10, 10000);

			public TSrcValue Value;
		}

		private class ValueAtPathVisitor : PathVisitor
		{
			public override void Reset()
			{
				base.Reset();
				this.Visitor = null;
				base.ReadonlyVisit = true;
			}

			protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
			{
				((IPropertyAccept<TContainer>)property).Accept(this.Visitor, ref container);
			}

			public static readonly ObjectPool<PropertyContainer.ValueAtPathVisitor> Pool = new ObjectPool<PropertyContainer.ValueAtPathVisitor>(() => new PropertyContainer.ValueAtPathVisitor(), null, delegate(PropertyContainer.ValueAtPathVisitor v)
			{
				v.Reset();
			}, null, true, 10, 10000);

			public IPropertyVisitor Visitor;
		}

		private class ExistsAtPathVisitor : PathVisitor
		{
			public override void Reset()
			{
				base.Reset();
				this.Exists = false;
				base.ReadonlyVisit = true;
			}

			protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
			{
				this.Exists = true;
			}

			public static readonly ObjectPool<PropertyContainer.ExistsAtPathVisitor> Pool = new ObjectPool<PropertyContainer.ExistsAtPathVisitor>(() => new PropertyContainer.ExistsAtPathVisitor(), null, delegate(PropertyContainer.ExistsAtPathVisitor v)
			{
				v.Reset();
			}, null, true, 10, 10000);

			public bool Exists;
		}

		internal class SetValueVisitor<TSrcValue> : PathVisitor
		{
			public override void Reset()
			{
				base.Reset();
				this.Value = default(TSrcValue);
			}

			protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
			{
				bool isReadOnly = property.IsReadOnly;
				if (isReadOnly)
				{
					base.ReturnCode = VisitReturnCode.AccessViolation;
				}
				else
				{
					TValue tvalue;
					bool flag = TypeConversion.TryConvert<TSrcValue, TValue>(ref this.Value, out tvalue);
					if (flag)
					{
						property.SetValue(ref container, tvalue);
					}
					else
					{
						base.ReturnCode = VisitReturnCode.InvalidCast;
					}
				}
			}

			public static readonly ObjectPool<PropertyContainer.SetValueVisitor<TSrcValue>> Pool = new ObjectPool<PropertyContainer.SetValueVisitor<TSrcValue>>(() => new PropertyContainer.SetValueVisitor<TSrcValue>(), null, delegate(PropertyContainer.SetValueVisitor<TSrcValue> v)
			{
				v.Reset();
			}, null, true, 10, 10000);

			public TSrcValue Value;
		}
	}
}
