using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;

namespace I18N.Common
{
	public class Manager
	{
		private Manager()
		{
			this.handlers = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			this.active = new Hashtable(16);
			this.assemblies = new Hashtable(8);
			this.LoadClassList();
		}

		public static Manager PrimaryManager
		{
			get
			{
				object obj = Manager.lockobj;
				Manager manager;
				lock (obj)
				{
					if (Manager.manager == null)
					{
						Manager.manager = new Manager();
					}
					manager = Manager.manager;
				}
				return manager;
			}
		}

		private static string Normalize(string name)
		{
			return name.ToLower(CultureInfo.InvariantCulture).Replace('-', '_');
		}

		public Encoding GetEncoding(int codePage)
		{
			return this.Instantiate("CP" + codePage.ToString()) as Encoding;
		}

		public Encoding GetEncoding(string name)
		{
			if (name == null)
			{
				return null;
			}
			string text = name;
			name = Manager.Normalize(name);
			Encoding encoding = this.Instantiate("ENC" + name) as Encoding;
			if (encoding == null)
			{
				encoding = this.Instantiate(name) as Encoding;
			}
			if (encoding == null)
			{
				string alias = Handlers.GetAlias(name);
				if (alias != null)
				{
					encoding = this.Instantiate("ENC" + alias) as Encoding;
					if (encoding == null)
					{
						encoding = this.Instantiate(alias) as Encoding;
					}
				}
			}
			if (encoding == null)
			{
				return null;
			}
			if (text.IndexOf('_') > 0 && encoding.WebName.IndexOf('-') > 0)
			{
				return null;
			}
			if (text.IndexOf('-') > 0 && encoding.WebName.IndexOf('_') > 0)
			{
				return null;
			}
			return encoding;
		}

		public CultureInfo GetCulture(int culture, bool useUserOverride)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("0123456789abcdef".get_Chars((culture >> 12) & 15));
			stringBuilder.Append("0123456789abcdef".get_Chars((culture >> 8) & 15));
			stringBuilder.Append("0123456789abcdef".get_Chars((culture >> 4) & 15));
			stringBuilder.Append("0123456789abcdef".get_Chars(culture & 15));
			string text = stringBuilder.ToString();
			if (useUserOverride)
			{
				object obj = this.Instantiate("CIDO" + text);
				if (obj != null)
				{
					return obj as CultureInfo;
				}
			}
			return this.Instantiate("CID" + text) as CultureInfo;
		}

		public CultureInfo GetCulture(string name, bool useUserOverride)
		{
			if (name == null)
			{
				return null;
			}
			name = Manager.Normalize(name);
			if (useUserOverride)
			{
				object obj = this.Instantiate("CNO" + name.ToString());
				if (obj != null)
				{
					return obj as CultureInfo;
				}
			}
			return this.Instantiate("CN" + name.ToString()) as CultureInfo;
		}

		internal object Instantiate(string name)
		{
			object obj2;
			lock (this)
			{
				object obj = this.active[name];
				if (obj != null)
				{
					obj2 = obj;
				}
				else
				{
					string text = (string)this.handlers[name];
					if (text == null)
					{
						obj2 = null;
					}
					else
					{
						Assembly assembly = (Assembly)this.assemblies[text];
						if (assembly == null)
						{
							try
							{
								AssemblyName name2 = typeof(Manager).Assembly.GetName();
								name2.Name = text;
								assembly = Assembly.Load(name2);
							}
							catch (SystemException)
							{
								assembly = null;
							}
							if (assembly == null)
							{
								return null;
							}
							this.assemblies[text] = assembly;
						}
						Type type = assembly.GetType(text + "." + name, false, true);
						if (type == null)
						{
							obj2 = null;
						}
						else
						{
							try
							{
								obj = type.InvokeMember(string.Empty, 564, null, null, null, null, null, null);
							}
							catch (MissingMethodException)
							{
								return null;
							}
							catch (SecurityException)
							{
								return null;
							}
							this.active.Add(name, obj);
							obj2 = obj;
						}
					}
				}
			}
			return obj2;
		}

		private void LoadClassList()
		{
			FileStream file;
			try
			{
				file = Assembly.GetExecutingAssembly().GetFile("I18N-handlers.def");
				if (file == null)
				{
					this.LoadInternalClasses();
					return;
				}
			}
			catch (FileLoadException)
			{
				this.LoadInternalClasses();
				return;
			}
			StreamReader streamReader = new StreamReader(file);
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (text.Length != 0 && text.get_Chars(0) != '#')
				{
					int num = text.LastIndexOf('.');
					if (num != -1)
					{
						string text2 = text.Substring(num + 1);
						if (!this.handlers.Contains(text2))
						{
							this.handlers.Add(text2, text.Substring(0, num));
						}
					}
				}
			}
			streamReader.Close();
		}

		private void LoadInternalClasses()
		{
			foreach (string text in Handlers.List)
			{
				int num = text.LastIndexOf('.');
				if (num != -1)
				{
					string text2 = text.Substring(num + 1);
					if (!this.handlers.Contains(text2))
					{
						this.handlers.Add(text2, text.Substring(0, num));
					}
				}
			}
		}

		private const string hex = "0123456789abcdef";

		private static Manager manager;

		private Hashtable handlers;

		private Hashtable active;

		private Hashtable assemblies;

		private static readonly object lockobj = new object();
	}
}
