using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class AttributeAction : ContainerAction
	{
		private static PrefixQName CreateAttributeQName(string name, string nsUri, InputScopeManager manager)
		{
			if (name == "xmlns")
			{
				return null;
			}
			if (nsUri == "http://www.w3.org/2000/xmlns/")
			{
				throw XsltException.Create("Elements and attributes cannot belong to the reserved namespace '{0}'.", new string[] { nsUri });
			}
			PrefixQName prefixQName = new PrefixQName();
			prefixQName.SetQName(name);
			prefixQName.Namespace = ((nsUri != null) ? nsUri : manager.ResolveXPathNamespace(prefixQName.Prefix));
			if (prefixQName.Prefix.StartsWith("xml", StringComparison.Ordinal))
			{
				if (prefixQName.Prefix.Length == 3)
				{
					if (!(prefixQName.Namespace == "http://www.w3.org/XML/1998/namespace") || (!(prefixQName.Name == "lang") && !(prefixQName.Name == "space")))
					{
						prefixQName.ClearPrefix();
					}
				}
				else if (prefixQName.Prefix == "xmlns")
				{
					if (prefixQName.Namespace == "http://www.w3.org/2000/xmlns/")
					{
						throw XsltException.Create("Prefix '{0}' is not defined.", new string[] { prefixQName.Prefix });
					}
					prefixQName.ClearPrefix();
				}
			}
			return prefixQName;
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.nameAvt, "name");
			this.name = CompiledAction.PrecalculateAvt(ref this.nameAvt);
			this.nsUri = CompiledAction.PrecalculateAvt(ref this.nsAvt);
			if (this.nameAvt == null && this.nsAvt == null)
			{
				if (this.name != "xmlns")
				{
					this.qname = AttributeAction.CreateAttributeQName(this.name, this.nsUri, compiler.CloneScopeManager());
				}
			}
			else
			{
				this.manager = compiler.CloneScopeManager();
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
			}
			else
			{
				if (!Ref.Equal(localName, compiler.Atoms.Namespace))
				{
					return false;
				}
				this.nsAvt = Avt.CompileAvt(compiler, value);
			}
			return true;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			switch (frame.State)
			{
			case 0:
				if (this.qname != null)
				{
					frame.CalulatedName = this.qname;
				}
				else
				{
					frame.CalulatedName = AttributeAction.CreateAttributeQName((this.nameAvt == null) ? this.name : this.nameAvt.Evaluate(processor, frame), (this.nsAvt == null) ? this.nsUri : this.nsAvt.Evaluate(processor, frame), this.manager);
					if (frame.CalulatedName == null)
					{
						frame.Finished();
						return;
					}
				}
				break;
			case 1:
				if (!processor.EndEvent(XPathNodeType.Attribute))
				{
					frame.State = 1;
					return;
				}
				frame.Finished();
				return;
			case 2:
				break;
			default:
				return;
			}
			PrefixQName calulatedName = frame.CalulatedName;
			if (!processor.BeginEvent(XPathNodeType.Attribute, calulatedName.Prefix, calulatedName.Name, calulatedName.Namespace, false))
			{
				frame.State = 2;
				return;
			}
			processor.PushActionFrame(frame);
			frame.State = 1;
		}

		private const int NameDone = 2;

		private Avt nameAvt;

		private Avt nsAvt;

		private InputScopeManager manager;

		private string name;

		private string nsUri;

		private PrefixQName qname;
	}
}
