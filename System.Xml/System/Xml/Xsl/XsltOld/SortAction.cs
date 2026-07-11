using System;
using System.Globalization;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class SortAction : CompiledAction
	{
		private string ParseLang(string value)
		{
			if (value == null)
			{
				return null;
			}
			if (XmlComplianceUtil.IsValidLanguageID(value.ToCharArray(), 0, value.Length) || (value.Length != 0 && CultureInfo.GetCultureInfo(value) != null))
			{
				return value;
			}
			if (this.forwardCompatibility)
			{
				return null;
			}
			throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "lang", value });
		}

		private XmlDataType ParseDataType(string value, InputScopeManager manager)
		{
			if (value == null)
			{
				return XmlDataType.Text;
			}
			if (value == "text")
			{
				return XmlDataType.Text;
			}
			if (value == "number")
			{
				return XmlDataType.Number;
			}
			string text;
			string text2;
			PrefixQName.ParseQualifiedName(value, out text, out text2);
			manager.ResolveXmlNamespace(text);
			if (text.Length == 0 && !this.forwardCompatibility)
			{
				throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "data-type", value });
			}
			return XmlDataType.Text;
		}

		private XmlSortOrder ParseOrder(string value)
		{
			if (value == null)
			{
				return XmlSortOrder.Ascending;
			}
			if (value == "ascending")
			{
				return XmlSortOrder.Ascending;
			}
			if (value == "descending")
			{
				return XmlSortOrder.Descending;
			}
			if (this.forwardCompatibility)
			{
				return XmlSortOrder.Ascending;
			}
			throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "order", value });
		}

		private XmlCaseOrder ParseCaseOrder(string value)
		{
			if (value == null)
			{
				return XmlCaseOrder.None;
			}
			if (value == "upper-first")
			{
				return XmlCaseOrder.UpperFirst;
			}
			if (value == "lower-first")
			{
				return XmlCaseOrder.LowerFirst;
			}
			if (this.forwardCompatibility)
			{
				return XmlCaseOrder.None;
			}
			throw XsltException.Create("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "case-order", value });
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckEmpty(compiler);
			if (this.selectKey == -1)
			{
				this.selectKey = compiler.AddQuery(".");
			}
			this.forwardCompatibility = compiler.ForwardCompatibility;
			this.manager = compiler.CloneScopeManager();
			this.lang = this.ParseLang(CompiledAction.PrecalculateAvt(ref this.langAvt));
			this.dataType = this.ParseDataType(CompiledAction.PrecalculateAvt(ref this.dataTypeAvt), this.manager);
			this.order = this.ParseOrder(CompiledAction.PrecalculateAvt(ref this.orderAvt));
			this.caseOrder = this.ParseCaseOrder(CompiledAction.PrecalculateAvt(ref this.caseOrderAvt));
			if (this.langAvt == null && this.dataTypeAvt == null && this.orderAvt == null && this.caseOrderAvt == null)
			{
				this.sort = new Sort(this.selectKey, this.lang, this.dataType, this.order, this.caseOrder);
			}
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Select))
			{
				this.selectKey = compiler.AddQuery(value);
			}
			else if (Ref.Equal(localName, compiler.Atoms.Lang))
			{
				this.langAvt = Avt.CompileAvt(compiler, value);
			}
			else if (Ref.Equal(localName, compiler.Atoms.DataType))
			{
				this.dataTypeAvt = Avt.CompileAvt(compiler, value);
			}
			else if (Ref.Equal(localName, compiler.Atoms.Order))
			{
				this.orderAvt = Avt.CompileAvt(compiler, value);
			}
			else
			{
				if (!Ref.Equal(localName, compiler.Atoms.CaseOrder))
				{
					return false;
				}
				this.caseOrderAvt = Avt.CompileAvt(compiler, value);
			}
			return true;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			processor.AddSort((this.sort != null) ? this.sort : new Sort(this.selectKey, (this.langAvt == null) ? this.lang : this.ParseLang(this.langAvt.Evaluate(processor, frame)), (this.dataTypeAvt == null) ? this.dataType : this.ParseDataType(this.dataTypeAvt.Evaluate(processor, frame), this.manager), (this.orderAvt == null) ? this.order : this.ParseOrder(this.orderAvt.Evaluate(processor, frame)), (this.caseOrderAvt == null) ? this.caseOrder : this.ParseCaseOrder(this.caseOrderAvt.Evaluate(processor, frame))));
			frame.Finished();
		}

		private int selectKey = -1;

		private Avt langAvt;

		private Avt dataTypeAvt;

		private Avt orderAvt;

		private Avt caseOrderAvt;

		private string lang;

		private XmlDataType dataType = XmlDataType.Text;

		private XmlSortOrder order = XmlSortOrder.Ascending;

		private XmlCaseOrder caseOrder;

		private Sort sort;

		private bool forwardCompatibility;

		private InputScopeManager manager;
	}
}
