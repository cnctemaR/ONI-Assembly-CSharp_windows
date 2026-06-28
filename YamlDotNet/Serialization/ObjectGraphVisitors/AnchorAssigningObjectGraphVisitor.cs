using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class AnchorAssigningObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public AnchorAssigningObjectGraphVisitor(IObjectGraphVisitor nextVisitor, IEventEmitter eventEmitter, IAliasProvider aliasProvider)
			: base(nextVisitor)
		{
			this.eventEmitter = eventEmitter;
			this.aliasProvider = aliasProvider;
		}

		public override bool Enter(IObjectDescriptor value)
		{
			string alias = this.aliasProvider.GetAlias(value.Value);
			if (alias != null && !this.emittedAliases.Add(alias))
			{
				this.eventEmitter.Emit(new AliasEventInfo(value)
				{
					Alias = alias
				});
				return false;
			}
			return base.Enter(value);
		}

		public override void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType)
		{
			this.eventEmitter.Emit(new MappingStartEventInfo(mapping)
			{
				Anchor = this.aliasProvider.GetAlias(mapping.Value)
			});
		}

		public override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType)
		{
			this.eventEmitter.Emit(new SequenceStartEventInfo(sequence)
			{
				Anchor = this.aliasProvider.GetAlias(sequence.Value)
			});
		}

		public override void VisitScalar(IObjectDescriptor scalar)
		{
			this.eventEmitter.Emit(new ScalarEventInfo(scalar)
			{
				Anchor = this.aliasProvider.GetAlias(scalar.Value)
			});
		}

		private readonly IEventEmitter eventEmitter;

		private readonly IAliasProvider aliasProvider;

		private readonly HashSet<string> emittedAliases = new HashSet<string>();
	}
}
