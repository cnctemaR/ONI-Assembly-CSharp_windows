using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal sealed class CompilerScopeManager<V>
	{
		public CompilerScopeManager()
		{
			this.records[0].flags = CompilerScopeManager<V>.ScopeFlags.NsDecl;
			this.records[0].ncName = "xml";
			this.records[0].nsUri = "http://www.w3.org/XML/1998/namespace";
		}

		public CompilerScopeManager(KeywordsTable atoms)
		{
			this.records[0].flags = CompilerScopeManager<V>.ScopeFlags.NsDecl;
			this.records[0].ncName = atoms.Xml;
			this.records[0].nsUri = atoms.UriXml;
		}

		public void EnterScope()
		{
			this.lastScopes++;
		}

		public void ExitScope()
		{
			if (0 < this.lastScopes)
			{
				this.lastScopes--;
				return;
			}
			CompilerScopeManager<V>.ScopeRecord[] array;
			int num;
			do
			{
				array = this.records;
				num = this.lastRecord - 1;
				this.lastRecord = num;
			}
			while (array[num].scopeCount == 0);
			this.lastScopes = this.records[this.lastRecord].scopeCount;
			this.lastScopes--;
		}

		[Conditional("DEBUG")]
		public void CheckEmpty()
		{
			this.ExitScope();
		}

		public bool EnterScope(NsDecl nsDecl)
		{
			this.lastScopes++;
			bool flag = false;
			bool flag2 = false;
			while (nsDecl != null)
			{
				if (nsDecl.NsUri == null)
				{
					flag2 = true;
				}
				else if (nsDecl.Prefix == null)
				{
					this.AddExNamespace(nsDecl.NsUri);
				}
				else
				{
					flag = true;
					this.AddNsDeclaration(nsDecl.Prefix, nsDecl.NsUri);
				}
				nsDecl = nsDecl.Prev;
			}
			if (flag2)
			{
				this.AddExNamespace(null);
			}
			return flag;
		}

		private void AddRecord()
		{
			this.records[this.lastRecord].scopeCount = this.lastScopes;
			int num = this.lastRecord + 1;
			this.lastRecord = num;
			if (num == this.records.Length)
			{
				CompilerScopeManager<V>.ScopeRecord[] array = new CompilerScopeManager<V>.ScopeRecord[this.lastRecord * 2];
				Array.Copy(this.records, 0, array, 0, this.lastRecord);
				this.records = array;
			}
			this.lastScopes = 0;
		}

		private void AddRecord(CompilerScopeManager<V>.ScopeFlags flag, string ncName, string uri, V value)
		{
			CompilerScopeManager<V>.ScopeFlags scopeFlags = this.records[this.lastRecord].flags;
			if (this.lastScopes != 0 || (scopeFlags & CompilerScopeManager<V>.ScopeFlags.ExclusiveFlags) != (CompilerScopeManager<V>.ScopeFlags)0)
			{
				this.AddRecord();
				scopeFlags &= CompilerScopeManager<V>.ScopeFlags.InheritedFlags;
			}
			this.records[this.lastRecord].flags = scopeFlags | flag;
			this.records[this.lastRecord].ncName = ncName;
			this.records[this.lastRecord].nsUri = uri;
			this.records[this.lastRecord].value = value;
		}

		private void SetFlag(CompilerScopeManager<V>.ScopeFlags flag, bool value)
		{
			CompilerScopeManager<V>.ScopeFlags scopeFlags = this.records[this.lastRecord].flags;
			if ((scopeFlags & flag) > (CompilerScopeManager<V>.ScopeFlags)0 != value)
			{
				if (this.lastScopes != 0)
				{
					this.AddRecord();
					scopeFlags &= CompilerScopeManager<V>.ScopeFlags.InheritedFlags;
				}
				if (flag == CompilerScopeManager<V>.ScopeFlags.CanHaveApplyImports)
				{
					scopeFlags ^= flag;
				}
				else
				{
					scopeFlags &= (CompilerScopeManager<V>.ScopeFlags)(-4);
					if (value)
					{
						scopeFlags |= flag;
					}
				}
				this.records[this.lastRecord].flags = scopeFlags;
			}
		}

		public void AddVariable(QilName varName, V value)
		{
			this.AddRecord(CompilerScopeManager<V>.ScopeFlags.Variable, varName.LocalName, varName.NamespaceUri, value);
		}

		private string LookupNamespace(string prefix, int from, int to)
		{
			int num = from;
			while (to <= num)
			{
				string text;
				string text2;
				if ((CompilerScopeManager<V>.GetName(ref this.records[num], out text, out text2) & CompilerScopeManager<V>.ScopeFlags.NsDecl) != (CompilerScopeManager<V>.ScopeFlags)0 && text == prefix)
				{
					return text2;
				}
				num--;
			}
			return null;
		}

		public string LookupNamespace(string prefix)
		{
			return this.LookupNamespace(prefix, this.lastRecord, 0);
		}

		private static CompilerScopeManager<V>.ScopeFlags GetName(ref CompilerScopeManager<V>.ScopeRecord re, out string prefix, out string nsUri)
		{
			prefix = re.ncName;
			nsUri = re.nsUri;
			return re.flags;
		}

		public void AddNsDeclaration(string prefix, string nsUri)
		{
			this.AddRecord(CompilerScopeManager<V>.ScopeFlags.NsDecl, prefix, nsUri, default(V));
		}

		public void AddExNamespace(string nsUri)
		{
			this.AddRecord(CompilerScopeManager<V>.ScopeFlags.NsExcl, null, nsUri, default(V));
		}

		public bool IsExNamespace(string nsUri)
		{
			int num = 0;
			int num2 = this.lastRecord;
			while (0 <= num2)
			{
				string text;
				string text2;
				CompilerScopeManager<V>.ScopeFlags name = CompilerScopeManager<V>.GetName(ref this.records[num2], out text, out text2);
				if ((name & CompilerScopeManager<V>.ScopeFlags.NsExcl) != (CompilerScopeManager<V>.ScopeFlags)0)
				{
					if (text2 == nsUri)
					{
						return true;
					}
					if (text2 == null)
					{
						num = num2;
					}
				}
				else if (num != 0 && (name & CompilerScopeManager<V>.ScopeFlags.NsDecl) != (CompilerScopeManager<V>.ScopeFlags)0 && text2 == nsUri)
				{
					bool flag = false;
					for (int i = num2 + 1; i < num; i++)
					{
						string text3;
						string text4;
						CompilerScopeManager<V>.GetName(ref this.records[i], out text3, out text4);
						if ((name & CompilerScopeManager<V>.ScopeFlags.NsDecl) != (CompilerScopeManager<V>.ScopeFlags)0 && text3 == text)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return true;
					}
				}
				num2--;
			}
			return false;
		}

		private int SearchVariable(string localName, string uri)
		{
			int num = this.lastRecord;
			while (0 <= num)
			{
				string text;
				string text2;
				if ((CompilerScopeManager<V>.GetName(ref this.records[num], out text, out text2) & CompilerScopeManager<V>.ScopeFlags.Variable) != (CompilerScopeManager<V>.ScopeFlags)0 && text == localName && text2 == uri)
				{
					return num;
				}
				num--;
			}
			return -1;
		}

		public V LookupVariable(string localName, string uri)
		{
			int num = this.SearchVariable(localName, uri);
			if (num >= 0)
			{
				return this.records[num].value;
			}
			return default(V);
		}

		public bool IsLocalVariable(string localName, string uri)
		{
			int num = this.SearchVariable(localName, uri);
			while (0 <= --num)
			{
				if (this.records[num].scopeCount != 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool ForwardCompatibility
		{
			get
			{
				return (this.records[this.lastRecord].flags & CompilerScopeManager<V>.ScopeFlags.ForwardCompatibility) > (CompilerScopeManager<V>.ScopeFlags)0;
			}
			set
			{
				this.SetFlag(CompilerScopeManager<V>.ScopeFlags.ForwardCompatibility, value);
			}
		}

		public bool BackwardCompatibility
		{
			get
			{
				return (this.records[this.lastRecord].flags & CompilerScopeManager<V>.ScopeFlags.BackwardCompatibility) > (CompilerScopeManager<V>.ScopeFlags)0;
			}
			set
			{
				this.SetFlag(CompilerScopeManager<V>.ScopeFlags.BackwardCompatibility, value);
			}
		}

		public bool CanHaveApplyImports
		{
			get
			{
				return (this.records[this.lastRecord].flags & CompilerScopeManager<V>.ScopeFlags.CanHaveApplyImports) > (CompilerScopeManager<V>.ScopeFlags)0;
			}
			set
			{
				this.SetFlag(CompilerScopeManager<V>.ScopeFlags.CanHaveApplyImports, value);
			}
		}

		internal IEnumerable<CompilerScopeManager<V>.ScopeRecord> GetActiveRecords()
		{
			int currentRecord = this.lastRecord + 1;
			for (;;)
			{
				int num = 0;
				int num2 = currentRecord - 1;
				currentRecord = num2;
				if (num >= num2)
				{
					break;
				}
				if (!this.records[currentRecord].IsNamespace || this.LookupNamespace(this.records[currentRecord].ncName, this.lastRecord, currentRecord + 1) == null)
				{
					yield return this.records[currentRecord];
				}
			}
			yield break;
		}

		public CompilerScopeManager<V>.NamespaceEnumerator GetEnumerator()
		{
			return new CompilerScopeManager<V>.NamespaceEnumerator(this);
		}

		private const int LastPredefRecord = 0;

		private CompilerScopeManager<V>.ScopeRecord[] records = new CompilerScopeManager<V>.ScopeRecord[32];

		private int lastRecord;

		private int lastScopes;

		public enum ScopeFlags
		{
			BackwardCompatibility = 1,
			ForwardCompatibility,
			CanHaveApplyImports = 4,
			NsDecl = 16,
			NsExcl = 32,
			Variable = 64,
			CompatibilityFlags = 3,
			InheritedFlags = 7,
			ExclusiveFlags = 112
		}

		public struct ScopeRecord
		{
			public bool IsVariable
			{
				get
				{
					return (this.flags & CompilerScopeManager<V>.ScopeFlags.Variable) > (CompilerScopeManager<V>.ScopeFlags)0;
				}
			}

			public bool IsNamespace
			{
				get
				{
					return (this.flags & CompilerScopeManager<V>.ScopeFlags.NsDecl) > (CompilerScopeManager<V>.ScopeFlags)0;
				}
			}

			public int scopeCount;

			public CompilerScopeManager<V>.ScopeFlags flags;

			public string ncName;

			public string nsUri;

			public V value;
		}

		internal struct NamespaceEnumerator
		{
			public NamespaceEnumerator(CompilerScopeManager<V> scope)
			{
				this.scope = scope;
				this.lastRecord = scope.lastRecord;
				this.currentRecord = this.lastRecord + 1;
			}

			public void Reset()
			{
				this.currentRecord = this.lastRecord + 1;
			}

			public bool MoveNext()
			{
				do
				{
					int num = 0;
					int num2 = this.currentRecord - 1;
					this.currentRecord = num2;
					if (num >= num2)
					{
						return false;
					}
				}
				while (!this.scope.records[this.currentRecord].IsNamespace || this.scope.LookupNamespace(this.scope.records[this.currentRecord].ncName, this.lastRecord, this.currentRecord + 1) != null);
				return true;
			}

			public CompilerScopeManager<V>.ScopeRecord Current
			{
				get
				{
					return this.scope.records[this.currentRecord];
				}
			}

			private CompilerScopeManager<V> scope;

			private int lastRecord;

			private int currentRecord;
		}
	}
}
