using System;
using System.Globalization;
using System.IO;

namespace System.Xml.Xsl.XsltOld
{
	internal class MessageAction : ContainerAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
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
			if (Ref.Equal(localName, compiler.Atoms.Terminate))
			{
				this._Terminate = compiler.GetYesNo(value);
				return true;
			}
			return false;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state == 0)
			{
				TextOnlyOutput textOnlyOutput = new TextOnlyOutput(processor, new StringWriter(CultureInfo.InvariantCulture));
				processor.PushOutput(textOnlyOutput);
				processor.PushActionFrame(frame);
				frame.State = 1;
				return;
			}
			if (state != 1)
			{
				return;
			}
			TextOnlyOutput textOnlyOutput2 = processor.PopOutput() as TextOnlyOutput;
			Console.WriteLine(textOnlyOutput2.Writer.ToString());
			if (this._Terminate)
			{
				throw XsltException.Create("Transform terminated: '{0}'.", new string[] { textOnlyOutput2.Writer.ToString() });
			}
			frame.Finished();
		}

		private bool _Terminate;
	}
}
