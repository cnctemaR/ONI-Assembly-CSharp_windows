using System;
using System.Collections;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.IlGen
{
	internal class XmlILElementAnalyzer : XmlILStateAnalyzer
	{
		public XmlILElementAnalyzer(QilFactory fac)
			: base(fac)
		{
		}

		public override QilNode Analyze(QilNode ndElem, QilNode ndContent)
		{
			this.parentInfo = XmlILConstructInfo.Write(ndElem);
			this.parentInfo.MightHaveNamespacesAfterAttributes = false;
			this.parentInfo.MightHaveAttributes = false;
			this.parentInfo.MightHaveDuplicateAttributes = false;
			this.parentInfo.MightHaveNamespaces = !this.parentInfo.IsNamespaceInScope;
			this.dupAttrs.Clear();
			return base.Analyze(ndElem, ndContent);
		}

		protected override void AnalyzeLoop(QilLoop ndLoop, XmlILConstructInfo info)
		{
			if (ndLoop.XmlType.MaybeMany)
			{
				this.CheckAttributeNamespaceConstruct(ndLoop.XmlType);
			}
			base.AnalyzeLoop(ndLoop, info);
		}

		protected override void AnalyzeCopy(QilNode ndCopy, XmlILConstructInfo info)
		{
			if (ndCopy.NodeType == QilNodeType.AttributeCtor)
			{
				this.AnalyzeAttributeCtor(ndCopy as QilBinary, info);
			}
			else
			{
				this.CheckAttributeNamespaceConstruct(ndCopy.XmlType);
			}
			base.AnalyzeCopy(ndCopy, info);
		}

		private void AnalyzeAttributeCtor(QilBinary ndAttr, XmlILConstructInfo info)
		{
			if (ndAttr.Left.NodeType == QilNodeType.LiteralQName)
			{
				QilName qilName = ndAttr.Left as QilName;
				this.parentInfo.MightHaveAttributes = true;
				if (!this.parentInfo.MightHaveDuplicateAttributes)
				{
					XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(this.attrNames.Add(qilName.LocalName), this.attrNames.Add(qilName.NamespaceUri));
					int i;
					for (i = 0; i < this.dupAttrs.Count; i++)
					{
						XmlQualifiedName xmlQualifiedName2 = (XmlQualifiedName)this.dupAttrs[i];
						if (xmlQualifiedName2.Name == xmlQualifiedName.Name && xmlQualifiedName2.Namespace == xmlQualifiedName.Namespace)
						{
							this.parentInfo.MightHaveDuplicateAttributes = true;
						}
					}
					if (i >= this.dupAttrs.Count)
					{
						this.dupAttrs.Add(xmlQualifiedName);
					}
				}
				if (!info.IsNamespaceInScope)
				{
					this.parentInfo.MightHaveNamespaces = true;
					return;
				}
			}
			else
			{
				this.CheckAttributeNamespaceConstruct(ndAttr.XmlType);
			}
		}

		private void CheckAttributeNamespaceConstruct(XmlQueryType typ)
		{
			if ((typ.NodeKinds & XmlNodeKindFlags.Attribute) != XmlNodeKindFlags.None)
			{
				this.parentInfo.MightHaveAttributes = true;
				this.parentInfo.MightHaveDuplicateAttributes = true;
				this.parentInfo.MightHaveNamespaces = true;
			}
			if ((typ.NodeKinds & XmlNodeKindFlags.Namespace) != XmlNodeKindFlags.None)
			{
				this.parentInfo.MightHaveNamespaces = true;
				if (this.parentInfo.MightHaveAttributes)
				{
					this.parentInfo.MightHaveNamespacesAfterAttributes = true;
				}
			}
		}

		private NameTable attrNames = new NameTable();

		private ArrayList dupAttrs = new ArrayList();
	}
}
