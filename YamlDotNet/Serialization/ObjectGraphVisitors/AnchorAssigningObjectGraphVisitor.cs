using System;
using System.Collections.Generic;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class AnchorAssigningObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public AnchorAssigningObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor, IEventEmitter eventEmitter, IAliasProvider aliasProvider)
			: base(nextVisitor)
		{
			this.eventEmitter = eventEmitter;
			this.aliasProvider = aliasProvider;
		}

		public override bool Enter(IObjectDescriptor value, IEmitter context)
		{
			string alias = this.aliasProvider.GetAlias(value.Value);
			if (alias != null && !this.emittedAliases.Add(alias))
			{
				this.eventEmitter.Emit(new AliasEventInfo(value)
				{
					Alias = alias
				}, context);
				return false;
			}
			return base.Enter(value, context);
		}

		public override void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, IEmitter context)
		{
			this.eventEmitter.Emit(new MappingStartEventInfo(mapping)
			{
				Anchor = this.aliasProvider.GetAlias(mapping.Value)
			}, context);
		}

		public override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, IEmitter context)
		{
			this.eventEmitter.Emit(new SequenceStartEventInfo(sequence)
			{
				Anchor = this.aliasProvider.GetAlias(sequence.Value)
			}, context);
		}

		public override void VisitScalar(IObjectDescriptor scalar, IEmitter context)
		{
			this.eventEmitter.Emit(new ScalarEventInfo(scalar)
			{
				Anchor = this.aliasProvider.GetAlias(scalar.Value)
			}, context);
		}

		private readonly IEventEmitter eventEmitter;

		private readonly IAliasProvider aliasProvider;

		private readonly HashSet<string> emittedAliases = new HashSet<string>();
	}
}
