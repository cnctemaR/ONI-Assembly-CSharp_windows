using System;
using System.Collections.Generic;
using System.Globalization;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class AnchorAssigner : PreProcessingPhaseObjectGraphVisitorSkeleton, IAliasProvider
	{
		public AnchorAssigner(IEnumerable<IYamlTypeConverter> typeConverters)
			: base(typeConverters)
		{
		}

		protected override bool Enter(IObjectDescriptor value)
		{
			AnchorAssigner.AnchorAssignment anchorAssignment;
			if (value.Value != null && this.assignments.TryGetValue(value.Value, out anchorAssignment))
			{
				if (anchorAssignment.Anchor == null)
				{
					anchorAssignment.Anchor = "o" + this.nextId.ToString(CultureInfo.InvariantCulture);
					this.nextId += 1U;
				}
				return false;
			}
			return true;
		}

		protected override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value)
		{
			return true;
		}

		protected override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value)
		{
			return true;
		}

		protected override void VisitScalar(IObjectDescriptor scalar)
		{
		}

		protected override void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType)
		{
			this.VisitObject(mapping);
		}

		protected override void VisitMappingEnd(IObjectDescriptor mapping)
		{
		}

		protected override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType)
		{
			this.VisitObject(sequence);
		}

		protected override void VisitSequenceEnd(IObjectDescriptor sequence)
		{
		}

		private void VisitObject(IObjectDescriptor value)
		{
			if (value.Value != null)
			{
				this.assignments.Add(value.Value, new AnchorAssigner.AnchorAssignment());
			}
		}

		string IAliasProvider.GetAlias(object target)
		{
			AnchorAssigner.AnchorAssignment anchorAssignment;
			if (target != null && this.assignments.TryGetValue(target, out anchorAssignment))
			{
				return anchorAssignment.Anchor;
			}
			return null;
		}

		private readonly IDictionary<object, AnchorAssigner.AnchorAssignment> assignments = new Dictionary<object, AnchorAssigner.AnchorAssignment>();

		private uint nextId;

		private class AnchorAssignment
		{
			public string Anchor;
		}
	}
}
