using System;
using System.IO;

namespace System.Resources
{
	internal abstract class Win32Resource
	{
		internal Win32Resource(NameOrId type, NameOrId name, int language)
		{
			this.type = type;
			this.name = name;
			this.language = language;
		}

		internal Win32Resource(Win32ResourceType type, int name, int language)
		{
			this.type = new NameOrId((int)type);
			this.name = new NameOrId(name);
			this.language = language;
		}

		public Win32ResourceType ResourceType
		{
			get
			{
				if (this.type.IsName)
				{
					return (Win32ResourceType)(-1);
				}
				return (Win32ResourceType)this.type.Id;
			}
		}

		public NameOrId Name
		{
			get
			{
				return this.name;
			}
		}

		public NameOrId Type
		{
			get
			{
				return this.type;
			}
		}

		public int Language
		{
			get
			{
				return this.language;
			}
		}

		public abstract void WriteTo(Stream s);

		public override string ToString()
		{
			string[] array = new string[5];
			array[0] = "Win32Resource (Kind=";
			array[1] = this.ResourceType.ToString();
			array[2] = ", Name=";
			int num = 3;
			NameOrId nameOrId = this.name;
			array[num] = ((nameOrId != null) ? nameOrId.ToString() : null);
			array[4] = ")";
			return string.Concat(array);
		}

		private NameOrId type;

		private NameOrId name;

		private int language;
	}
}
