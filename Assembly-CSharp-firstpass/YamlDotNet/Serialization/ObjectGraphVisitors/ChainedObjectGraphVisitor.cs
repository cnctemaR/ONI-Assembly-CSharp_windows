using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public abstract class ChainedObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
	{
		protected ChainedObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
		{
			this.nextVisitor = nextVisitor;
		}

		public virtual bool Enter(IObjectDescriptor value, IEmitter context)
		{
			return this.nextVisitor.Enter(value, context);
		}

		public virtual bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return this.nextVisitor.EnterMapping(key, value, context);
		}

		public virtual bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return this.nextVisitor.EnterMapping(key, value, context);
		}

		public virtual void VisitScalar(IObjectDescriptor scalar, IEmitter context)
		{
			this.nextVisitor.VisitScalar(scalar, context);
		}

		public virtual void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, IEmitter context)
		{
			this.nextVisitor.VisitMappingStart(mapping, keyType, valueType, context);
		}

		public virtual void VisitMappingEnd(IObjectDescriptor mapping, IEmitter context)
		{
			this.nextVisitor.VisitMappingEnd(mapping, context);
		}

		public virtual void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, IEmitter context)
		{
			this.nextVisitor.VisitSequenceStart(sequence, elementType, context);
		}

		public virtual void VisitSequenceEnd(IObjectDescriptor sequence, IEmitter context)
		{
			this.nextVisitor.VisitSequenceEnd(sequence, context);
		}

		private readonly IObjectGraphVisitor<IEmitter> nextVisitor;
	}
}
