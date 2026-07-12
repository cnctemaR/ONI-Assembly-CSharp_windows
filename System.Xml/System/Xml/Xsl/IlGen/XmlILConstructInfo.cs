using System;
using System.Collections;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.IlGen
{
	internal class XmlILConstructInfo : IQilAnnotation
	{
		public static XmlILConstructInfo Read(QilNode nd)
		{
			XmlILAnnotation xmlILAnnotation = nd.Annotation as XmlILAnnotation;
			XmlILConstructInfo xmlILConstructInfo = ((xmlILAnnotation != null) ? xmlILAnnotation.ConstructInfo : null);
			if (xmlILConstructInfo == null)
			{
				if (XmlILConstructInfo.Default == null)
				{
					xmlILConstructInfo = new XmlILConstructInfo(QilNodeType.Unknown);
					xmlILConstructInfo.isReadOnly = true;
					XmlILConstructInfo.Default = xmlILConstructInfo;
				}
				else
				{
					xmlILConstructInfo = XmlILConstructInfo.Default;
				}
			}
			return xmlILConstructInfo;
		}

		public static XmlILConstructInfo Write(QilNode nd)
		{
			XmlILAnnotation xmlILAnnotation = XmlILAnnotation.Write(nd);
			XmlILConstructInfo xmlILConstructInfo = xmlILAnnotation.ConstructInfo;
			if (xmlILConstructInfo == null || xmlILConstructInfo.isReadOnly)
			{
				xmlILConstructInfo = new XmlILConstructInfo(nd.NodeType);
				xmlILAnnotation.ConstructInfo = xmlILConstructInfo;
			}
			return xmlILConstructInfo;
		}

		private XmlILConstructInfo(QilNodeType nodeType)
		{
			this.nodeType = nodeType;
			this.xstatesInitial = (this.xstatesFinal = PossibleXmlStates.Any);
			this.xstatesBeginLoop = (this.xstatesEndLoop = PossibleXmlStates.None);
			this.isNmspInScope = false;
			this.mightHaveNmsp = true;
			this.mightHaveAttrs = true;
			this.mightHaveDupAttrs = true;
			this.mightHaveNmspAfterAttrs = true;
			this.constrMeth = XmlILConstructMethod.Iterator;
			this.parentInfo = null;
		}

		public PossibleXmlStates InitialStates
		{
			get
			{
				return this.xstatesInitial;
			}
			set
			{
				this.xstatesInitial = value;
			}
		}

		public PossibleXmlStates FinalStates
		{
			get
			{
				return this.xstatesFinal;
			}
			set
			{
				this.xstatesFinal = value;
			}
		}

		public PossibleXmlStates BeginLoopStates
		{
			set
			{
				this.xstatesBeginLoop = value;
			}
		}

		public PossibleXmlStates EndLoopStates
		{
			set
			{
				this.xstatesEndLoop = value;
			}
		}

		public XmlILConstructMethod ConstructMethod
		{
			get
			{
				return this.constrMeth;
			}
			set
			{
				this.constrMeth = value;
			}
		}

		public bool PushToWriterFirst
		{
			get
			{
				return this.constrMeth == XmlILConstructMethod.Writer || this.constrMeth == XmlILConstructMethod.WriterThenIterator;
			}
			set
			{
				XmlILConstructMethod xmlILConstructMethod = this.constrMeth;
				if (xmlILConstructMethod == XmlILConstructMethod.Iterator)
				{
					this.constrMeth = XmlILConstructMethod.WriterThenIterator;
					return;
				}
				if (xmlILConstructMethod != XmlILConstructMethod.IteratorThenWriter)
				{
					return;
				}
				this.constrMeth = XmlILConstructMethod.Writer;
			}
		}

		public bool PushToWriterLast
		{
			get
			{
				return this.constrMeth == XmlILConstructMethod.Writer || this.constrMeth == XmlILConstructMethod.IteratorThenWriter;
			}
			set
			{
				XmlILConstructMethod xmlILConstructMethod = this.constrMeth;
				if (xmlILConstructMethod == XmlILConstructMethod.Iterator)
				{
					this.constrMeth = XmlILConstructMethod.IteratorThenWriter;
					return;
				}
				if (xmlILConstructMethod != XmlILConstructMethod.WriterThenIterator)
				{
					return;
				}
				this.constrMeth = XmlILConstructMethod.Writer;
			}
		}

		public bool PullFromIteratorFirst
		{
			get
			{
				return this.constrMeth == XmlILConstructMethod.IteratorThenWriter || this.constrMeth == XmlILConstructMethod.Iterator;
			}
			set
			{
				XmlILConstructMethod xmlILConstructMethod = this.constrMeth;
				if (xmlILConstructMethod == XmlILConstructMethod.Writer)
				{
					this.constrMeth = XmlILConstructMethod.IteratorThenWriter;
					return;
				}
				if (xmlILConstructMethod != XmlILConstructMethod.WriterThenIterator)
				{
					return;
				}
				this.constrMeth = XmlILConstructMethod.Iterator;
			}
		}

		public XmlILConstructInfo ParentInfo
		{
			set
			{
				this.parentInfo = value;
			}
		}

		public XmlILConstructInfo ParentElementInfo
		{
			get
			{
				if (this.parentInfo != null && this.parentInfo.nodeType == QilNodeType.ElementCtor)
				{
					return this.parentInfo;
				}
				return null;
			}
		}

		public bool IsNamespaceInScope
		{
			get
			{
				return this.isNmspInScope;
			}
			set
			{
				this.isNmspInScope = value;
			}
		}

		public bool MightHaveNamespaces
		{
			get
			{
				return this.mightHaveNmsp;
			}
			set
			{
				this.mightHaveNmsp = value;
			}
		}

		public bool MightHaveNamespacesAfterAttributes
		{
			get
			{
				return this.mightHaveNmspAfterAttrs;
			}
			set
			{
				this.mightHaveNmspAfterAttrs = value;
			}
		}

		public bool MightHaveAttributes
		{
			get
			{
				return this.mightHaveAttrs;
			}
			set
			{
				this.mightHaveAttrs = value;
			}
		}

		public bool MightHaveDuplicateAttributes
		{
			get
			{
				return this.mightHaveDupAttrs;
			}
			set
			{
				this.mightHaveDupAttrs = value;
			}
		}

		public ArrayList CallersInfo
		{
			get
			{
				if (this.callersInfo == null)
				{
					this.callersInfo = new ArrayList();
				}
				return this.callersInfo;
			}
		}

		public virtual string Name
		{
			get
			{
				return "ConstructInfo";
			}
		}

		public override string ToString()
		{
			string text = "";
			if (this.constrMeth != XmlILConstructMethod.Iterator)
			{
				text += this.constrMeth.ToString();
				text = text + ", " + this.xstatesInitial.ToString();
				if (this.xstatesBeginLoop != PossibleXmlStates.None)
				{
					text = string.Concat(new string[]
					{
						text,
						" => ",
						this.xstatesBeginLoop.ToString(),
						" => ",
						this.xstatesEndLoop.ToString()
					});
				}
				text = text + " => " + this.xstatesFinal.ToString();
				if (!this.MightHaveAttributes)
				{
					text += ", NoAttrs";
				}
				if (!this.MightHaveDuplicateAttributes)
				{
					text += ", NoDupAttrs";
				}
				if (!this.MightHaveNamespaces)
				{
					text += ", NoNmsp";
				}
				if (!this.MightHaveNamespacesAfterAttributes)
				{
					text += ", NoNmspAfterAttrs";
				}
			}
			return text;
		}

		private QilNodeType nodeType;

		private PossibleXmlStates xstatesInitial;

		private PossibleXmlStates xstatesFinal;

		private PossibleXmlStates xstatesBeginLoop;

		private PossibleXmlStates xstatesEndLoop;

		private bool isNmspInScope;

		private bool mightHaveNmsp;

		private bool mightHaveAttrs;

		private bool mightHaveDupAttrs;

		private bool mightHaveNmspAfterAttrs;

		private XmlILConstructMethod constrMeth;

		private XmlILConstructInfo parentInfo;

		private ArrayList callersInfo;

		private bool isReadOnly;

		private static volatile XmlILConstructInfo Default;
	}
}
