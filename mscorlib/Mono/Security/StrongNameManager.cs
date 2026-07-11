using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using Mono.Security.Cryptography;
using Mono.Xml;

namespace Mono.Security
{
	internal class StrongNameManager
	{
		public static void LoadConfig(string filename)
		{
			if (File.Exists(filename))
			{
				SecurityParser securityParser = new SecurityParser();
				using (StreamReader streamReader = new StreamReader(filename))
				{
					string text = streamReader.ReadToEnd();
					securityParser.LoadXml(text);
				}
				SecurityElement securityElement = securityParser.ToXml();
				if (securityElement != null && securityElement.Tag == "configuration")
				{
					SecurityElement securityElement2 = securityElement.SearchForChildByTag("strongNames");
					if (securityElement2 != null && securityElement2.Children.Count > 0)
					{
						SecurityElement securityElement3 = securityElement2.SearchForChildByTag("pubTokenMapping");
						if (securityElement3 != null && securityElement3.Children.Count > 0)
						{
							StrongNameManager.LoadMapping(securityElement3);
						}
						SecurityElement securityElement4 = securityElement2.SearchForChildByTag("verificationSettings");
						if (securityElement4 != null && securityElement4.Children.Count > 0)
						{
							StrongNameManager.LoadVerificationSettings(securityElement4);
						}
					}
				}
			}
		}

		private static void LoadMapping(SecurityElement mapping)
		{
			if (StrongNameManager.mappings == null)
			{
				StrongNameManager.mappings = new Hashtable();
			}
			object syncRoot = StrongNameManager.mappings.SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in mapping.Children)
				{
					SecurityElement securityElement = (SecurityElement)obj;
					if (!(securityElement.Tag != "map"))
					{
						string text = securityElement.Attribute("Token");
						if (text != null && text.Length == 16)
						{
							text = text.ToUpper(CultureInfo.InvariantCulture);
							string text2 = securityElement.Attribute("PublicKey");
							if (text2 != null)
							{
								if (StrongNameManager.mappings[text] == null)
								{
									StrongNameManager.mappings.Add(text, text2);
								}
								else
								{
									StrongNameManager.mappings[text] = text2;
								}
							}
						}
					}
				}
			}
		}

		private static void LoadVerificationSettings(SecurityElement settings)
		{
			if (StrongNameManager.tokens == null)
			{
				StrongNameManager.tokens = new Hashtable();
			}
			object syncRoot = StrongNameManager.tokens.SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in settings.Children)
				{
					SecurityElement securityElement = (SecurityElement)obj;
					if (!(securityElement.Tag != "skip"))
					{
						string text = securityElement.Attribute("Token");
						if (text != null)
						{
							text = text.ToUpper(CultureInfo.InvariantCulture);
							string text2 = securityElement.Attribute("Assembly");
							if (text2 == null)
							{
								text2 = "*";
							}
							string text3 = securityElement.Attribute("Users");
							if (text3 == null)
							{
								text3 = "*";
							}
							StrongNameManager.Element element = (StrongNameManager.Element)StrongNameManager.tokens[text];
							if (element == null)
							{
								element = new StrongNameManager.Element(text2, text3);
								StrongNameManager.tokens.Add(text, element);
							}
							else if ((string)element.assemblies[text2] == null)
							{
								element.assemblies.Add(text2, text3);
							}
							else if (text3 == "*")
							{
								element.assemblies[text2] = "*";
							}
							else
							{
								string text4 = (string)element.assemblies[text2] + "," + text3;
								element.assemblies[text2] = text4;
							}
						}
					}
				}
			}
		}

		public static byte[] GetMappedPublicKey(byte[] token)
		{
			if (StrongNameManager.mappings == null || token == null)
			{
				return null;
			}
			string text = CryptoConvert.ToHex(token);
			string text2 = (string)StrongNameManager.mappings[text];
			if (text2 == null)
			{
				return null;
			}
			return CryptoConvert.FromHex(text2);
		}

		public static bool MustVerify(AssemblyName an)
		{
			if (an == null || StrongNameManager.tokens == null)
			{
				return true;
			}
			string text = CryptoConvert.ToHex(an.GetPublicKeyToken());
			StrongNameManager.Element element = (StrongNameManager.Element)StrongNameManager.tokens[text];
			if (element != null)
			{
				string text2 = element.GetUsers(an.Name);
				if (text2 == null)
				{
					text2 = element.GetUsers("*");
				}
				if (text2 != null)
				{
					return !(text2 == "*") && text2.IndexOf(Environment.UserName) < 0;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Public Key Token\tAssemblies\t\tUsers");
			stringBuilder.Append(Environment.NewLine);
			foreach (object obj in StrongNameManager.tokens)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				stringBuilder.Append((string)dictionaryEntry.Key);
				StrongNameManager.Element element = (StrongNameManager.Element)dictionaryEntry.Value;
				bool flag = true;
				foreach (object obj2 in element.assemblies)
				{
					DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
					if (flag)
					{
						stringBuilder.Append("\t");
						flag = false;
					}
					else
					{
						stringBuilder.Append("\t\t\t");
					}
					stringBuilder.Append((string)dictionaryEntry2.Key);
					stringBuilder.Append("\t");
					string text = (string)dictionaryEntry2.Value;
					if (text == "*")
					{
						text = "All users";
					}
					stringBuilder.Append(text);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		private static Hashtable mappings;

		private static Hashtable tokens;

		private class Element
		{
			public Element()
			{
				this.assemblies = new Hashtable();
			}

			public Element(string assembly, string users)
				: this()
			{
				this.assemblies.Add(assembly, users);
			}

			public string GetUsers(string assembly)
			{
				return (string)this.assemblies[assembly];
			}

			internal Hashtable assemblies;
		}
	}
}
