using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class WithParamAction : VariableAction
	{
		internal WithParamAction()
			: base(VariableType.WithParameter)
		{
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.name, "name");
			if (compiler.Recurse())
			{
				base.CompileTemplate(compiler);
				compiler.ToParent();
				if (this.selectKey != -1 && this.containedActions != null)
				{
					throw XsltException.Create("The variable or parameter '{0}' cannot have both a 'select' attribute and non-empty content.", new string[] { this.nameStr });
				}
			}
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
				RecordOutput recordOutput = processor.PopOutput();
				processor.SetParameter(this.name, ((NavigatorOutput)recordOutput).Navigator);
				frame.Finished();
				return;
			}
			else
			{
				if (this.selectKey != -1)
				{
					object obj = processor.RunQuery(frame, this.selectKey);
					processor.SetParameter(this.name, obj);
					frame.Finished();
					return;
				}
				if (this.containedActions == null)
				{
					processor.SetParameter(this.name, string.Empty);
					frame.Finished();
					return;
				}
				NavigatorOutput navigatorOutput = new NavigatorOutput(this.baseUri);
				processor.PushOutput(navigatorOutput);
				processor.PushActionFrame(frame);
				frame.State = 1;
				return;
			}
		}
	}
}
