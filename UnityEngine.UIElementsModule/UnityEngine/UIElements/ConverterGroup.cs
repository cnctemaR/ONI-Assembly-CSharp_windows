using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public class ConverterGroup
	{
		public string id { get; }

		public string displayName { get; }

		public string description { get; }

		internal TypeConverterRegistry registry { get; }

		public ConverterGroup(string id, string displayName = null, string description = null)
		{
			this.id = id;
			this.displayName = displayName;
			this.description = description;
			this.registry = TypeConverterRegistry.Create();
		}

		public void AddConverter<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
		{
			this.registry.Register(typeof(TSource), typeof(TDestination), converter);
		}

		public bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
		{
			Delegate @delegate;
			TypeConverter<TSource, TDestination> typeConverter;
			bool flag;
			if (this.registry.TryGetConverter(typeof(TSource), typeof(TDestination), out @delegate))
			{
				typeConverter = @delegate as TypeConverter<TSource, TDestination>;
				flag = typeConverter != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool flag3;
			if (flag2)
			{
				destination = typeConverter(ref source);
				flag3 = true;
			}
			else
			{
				destination = default(TDestination);
				flag3 = false;
			}
			return flag3;
		}

		public bool TrySetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value, out VisitReturnCode returnCode)
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
				SetValueVisitor<TValue> setValueVisitor = SetValueVisitor<TValue>.Pool.Get();
				setValueVisitor.group = this;
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
					SetValueVisitor<TValue>.Pool.Release(setValueVisitor);
				}
				flag = returnCode == VisitReturnCode.Ok;
			}
			return flag;
		}
	}
}
