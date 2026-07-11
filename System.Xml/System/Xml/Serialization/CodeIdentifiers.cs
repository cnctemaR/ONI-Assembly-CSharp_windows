using System;
using System.Collections;
using System.Globalization;

namespace System.Xml.Serialization
{
	public class CodeIdentifiers
	{
		public CodeIdentifiers()
			: this(true)
		{
		}

		public CodeIdentifiers(bool caseSensitive)
		{
			if (caseSensitive)
			{
				this.identifiers = new Hashtable();
				this.reservedIdentifiers = new Hashtable();
			}
			else
			{
				IEqualityComparer equalityComparer = new CaseInsensitiveKeyComparer();
				this.identifiers = new Hashtable(equalityComparer);
				this.reservedIdentifiers = new Hashtable(equalityComparer);
			}
			this.list = new ArrayList();
		}

		public void Clear()
		{
			this.identifiers.Clear();
			this.list.Clear();
		}

		public bool UseCamelCasing
		{
			get
			{
				return this.camelCase;
			}
			set
			{
				this.camelCase = value;
			}
		}

		public string MakeRightCase(string identifier)
		{
			if (this.camelCase)
			{
				return CodeIdentifier.MakeCamel(identifier);
			}
			return CodeIdentifier.MakePascal(identifier);
		}

		public string MakeUnique(string identifier)
		{
			if (this.IsInUse(identifier))
			{
				int num = 1;
				string text;
				for (;;)
				{
					text = identifier + num.ToString(CultureInfo.InvariantCulture);
					if (!this.IsInUse(text))
					{
						break;
					}
					num++;
				}
				identifier = text;
			}
			if (identifier.Length > 511)
			{
				return this.MakeUnique("Item");
			}
			return identifier;
		}

		public void AddReserved(string identifier)
		{
			this.reservedIdentifiers.Add(identifier, identifier);
		}

		public void RemoveReserved(string identifier)
		{
			this.reservedIdentifiers.Remove(identifier);
		}

		public string AddUnique(string identifier, object value)
		{
			identifier = this.MakeUnique(identifier);
			this.Add(identifier, value);
			return identifier;
		}

		public bool IsInUse(string identifier)
		{
			return this.identifiers.Contains(identifier) || this.reservedIdentifiers.Contains(identifier);
		}

		public void Add(string identifier, object value)
		{
			this.identifiers.Add(identifier, value);
			this.list.Add(value);
		}

		public void Remove(string identifier)
		{
			this.list.Remove(this.identifiers[identifier]);
			this.identifiers.Remove(identifier);
		}

		public object ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, this.list.Count);
			this.list.CopyTo(array, 0);
			return array;
		}

		internal CodeIdentifiers Clone()
		{
			return new CodeIdentifiers
			{
				identifiers = (Hashtable)this.identifiers.Clone(),
				reservedIdentifiers = (Hashtable)this.reservedIdentifiers.Clone(),
				list = (ArrayList)this.list.Clone(),
				camelCase = this.camelCase
			};
		}

		private Hashtable identifiers;

		private Hashtable reservedIdentifiers;

		private ArrayList list;

		private bool camelCase;
	}
}
