using System;
using System.Reflection;

namespace System.Runtime.Serialization
{
	[Serializable]
	internal sealed class MemberHolder
	{
		internal MemberHolder(Type type, StreamingContext ctx)
		{
			this._memberType = type;
			this._context = ctx;
		}

		public override int GetHashCode()
		{
			return this._memberType.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			MemberHolder memberHolder = obj as MemberHolder;
			return memberHolder != null && memberHolder._memberType == this._memberType && memberHolder._context.State == this._context.State;
		}

		internal readonly MemberInfo[] _members;

		internal readonly Type _memberType;

		internal readonly StreamingContext _context;
	}
}
