using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class VariableAction : ContainerAction, IXsltContextVariable
	{
		internal int Stylesheetid
		{
			get
			{
				return this.stylesheetid;
			}
		}

		internal XmlQualifiedName Name
		{
			get
			{
				return this.name;
			}
		}

		internal string NameStr
		{
			get
			{
				return this.nameStr;
			}
		}

		internal VariableType VarType
		{
			get
			{
				return this.varType;
			}
		}

		internal int VarKey
		{
			get
			{
				return this.varKey;
			}
		}

		internal bool IsGlobal
		{
			get
			{
				return this.varType == VariableType.GlobalVariable || this.varType == VariableType.GlobalParameter;
			}
		}

		internal VariableAction(VariableType type)
		{
			this.varType = type;
		}

		internal override void Compile(Compiler compiler)
		{
			this.stylesheetid = compiler.Stylesheetid;
			this.baseUri = compiler.Input.BaseURI;
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
			if (this.containedActions != null)
			{
				this.baseUri = this.baseUri + "#" + compiler.GetUnicRtfId();
			}
			else
			{
				this.baseUri = null;
			}
			this.varKey = compiler.InsertVariable(this);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Name))
			{
				this.nameStr = value;
				this.name = compiler.CreateXPathQName(this.nameStr);
			}
			else
			{
				if (!Ref.Equal(localName, compiler.Atoms.Select))
				{
					return false;
				}
				this.selectKey = compiler.AddQuery(value);
			}
			return true;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			object obj = null;
			switch (frame.State)
			{
			case 0:
				if (this.IsGlobal)
				{
					if (frame.GetVariable(this.varKey) != null)
					{
						frame.Finished();
						return;
					}
					frame.SetVariable(this.varKey, VariableAction.BeingComputedMark);
				}
				if (this.varType == VariableType.GlobalParameter)
				{
					obj = processor.GetGlobalParameter(this.name);
				}
				else if (this.varType == VariableType.LocalParameter)
				{
					obj = processor.GetParameter(this.name);
				}
				if (obj == null)
				{
					if (this.selectKey != -1)
					{
						obj = processor.RunQuery(frame, this.selectKey);
					}
					else
					{
						if (this.containedActions != null)
						{
							NavigatorOutput navigatorOutput = new NavigatorOutput(this.baseUri);
							processor.PushOutput(navigatorOutput);
							processor.PushActionFrame(frame);
							frame.State = 1;
							return;
						}
						obj = string.Empty;
					}
				}
				break;
			case 1:
				obj = ((NavigatorOutput)processor.PopOutput()).Navigator;
				break;
			case 2:
				break;
			default:
				return;
			}
			frame.SetVariable(this.varKey, obj);
			frame.Finished();
		}

		XPathResultType IXsltContextVariable.VariableType
		{
			get
			{
				return XPathResultType.Any;
			}
		}

		object IXsltContextVariable.Evaluate(XsltContext xsltContext)
		{
			return ((XsltCompileContext)xsltContext).EvaluateVariable(this);
		}

		bool IXsltContextVariable.IsLocal
		{
			get
			{
				return this.varType == VariableType.LocalVariable || this.varType == VariableType.LocalParameter;
			}
		}

		bool IXsltContextVariable.IsParam
		{
			get
			{
				return this.varType == VariableType.LocalParameter || this.varType == VariableType.GlobalParameter;
			}
		}

		public static object BeingComputedMark = new object();

		private const int ValueCalculated = 2;

		protected XmlQualifiedName name;

		protected string nameStr;

		protected string baseUri;

		protected int selectKey = -1;

		protected int stylesheetid;

		protected VariableType varType;

		private int varKey;
	}
}
