using System;
using System.Collections.Generic;
using System.Globalization;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class AnchorAssigner : IObjectGraphVisitor, IAliasProvider
	{
		bool IObjectGraphVisitor.Enter(IObjectDescriptor value)
		{
			if (value.Value == null || value.Type.GetTypeCode() != TypeCode.Object)
			{
				return false;
			}
			AnchorAssigner.AnchorAssignment anchorAssignment;
			if (this.assignments.TryGetValue(value.Value, out anchorAssignment))
			{
				if (anchorAssignment.Anchor == null)
				{
					anchorAssignment.Anchor = "o" + this.nextId.ToString(CultureInfo.InvariantCulture);
					this.nextId += 1U;
				}
				return false;
			}
			this.assignments.Add(value.Value, new AnchorAssigner.AnchorAssignment());
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
		}

		void IObjectGraphVisitor.VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType)
		{
		}

		void IObjectGraphVisitor.VisitMappingEnd(IObjectDescriptor mapping)
		{
		}

		void IObjectGraphVisitor.VisitSequenceStart(IObjectDescriptor sequence, Type elementType)
		{
		}

		void IObjectGraphVisitor.VisitSequenceEnd(IObjectDescriptor sequence)
		{
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
