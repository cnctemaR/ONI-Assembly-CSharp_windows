using System;
using System.Collections.Generic;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class LabelScopeInfo
	{
		internal LabelScopeInfo(LabelScopeInfo parent, LabelScopeKind kind)
		{
			this.Parent = parent;
			this.Kind = kind;
		}

		internal bool CanJumpInto
		{
			get
			{
				LabelScopeKind kind = this.Kind;
				return kind <= LabelScopeKind.Lambda;
			}
		}

		internal bool ContainsTarget(LabelTarget target)
		{
			return this._labels != null && this._labels.ContainsKey(target);
		}

		internal bool TryGetLabelInfo(LabelTarget target, out LabelInfo info)
		{
			if (this._labels == null)
			{
				info = null;
				return false;
			}
			return this._labels.TryGetValue(target, out info);
		}

		internal void AddLabelInfo(LabelTarget target, LabelInfo info)
		{
			if (this._labels == null)
			{
				this._labels = new Dictionary<LabelTarget, LabelInfo>();
			}
			this._labels.Add(target, info);
		}

		private Dictionary<LabelTarget, LabelInfo> _labels;

		internal readonly LabelScopeKind Kind;

		internal readonly LabelScopeInfo Parent;
	}
}
