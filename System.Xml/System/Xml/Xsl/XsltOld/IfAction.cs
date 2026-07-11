using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class IfAction : ContainerAction
	{
		internal IfAction(IfAction.ConditionType type)
		{
			this.type = type;
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			if (this.type != IfAction.ConditionType.ConditionOtherwise)
			{
				base.CheckRequiredAttribute(compiler, this.testKey != -1, "test");
			}
			if (compiler.Recurse())
			{
				base.CompileTemplate(compiler);
				compiler.ToParent();
			}
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (!Ref.Equal(localName, compiler.Atoms.Test))
			{
				return false;
			}
			if (this.type == IfAction.ConditionType.ConditionOtherwise)
			{
				return false;
			}
			this.testKey = compiler.AddBooleanQuery(value);
			return true;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 1)
				{
					return;
				}
				if (this.type == IfAction.ConditionType.ConditionWhen || this.type == IfAction.ConditionType.ConditionOtherwise)
				{
					frame.Exit();
				}
				frame.Finished();
				return;
			}
			else
			{
				if ((this.type == IfAction.ConditionType.ConditionIf || this.type == IfAction.ConditionType.ConditionWhen) && !processor.EvaluateBoolean(frame, this.testKey))
				{
					frame.Finished();
					return;
				}
				processor.PushActionFrame(frame);
				frame.State = 1;
				return;
			}
		}

		private IfAction.ConditionType type;

		private int testKey = -1;

		internal enum ConditionType
		{
			ConditionIf,
			ConditionWhen,
			ConditionOtherwise
		}
	}
}
