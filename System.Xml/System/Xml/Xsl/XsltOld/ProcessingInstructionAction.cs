using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class ProcessingInstructionAction : ContainerAction
	{
		internal ProcessingInstructionAction()
		{
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.nameAvt, "name");
			if (this.nameAvt.IsConstant)
			{
				this.name = this.nameAvt.Evaluate(null, null);
				this.nameAvt = null;
				if (!ProcessingInstructionAction.IsProcessingInstructionName(this.name))
				{
					this.name = null;
				}
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
			if (Ref.Equal(localName, compiler.Atoms.Name))
			{
				this.nameAvt = Avt.CompileAvt(compiler, value);
				return true;
			}
			return false;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			switch (frame.State)
			{
			case 0:
				if (this.nameAvt == null)
				{
					frame.StoredOutput = this.name;
					if (this.name == null)
					{
						frame.Finished();
						return;
					}
				}
				else
				{
					frame.StoredOutput = this.nameAvt.Evaluate(processor, frame);
					if (!ProcessingInstructionAction.IsProcessingInstructionName(frame.StoredOutput))
					{
						frame.Finished();
						return;
					}
				}
				break;
			case 1:
				if (!processor.EndEvent(XPathNodeType.ProcessingInstruction))
				{
					frame.State = 1;
					return;
				}
				frame.Finished();
				return;
			case 2:
				goto IL_00B5;
			case 3:
				break;
			default:
				goto IL_00B5;
			}
			if (!processor.BeginEvent(XPathNodeType.ProcessingInstruction, string.Empty, frame.StoredOutput, string.Empty, false))
			{
				frame.State = 3;
				return;
			}
			processor.PushActionFrame(frame);
			frame.State = 1;
			return;
			IL_00B5:
			frame.Finished();
		}

		internal static bool IsProcessingInstructionName(string name)
		{
			if (name == null)
			{
				return false;
			}
			int length = name.Length;
			int num = 0;
			XmlCharType instance = XmlCharType.Instance;
			while (num < length && instance.IsWhiteSpace(name[num]))
			{
				num++;
			}
			if (num >= length)
			{
				return false;
			}
			int num2 = ValidateNames.ParseNCName(name, num);
			if (num2 == 0)
			{
				return false;
			}
			num += num2;
			while (num < length && instance.IsWhiteSpace(name[num]))
			{
				num++;
			}
			return num >= length && (length != 3 || (name[0] != 'X' && name[0] != 'x') || (name[1] != 'M' && name[1] != 'm') || (name[2] != 'L' && name[2] != 'l'));
		}

		private const int NameEvaluated = 2;

		private const int NameReady = 3;

		private Avt nameAvt;

		private string name;

		private const char CharX = 'X';

		private const char Charx = 'x';

		private const char CharM = 'M';

		private const char Charm = 'm';

		private const char CharL = 'L';

		private const char Charl = 'l';
	}
}
