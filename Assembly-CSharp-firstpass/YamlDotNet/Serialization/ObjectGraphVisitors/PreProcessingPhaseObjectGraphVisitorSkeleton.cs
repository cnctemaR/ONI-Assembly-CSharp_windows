using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public abstract class PreProcessingPhaseObjectGraphVisitorSkeleton : IObjectGraphVisitor<Nothing>
	{
		public PreProcessingPhaseObjectGraphVisitorSkeleton(IEnumerable<IYamlTypeConverter> typeConverters)
		{
			this.typeConverters = ((typeConverters == null) ? Enumerable.Empty<IYamlTypeConverter>() : typeConverters.ToList<IYamlTypeConverter>());
		}

		bool IObjectGraphVisitor<Nothing>.Enter(IObjectDescriptor value, Nothing context)
		{
			IYamlTypeConverter yamlTypeConverter = this.typeConverters.FirstOrDefault<IYamlTypeConverter>((IYamlTypeConverter t) => t.Accepts(value.Type));
			if (yamlTypeConverter != null)
			{
				return false;
			}
			IYamlConvertible yamlConvertible = value.Value as IYamlConvertible;
			if (yamlConvertible != null)
			{
				return false;
			}
			IYamlSerializable yamlSerializable = value.Value as IYamlSerializable;
			return yamlSerializable == null && this.Enter(value);
		}

		bool IObjectGraphVisitor<Nothing>.EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, Nothing context)
		{
			return this.EnterMapping(key, value);
		}

		bool IObjectGraphVisitor<Nothing>.EnterMapping(IObjectDescriptor key, IObjectDescriptor value, Nothing context)
		{
			return this.EnterMapping(key, value);
		}

		void IObjectGraphVisitor<Nothing>.VisitMappingEnd(IObjectDescriptor mapping, Nothing context)
		{
			this.VisitMappingEnd(mapping);
		}

		void IObjectGraphVisitor<Nothing>.VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, Nothing context)
		{
			this.VisitMappingStart(mapping, keyType, valueType);
		}

		void IObjectGraphVisitor<Nothing>.VisitScalar(IObjectDescriptor scalar, Nothing context)
		{
			this.VisitScalar(scalar);
		}

		void IObjectGraphVisitor<Nothing>.VisitSequenceEnd(IObjectDescriptor sequence, Nothing context)
		{
			this.VisitSequenceEnd(sequence);
		}

		void IObjectGraphVisitor<Nothing>.VisitSequenceStart(IObjectDescriptor sequence, Type elementType, Nothing context)
		{
			this.VisitSequenceStart(sequence, elementType);
		}

		protected abstract bool Enter(IObjectDescriptor value);

		protected abstract bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value);

		protected abstract bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value);

		protected abstract void VisitMappingEnd(IObjectDescriptor mapping);

		protected abstract void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType);

		protected abstract void VisitScalar(IObjectDescriptor scalar);

		protected abstract void VisitSequenceEnd(IObjectDescriptor sequence);

		protected abstract void VisitSequenceStart(IObjectDescriptor sequence, Type elementType);

		protected readonly IEnumerable<IYamlTypeConverter> typeConverters;
	}
}
