using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace Unity.Properties
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal readonly struct ConversionRegistry : IEqualityComparer<ConversionRegistry>
	{
		private ConversionRegistry(Dictionary<ConversionRegistry.ConverterKey, Delegate> storage)
		{
			this.m_Converters = storage;
		}

		public static ConversionRegistry Create()
		{
			return new ConversionRegistry(new Dictionary<ConversionRegistry.ConverterKey, Delegate>(ConversionRegistry.Comparer));
		}

		public void Register(Type source, Type destination, Delegate converter)
		{
			Dictionary<ConversionRegistry.ConverterKey, Delegate> converters = this.m_Converters;
			ConversionRegistry.ConverterKey converterKey = new ConversionRegistry.ConverterKey(source, destination);
			if (converter == null)
			{
				throw new ArgumentException("converter");
			}
			converters[converterKey] = converter;
		}

		public Delegate GetConverter(Type source, Type destination)
		{
			ConversionRegistry.ConverterKey converterKey = new ConversionRegistry.ConverterKey(source, destination);
			Delegate @delegate;
			return this.m_Converters.TryGetValue(converterKey, out @delegate) ? @delegate : null;
		}

		public bool TryGetConverter(Type source, Type destination, out Delegate converter)
		{
			converter = this.GetConverter(source, destination);
			return converter != null;
		}

		public bool Equals(ConversionRegistry x, ConversionRegistry y)
		{
			return x.m_Converters == y.m_Converters;
		}

		public int GetHashCode(ConversionRegistry obj)
		{
			return (obj.m_Converters != null) ? obj.m_Converters.GetHashCode() : 0;
		}

		private static readonly ConversionRegistry.ConverterKeyComparer Comparer = new ConversionRegistry.ConverterKeyComparer();

		private readonly Dictionary<ConversionRegistry.ConverterKey, Delegate> m_Converters;

		private class ConverterKeyComparer : IEqualityComparer<ConversionRegistry.ConverterKey>
		{
			public bool Equals(ConversionRegistry.ConverterKey x, ConversionRegistry.ConverterKey y)
			{
				return x.SourceType == y.SourceType && x.DestinationType == y.DestinationType;
			}

			public int GetHashCode(ConversionRegistry.ConverterKey obj)
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
