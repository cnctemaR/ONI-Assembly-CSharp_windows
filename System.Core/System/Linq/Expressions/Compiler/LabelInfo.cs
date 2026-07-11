using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Reflection.Emit;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class LabelInfo
	{
		internal Label Label
		{
			get
			{
				this.EnsureLabelAndValue();
				return this._label;
			}
		}

		internal LabelInfo(ILGenerator il, LabelTarget node, bool canReturn)
		{
			this._ilg = il;
			this._node = node;
			this._canReturn = canReturn;
		}

		internal bool CanReturn
		{
			get
			{
				return this._canReturn;
			}
		}

		internal bool CanBranch
		{
			get
			{
				return this._opCode != OpCodes.Leave;
			}
		}

		internal void Reference(LabelScopeInfo block)
		{
			this._references.Add(block);
			if (this._definitions.Count > 0)
			{
				this.ValidateJump(block);
			}
		}

		internal void Define(LabelScopeInfo block)
		{
			for (LabelScopeInfo labelScopeInfo = block; labelScopeInfo != null; labelScopeInfo = labelScopeInfo.Parent)
			{
				if (labelScopeInfo.ContainsTarget(this._node))
				{
					throw Error.LabelTargetAlreadyDefined(this._node.Name);
				}
			}
			this._definitions.Add(block);
			block.AddLabelInfo(this._node, this);
			if (this._definitions.Count == 1)
			{
				using (List<LabelScopeInfo>.Enumerator enumerator = this._references.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						LabelScopeInfo labelScopeInfo2 = enumerator.Current;
						this.ValidateJump(labelScopeInfo2);
					}
					return;
				}
			}
			if (this._acrossBlockJump)
			{
				throw Error.AmbiguousJump(this._node.Name);
			}
			this._labelDefined = false;
		}

		private void ValidateJump(LabelScopeInfo reference)
		{
			this._opCode = (this._canReturn ? OpCodes.Ret : OpCodes.Br);
			for (LabelScopeInfo labelScopeInfo = reference; labelScopeInfo != null; labelScopeInfo = labelScopeInfo.Parent)
			{
				if (this._definitions.Contains(labelScopeInfo))
				{
					return;
				}
				if (labelScopeInfo.Kind == LabelScopeKind.Finally || labelScopeInfo.Kind == LabelScopeKind.Filter)
				{
					break;
				}
				if (labelScopeInfo.Kind == LabelScopeKind.Try || labelScopeInfo.Kind == LabelScopeKind.Catch)
				{
					this._opCode = OpCodes.Leave;
				}
			}
			this._acrossBlockJump = true;
			if (this._node != null && this._node.Type != typeof(void))
			{
				throw Error.NonLocalJumpWithValue(this._node.Name);
			}
			if (this._definitions.Count > 1)
			{
				throw Error.AmbiguousJump(this._node.Name);
			}
			LabelScopeInfo labelScopeInfo2 = this._definitions.First<LabelScopeInfo>();
			LabelScopeInfo labelScopeInfo3 = Helpers.CommonNode<LabelScopeInfo>(labelScopeInfo2, reference, (LabelScopeInfo b) => b.Parent);
			this._opCode = (this._canReturn ? OpCodes.Ret : OpCodes.Br);
			for (LabelScopeInfo labelScopeInfo4 = reference; labelScopeInfo4 != labelScopeInfo3; labelScopeInfo4 = labelScopeInfo4.Parent)
			{
				if (labelScopeInfo4.Kind == LabelScopeKind.Finally)
				{
					throw Error.ControlCannotLeaveFinally();
				}
				if (labelScopeInfo4.Kind == LabelScopeKind.Filter)
				{
					throw Error.ControlCannotLeaveFilterTest();
				}
				if (labelScopeInfo4.Kind == LabelScopeKind.Try || labelScopeInfo4.Kind == LabelScopeKind.Catch)
				{
					this._opCode = OpCodes.Leave;
				}
			}
			LabelScopeInfo labelScopeInfo5 = labelScopeInfo2;
			while (labelScopeInfo5 != labelScopeInfo3)
			{
				if (!labelScopeInfo5.CanJumpInto)
				{
					if (labelScopeInfo5.Kind == LabelScopeKind.Expression)
					{
						throw Error.ControlCannotEnterExpression();
					}
					throw Error.ControlCannotEnterTry();
				}
				else
				{
					labelScopeInfo5 = labelScopeInfo5.Parent;
				}
			}
		}

		internal void ValidateFinish()
		{
			if (this._references.Count > 0 && this._definitions.Count == 0)
			{
				throw Error.LabelTargetUndefined(this._node.Name);
			}
		}

		internal void EmitJump()
		{
			if (this._opCode == OpCodes.Ret)
			{
				this._ilg.Emit(OpCodes.Ret);
				return;
			}
			this.StoreValue();
			this._ilg.Emit(this._opCode, this.Label);
		}

		private void StoreValue()
		{
			this.EnsureLabelAndValue();
			if (this._value != null)
			{
				this._ilg.Emit(OpCodes.Stloc, this._value);
			}
		}

		internal void Mark()
		{
			if (this._canReturn)
			{
				if (!this._labelDefined)
				{
					return;
				}
				this._ilg.Emit(OpCodes.Ret);
			}
			else
			{
				this.StoreValue();
			}
			this.MarkWithEmptyStack();
		}

		internal void MarkWithEmptyStack()
		{
			this._ilg.MarkLabel(this.Label);
			if (this._value != null)
			{
				this._ilg.Emit(OpCodes.Ldloc, this._value);
			}
		}

		private void EnsureLabelAndValue()
		{
			if (!this._labelDefined)
			{
				this._labelDefined = true;
				this._label = this._ilg.DefineLabel();
				if (this._node != null && this._node.Type != typeof(void))
				{
					this._value = this._ilg.DeclareLocal(this._node.Type);
				}
			}
		}

		private readonly LabelTarget _node;

		private Label _label;

		private bool _labelDefined;

		private LocalBuilder _value;

		private readonly HashSet<LabelScopeInfo> _definitions = new HashSet<LabelScopeInfo>();

		private readonly List<LabelScopeInfo> _references = new List<LabelScopeInfo>();

		private readonly bool _canReturn;

		private bool _acrossBlockJump;

		private OpCode _opCode = OpCodes.Leave;

		private readonly ILGenerator _ilg;
	}
}
