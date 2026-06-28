using System;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public abstract class ChainedObjectGraphVisitor : IObjectGraphVisitor
	{
		protected ChainedObjectGraphVisitor(IObjectGraphVisitor nextVisitor)
		{
			this.nextVisitor = nextVisitor;
		}

		public virtual bool Enter(IObjectDescriptor value)
		{
			return this.nextVisitor.Enter(value);
		}

		public virtual bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value)
		{
			return this.nextVisitor.EnterMapping(key, value);
		}

		public virtual bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value)
		{
			return this.nextVisitor.EnterMapping(key, value);
		}

		public virtual void VisitScalar(IObjectDescriptor scalar)
		{
			this.nextVisitor.VisitScalar(scalar);
		}

		public virtual void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType)
		{
			this.nextVisitor.VisitMappingStart(mapping, keyType, valueType);
		}

		public virtual void VisitMappingEnd(IObjectDescriptor mapping)
		{
			this.nextVisitor.VisitMappingEnd(mapping);
		}

		public virtual void VisitSequenceStart(IObjectDescriptor sequence, Type elementType)
		{
			this.nextVisitor.VisitSequenceStart(sequence, elementType);
		}

		public virtual void VisitSequenceEnd(IObjectDescriptor sequence)
		{
			this.nextVisitor.VisitSequenceEnd(sequence);
		}

		private readonly IObjectGraphVisitor nextVisitor;
	}
}
