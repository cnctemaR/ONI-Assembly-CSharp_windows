using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Runtime
{
	internal class WhitespaceRuleLookup
	{
		public WhitespaceRuleLookup()
		{
			this.qnames = new Hashtable();
			this.wildcards = new ArrayList();
		}

		public WhitespaceRuleLookup(IList<WhitespaceRule> rules)
			: this()
		{
			for (int i = rules.Count - 1; i >= 0; i--)
			{
				WhitespaceRule whitespaceRule = rules[i];
				WhitespaceRuleLookup.InternalWhitespaceRule internalWhitespaceRule = new WhitespaceRuleLookup.InternalWhitespaceRule(whitespaceRule.LocalName, whitespaceRule.NamespaceName, whitespaceRule.PreserveSpace, -i);
				if (whitespaceRule.LocalName == null || whitespaceRule.NamespaceName == null)
				{
					this.wildcards.Add(internalWhitespaceRule);
				}
				else
				{
					this.qnames[internalWhitespaceRule] = internalWhitespaceRule;
				}
			}
			this.ruleTemp = new WhitespaceRuleLookup.InternalWhitespaceRule();
		}

		public void Atomize(XmlNameTable nameTable)
		{
			if (nameTable != this.nameTable)
			{
				this.nameTable = nameTable;
				foreach (object obj in this.qnames.Values)
				{
					((WhitespaceRuleLookup.InternalWhitespaceRule)obj).Atomize(nameTable);
				}
				foreach (object obj2 in this.wildcards)
				{
					((WhitespaceRuleLookup.InternalWhitespaceRule)obj2).Atomize(nameTable);
				}
			}
		}

		public bool ShouldStripSpace(string localName, string namespaceName)
		{
			this.ruleTemp.Init(localName, namespaceName, false, 0);
			WhitespaceRuleLookup.InternalWhitespaceRule internalWhitespaceRule = this.qnames[this.ruleTemp] as WhitespaceRuleLookup.InternalWhitespaceRule;
			int count = this.wildcards.Count;
			while (count-- != 0)
			{
				WhitespaceRuleLookup.InternalWhitespaceRule internalWhitespaceRule2 = this.wildcards[count] as WhitespaceRuleLookup.InternalWhitespaceRule;
				if (internalWhitespaceRule != null)
				{
					if (internalWhitespaceRule.Priority > internalWhitespaceRule2.Priority)
					{
						return !internalWhitespaceRule.PreserveSpace;
					}
					if (internalWhitespaceRule.PreserveSpace == internalWhitespaceRule2.PreserveSpace)
					{
						continue;
					}
				}
				if ((internalWhitespaceRule2.LocalName == null || internalWhitespaceRule2.LocalName == localName) && (internalWhitespaceRule2.NamespaceName == null || internalWhitespaceRule2.NamespaceName == namespaceName))
				{
					return !internalWhitespaceRule2.PreserveSpace;
				}
			}
			return internalWhitespaceRule != null && !internalWhitespaceRule.PreserveSpace;
		}

		private Hashtable qnames;

		private ArrayList wildcards;

		private WhitespaceRuleLookup.InternalWhitespaceRule ruleTemp;

		private XmlNameTable nameTable;

		private class InternalWhitespaceRule : WhitespaceRule
		{
			public InternalWhitespaceRule()
			{
			}

			public InternalWhitespaceRule(string localName, string namespaceName, bool preserveSpace, int priority)
			{
				this.Init(localName, namespaceName, preserveSpace, priority);
			}

			public void Init(string localName, string namespaceName, bool preserveSpace, int priority)
			{
				base.Init(localName, namespaceName, preserveSpace);
				this.priority = priority;
				if (localName != null && namespaceName != null)
				{
					this.hashCode = localName.GetHashCode();
				}
			}

			public void Atomize(XmlNameTable nameTable)
			{
				if (base.LocalName != null)
				{
					base.LocalName = nameTable.Add(base.LocalName);
				}
				if (base.NamespaceName != null)
				{
					base.NamespaceName = nameTable.Add(base.NamespaceName);
				}
			}

			public int Priority
			{
				get
				{
					return this.priority;
				}
			}

			public override int GetHashCode()
			{
				return this.hashCode;
			}

			public override bool Equals(object obj)
			{
				WhitespaceRuleLookup.InternalWhitespaceRule internalWhitespaceRule = obj as WhitespaceRuleLookup.InternalWhitespaceRule;
				return base.LocalName == internalWhitespaceRule.LocalName && base.NamespaceName == internalWhitespaceRule.NamespaceName;
			}

			private int priority;

			private int hashCode;
		}
	}
}
