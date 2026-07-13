using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal readonly struct TypeConverterRegistry : IEqualityComparer<TypeConverterRegistry>
	{
		private TypeConverterRegistry(Dictionary<TypeConverterRegistry.ConverterKey, Delegate> storage)
		{
			this.m_Converters = storage;
		}

		public int ConverterCount
		{
			get
			{
				Dictionary<TypeConverterRegistry.ConverterKey, Delegate> converters = this.m_Converters;
				return (converters != null) ? converters.Count : 0;
			}
		}

		public static TypeConverterRegistry Create()
		{
			return new TypeConverterRegistry(new Dictionary<TypeConverterRegistry.ConverterKey, Delegate>(TypeConverterRegistry.k_Comparer));
		}

		public void Register(Type source, Type destination, Delegate converter)
		{
			Dictionary<TypeConverterRegistry.ConverterKey, Delegate> converters = this.m_Converters;
			TypeConverterRegistry.ConverterKey converterKey = new TypeConverterRegistry.ConverterKey(source, destination);
			if (converter == null)
			{
				throw new ArgumentException("converter");
			}
			converters[converterKey] = converter;
		}

		public void Unregister(Type source, Type destination)
		{
			this.m_Converters.Remove(new TypeConverterRegistry.ConverterKey(source, destination));
		}

		internal void Apply(TypeConverterRegistry registry)
		{
			foreach (KeyValuePair<TypeConverterRegistry.ConverterKey, Delegate> keyValuePair in registry.m_Converters)
			{
				this.Register(keyValuePair.Key.SourceType, keyValuePair.Key.DestinationType, keyValuePair.Value);
			}
		}

		public Delegate GetConverter(Type source, Type destination)
		{
			TypeConverterRegistry.ConverterKey converterKey = new TypeConverterRegistry.ConverterKey(source, destination);
			Delegate @delegate;
			return this.m_Converters.TryGetValue(converterKey, out @delegate) ? @delegate : null;
		}

		public bool TryGetConverter(Type source, Type destination, out Delegate converter)
		{
			converter = this.GetConverter(source, destination);
			return converter != null;
		}

		public void GetAllTypesConvertingToType(Type type, List<Type> result)
		{
			bool flag = this.m_Converters == null;
			if (!flag)
			{
				foreach (TypeConverterRegistry.ConverterKey converterKey in this.m_Converters.Keys)
				{
					bool flag2 = converterKey.DestinationType == type;
					if (flag2)
					{
						result.Add(converterKey.SourceType);
					}
				}
			}
		}

		public void GetAllTypesConvertingFromType(Type type, List<Type> result)
		{
			bool flag = this.m_Converters == null;
			if (!flag)
			{
				foreach (TypeConverterRegistry.ConverterKey converterKey in this.m_Converters.Keys)
				{
					bool flag2 = converterKey.SourceType == type;
					if (flag2)
					{
						result.Add(converterKey.DestinationType);
					}
				}
			}
		}

		public void GetAllConversions(List<ValueTuple<Type, Type>> result)
		{
			bool flag = this.m_Converters == null;
			if (!flag)
			{
				foreach (TypeConverterRegistry.ConverterKey converterKey in this.m_Converters.Keys)
				{
					result.Add(new ValueTuple<Type, Type>(converterKey.SourceType, converterKey.DestinationType));
				}
			}
		}

		public bool Equals(TypeConverterRegistry x, TypeConverterRegistry y)
		{
			return x.m_Converters == y.m_Converters;
		}

		public int GetHashCode(TypeConverterRegistry obj)
		{
			return (obj.m_Converters != null) ? obj.m_Converters.GetHashCode() : 0;
		}

		private static readonly TypeConverterRegistry.ConverterKeyComparer k_Comparer = new TypeConverterRegistry.ConverterKeyComparer();

		private readonly Dictionary<TypeConverterRegistry.ConverterKey, Delegate> m_Converters;

		private class ConverterKeyComparer : IEqualityComparer<TypeConverterRegistry.ConverterKey>
		{
			public bool Equals(TypeConverterRegistry.ConverterKey x, TypeConverterRegistry.ConverterKey y)
			{
				return x.SourceType == y.SourceType && x.DestinationType == y.DestinationType;
			}

			public int GetHashCode(TypeConverterRegistry.ConverterKey obj)
			{
				return (((obj.SourceType != null) ? obj.SourceType.GetHashCode() : 0) * 397) ^ ((obj.DestinationType != null) ? obj.DestinationType.GetHashCode() : 0);
			}
		}

		private readonly struct ConverterKey
		{
			public ConverterKey(Type source, Type destination)
			{
				this.SourceType = source;
				this.DestinationType = destination;
			}

			public readonly Type SourceType;

			public readonly Type DestinationType;
		}
	}
}
