using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Xml.XPath;
using System.Xml.Xsl.Runtime;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class RootAction : TemplateBaseAction
	{
		internal XsltOutput Output
		{
			get
			{
				if (this.output == null)
				{
					this.output = new XsltOutput();
				}
				return this.output;
			}
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileDocument(compiler, false);
		}

		internal void InsertKey(XmlQualifiedName name, int MatchKey, int UseKey)
		{
			if (this.keyList == null)
			{
				this.keyList = new List<Key>();
			}
			this.keyList.Add(new Key(name, MatchKey, UseKey));
		}

		internal AttributeSetAction GetAttributeSet(XmlQualifiedName name)
		{
			AttributeSetAction attributeSetAction = (AttributeSetAction)this.attributeSetTable[name];
			if (attributeSetAction == null)
			{
				throw XsltException.Create("A reference to attribute set '{0}' cannot be resolved. An 'xsl:attribute-set' of this name must be declared at the top level of the stylesheet.", new string[] { name.ToString() });
			}
			return attributeSetAction;
		}

		public void PorcessAttributeSets(Stylesheet rootStylesheet)
		{
			this.MirgeAttributeSets(rootStylesheet);
			foreach (object obj in this.attributeSetTable.Values)
			{
				AttributeSetAction attributeSetAction = (AttributeSetAction)obj;
				if (attributeSetAction.containedActions != null)
				{
					attributeSetAction.containedActions.Reverse();
				}
			}
			this.CheckAttributeSets_RecurceInList(new Hashtable(), this.attributeSetTable.Keys);
		}

		private void MirgeAttributeSets(Stylesheet stylesheet)
		{
			if (stylesheet.AttributeSetTable != null)
			{
				foreach (object obj in stylesheet.AttributeSetTable.Values)
				{
					AttributeSetAction attributeSetAction = (AttributeSetAction)obj;
					ArrayList containedActions = attributeSetAction.containedActions;
					AttributeSetAction attributeSetAction2 = (AttributeSetAction)this.attributeSetTable[attributeSetAction.Name];
					if (attributeSetAction2 == null)
					{
						attributeSetAction2 = new AttributeSetAction();
						attributeSetAction2.name = attributeSetAction.Name;
						attributeSetAction2.containedActions = new ArrayList();
						this.attributeSetTable[attributeSetAction.Name] = attributeSetAction2;
					}
					ArrayList containedActions2 = attributeSetAction2.containedActions;
					if (containedActions != null)
					{
						int num = containedActions.Count - 1;
						while (0 <= num)
						{
							containedActions2.Add(containedActions[num]);
							num--;
						}
					}
				}
			}
			foreach (object obj2 in stylesheet.Imports)
			{
				Stylesheet stylesheet2 = (Stylesheet)obj2;
				this.MirgeAttributeSets(stylesheet2);
			}
		}

		private void CheckAttributeSets_RecurceInList(Hashtable markTable, ICollection setQNames)
		{
			foreach (object obj in setQNames)
			{
				XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)obj;
				object obj2 = markTable[xmlQualifiedName];
				if (obj2 == "P")
				{
					throw XsltException.Create("Circular reference in the definition of attribute set '{0}'.", new string[] { xmlQualifiedName.ToString() });
				}
				if (obj2 != "D")
				{
					markTable[xmlQualifiedName] = "P";
					this.CheckAttributeSets_RecurceInContainer(markTable, this.GetAttributeSet(xmlQualifiedName));
					markTable[xmlQualifiedName] = "D";
				}
			}
		}

		private void CheckAttributeSets_RecurceInContainer(Hashtable markTable, ContainerAction container)
		{
			if (container.containedActions == null)
			{
				return;
			}
			foreach (object obj in container.containedActions)
			{
				Action action = (Action)obj;
				if (action is UseAttributeSetsAction)
				{
					this.CheckAttributeSets_RecurceInList(markTable, ((UseAttributeSetsAction)action).UsedSets);
				}
				else if (action is ContainerAction)
				{
					this.CheckAttributeSets_RecurceInContainer(markTable, (ContainerAction)action);
				}
			}
		}

		internal void AddDecimalFormat(XmlQualifiedName name, DecimalFormat formatinfo)
		{
			DecimalFormat decimalFormat = (DecimalFormat)this.decimalFormatTable[name];
			if (decimalFormat != null)
			{
				NumberFormatInfo info = decimalFormat.info;
				NumberFormatInfo info2 = formatinfo.info;
				if (info.NumberDecimalSeparator != info2.NumberDecimalSeparator || info.NumberGroupSeparator != info2.NumberGroupSeparator || info.PositiveInfinitySymbol != info2.PositiveInfinitySymbol || info.NegativeSign != info2.NegativeSign || info.NaNSymbol != info2.NaNSymbol || info.PercentSymbol != info2.PercentSymbol || info.PerMilleSymbol != info2.PerMilleSymbol || decimalFormat.zeroDigit != formatinfo.zeroDigit || decimalFormat.digit != formatinfo.digit || decimalFormat.patternSeparator != formatinfo.patternSeparator)
				{
					throw XsltException.Create("Decimal format '{0}' has a duplicate declaration.", new string[] { name.ToString() });
				}
			}
			this.decimalFormatTable[name] = formatinfo;
		}

		internal DecimalFormat GetDecimalFormat(XmlQualifiedName name)
		{
			return this.decimalFormatTable[name] as DecimalFormat;
		}

		internal List<Key> KeyList
		{
			get
			{
				return this.keyList;
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			switch (frame.State)
			{
			case 0:
			{
				frame.AllocateVariables(this.variableCount);
				XPathNavigator xpathNavigator = processor.Document.Clone();
				xpathNavigator.MoveToRoot();
				frame.InitNodeSet(new XPathSingletonIterator(xpathNavigator));
				if (this.containedActions != null && this.containedActions.Count > 0)
				{
					processor.PushActionFrame(frame);
				}
				frame.State = 2;
				return;
			}
			case 1:
				break;
			case 2:
				frame.NextNode(processor);
				if (processor.Debugger != null)
				{
					processor.PopDebuggerStack();
				}
				processor.PushTemplateLookup(frame.NodeSet, null, null);
				frame.State = 3;
				return;
			case 3:
				frame.Finished();
				break;
			default:
				return;
			}
		}

		private const int QueryInitialized = 2;

		private const int RootProcessed = 3;

		private Hashtable attributeSetTable = new Hashtable();

		private Hashtable decimalFormatTable = new Hashtable();

		private List<Key> keyList;

		private XsltOutput output;

		public Stylesheet builtInSheet;

		public PermissionSet permissions;
	}
}
