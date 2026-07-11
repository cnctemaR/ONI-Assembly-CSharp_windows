using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class ValueOfAction : CompiledAction
	{
		internal static Action BuiltInRule()
		{
			return ValueOfAction.s_BuiltInRule;
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.selectKey != -1, "select");
			base.CheckEmpty(compiler);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Select))
			{
				this.selectKey = compiler.AddQuery(value);
			}
			else
			{
				if (!Ref.Equal(localName, compiler.Atoms.DisableOutputEscaping))
				{
					return false;
				}
				this.disableOutputEscaping = compiler.GetYesNo(value);
			}
			return true;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 2)
				{
					return;
				}
				processor.TextEvent(frame.StoredOutput);
				frame.Finished();
				return;
			}
			else
			{
				string text = processor.ValueOf(frame, this.selectKey);
				if (processor.TextEvent(text, this.disableOutputEscaping))
				{
					frame.Finished();
					return;
				}
				frame.StoredOutput = text;
				frame.State = 2;
				return;
			}
		}

		private const int ResultStored = 2;

		private int selectKey = -1;

		private bool disableOutputEscaping;

		private static Action s_BuiltInRule = new BuiltInRuleTextAction();
	}
}
