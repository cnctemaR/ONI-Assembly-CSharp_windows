using System;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class EmittingObjectGraphVisitor : IObjectGraphVisitor
	{
		public EmittingObjectGraphVisitor(IEventEmitter eventEmitter)
		{
			this.eventEmitter = eventEmitter;
		}

		bool IObjectGraphVisitor.Enter(IObjectDescriptor value)
		{
			return true;
		}

		bool IObjectGraphVisitor.EnterMapping(IObjectDescriptor key, IObjectDescriptor value)
		{
			return true;
		}

		bool IObjectGraphVisitor.EnterMapping(IPropertyDescriptor key, IObjectDescriptor value)
		{
			return true;
		}

		void IObjectGraphVisitor.VisitScalar(IObjectDescriptor scalar)
		{
			this.eventEmitter.Emit(new ScalarEventInfo(scalar));
		}

		void IObjectGraphVisitor.VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType)
		{
			this.eventEmitter.Emit(new MappingStartEventInfo(mapping));
		}

		void IObjectGraphVisitor.VisitMappingEnd(IObjectDescriptor mapping)
		{
			this.eventEmitter.Emit(new MappingEndEventInfo(mapping));
		}

		void IObjectGraphVisitor.VisitSequenceStart(IObjectDescriptor sequence, Type elementType)
		{
			this.eventEmitter.Emit(new SequenceStartEventInfo(sequence));
		}

		void IObjectGraphVisitor.VisitSequenceEnd(IObjectDescriptor sequence)
		{
			this.eventEmitter.Emit(new SequenceEndEventInfo(sequence));
		}

		private readonly IEventEmitter eventEmitter;
	}
}
