using System;

namespace Mono.Cecil
{
	public abstract class EventReference : MemberReference
	{
		public TypeReference EventType
		{
			get
			{
				return this.event_type;
			}
			set
			{
				this.event_type = value;
			}
		}

		public override string FullName
		{
			get
			{
				return this.event_type.FullName + " " + base.MemberFullName();
			}
		}

		protected EventReference(string name, TypeReference eventType)
			: base(name)
		{
			Mixin.CheckType(eventType, Mixin.Argument.eventType);
			this.event_type = eventType;
		}

		protected override IMemberDefinition ResolveDefinition()
		{
			return this.Resolve();
		}

		public new abstract EventDefinition Resolve();

		private TypeReference event_type;
	}
}
