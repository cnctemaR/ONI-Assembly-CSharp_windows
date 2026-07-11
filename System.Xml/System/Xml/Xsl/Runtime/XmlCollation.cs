using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XmlCollation
	{
		private XmlCollation(CultureInfo cultureInfo, XmlCollation.Options options)
		{
			this.cultInfo = cultureInfo;
			this.options = options;
			this.compops = options.CompareOptions;
		}

		internal static XmlCollation CodePointCollation
		{
			get
			{
				return XmlCollation.cp;
			}
		}

		internal static XmlCollation Create(string collationLiteral)
		{
			return XmlCollation.Create(collationLiteral, true);
		}

		internal static XmlCollation Create(string collationLiteral, bool throwOnError)
		{
			if (collationLiteral == "http://www.w3.org/2004/10/xpath-functions/collation/codepoint")
			{
				return XmlCollation.CodePointCollation;
			}
			CultureInfo cultureInfo = null;
			XmlCollation.Options options = default(XmlCollation.Options);
			Uri uri;
			if (throwOnError)
			{
				uri = new Uri(collationLiteral);
			}
			else if (!Uri.TryCreate(collationLiteral, UriKind.Absolute, out uri))
			{
				return null;
			}
			if (uri.GetLeftPart(UriPartial.Authority) == "http://collations.microsoft.com")
			{
				string text = uri.LocalPath.Substring(1);
				if (text.Length == 0)
				{
					goto IL_00C7;
				}
				try
				{
					cultureInfo = new CultureInfo(text);
					goto IL_00C7;
				}
				catch (ArgumentException)
				{
					if (!throwOnError)
					{
						return null;
					}
					throw new XslTransformException("Collation language '{0}' is not supported.", new string[] { text });
				}
			}
			if (uri.IsBaseOf(new Uri("http://www.w3.org/2004/10/xpath-functions/collation/codepoint")))
			{
				options.CompareOptions = CompareOptions.Ordinal;
			}
			else
			{
				if (!throwOnError)
				{
					return null;
				}
				throw new XslTransformException("The collation '{0}' is not supported.", new string[] { collationLiteral });
			}
			IL_00C7:
			string query = uri.Query;
			string text2 = null;
			if (query.Length != 0)
			{
				string[] array = query.Substring(1).Split(new char[] { '&' });
				int i = 0;
				while (i < array.Length)
				{
					string text3 = array[i];
					string[] array2 = text3.Split(new char[] { '=' });
					if (array2.Length != 2)
					{
						if (!throwOnError)
						{
							return null;
						}
						throw new XslTransformException("Collation option '{0}' is invalid. Options must have the following format: <option-name>=<option-value>.", new string[] { text3 });
					}
					else
					{
						string text4 = array2[0].ToUpper(CultureInfo.InvariantCulture);
						string text5 = array2[1].ToUpper(CultureInfo.InvariantCulture);
						if (text4 == "SORT")
						{
							text2 = text5;
						}
						else
						{
							uint num = <PrivateImplementationDetails>.ComputeStringHash(text4);
							int num2;
							if (num <= 1153929311U)
							{
								if (num <= 399689514U)
								{
									if (num != 346004547U)
									{
										if (num != 399689514U)
										{
											goto IL_02BB;
										}
										if (!(text4 == "IGNOREKANATYPE"))
										{
											goto IL_02BB;
										}
										num2 = 8;
									}
									else
									{
										if (!(text4 == "UPPERFIRST"))
										{
											goto IL_02BB;
										}
										num2 = 4096;
									}
								}
								else if (num != 542255445U)
								{
									if (num != 1153929311U)
									{
										goto IL_02BB;
									}
									if (!(text4 == "IGNORECASE"))
									{
										goto IL_02BB;
									}
									num2 = 1;
								}
								else
								{
									if (!(text4 == "IGNOREWIDTH"))
									{
										goto IL_02BB;
									}
									num2 = 16;
								}
							}
							else if (num <= 1618186332U)
							{
								if (num != 1537080989U)
								{
									if (num != 1618186332U)
									{
										goto IL_02BB;
									}
									if (!(text4 == "IGNORENONSPACE"))
									{
										goto IL_02BB;
									}
									num2 = 2;
								}
								else
								{
									if (!(text4 == "DESCENDINGORDER"))
									{
										goto IL_02BB;
									}
									num2 = 16384;
								}
							}
							else if (num != 1721049792U)
							{
								if (num != 3407466425U)
								{
									goto IL_02BB;
								}
								if (!(text4 == "EMPTYGREATEST"))
								{
									goto IL_02BB;
								}
								num2 = 8192;
							}
							else
							{
								if (!(text4 == "IGNORESYMBOLS"))
								{
									goto IL_02BB;
								}
								num2 = 4;
							}
							if (text5 == "0" || text5 == "FALSE")
							{
								options.SetFlag(num2, false);
								goto IL_034E;
							}
							if (text5 == "1" || text5 == "TRUE")
							{
								options.SetFlag(num2, true);
								goto IL_034E;
							}
							if (!throwOnError)
							{
								return null;
							}
							throw new XslTransformException("Collation option '{0}' cannot have the value '{1}'.", new string[]
							{
								array2[0],
								array2[1]
							});
							IL_02BB:
							if (!throwOnError)
							{
								return null;
							}
							throw new XslTransformException("Unsupported option '{0}' in collation.", new string[] { array2[0] });
						}
						IL_034E:
						i++;
					}
				}
			}
			if (options.UpperFirst && options.IgnoreCase)
			{
				options.UpperFirst = false;
			}
			if (options.Ordinal)
			{
				options.CompareOptions = CompareOptions.Ordinal;
				options.UpperFirst = false;
			}
			if (text2 != null && cultureInfo != null)
			{
				int langID = XmlCollation.GetLangID(cultureInfo.LCID);
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
				if (num <= 1363454193U)
				{
					if (num <= 1283486598U)
					{
						if (num != 1278716217U)
						{
							if (num == 1283486598U)
							{
								if (text2 == "trad")
								{
									goto IL_05EE;
								}
							}
						}
						else if (text2 == "dict")
						{
							goto IL_05EE;
						}
					}
					else if (num != 1339334217U)
					{
						if (num == 1363454193U)
						{
							if (text2 == "phn")
							{
								if (langID == 1031)
								{
									cultureInfo = new CultureInfo(66567);
									goto IL_05EE;
								}
								goto IL_05EE;
							}
						}
					}
					else if (text2 == "uni")
					{
						if (langID == 1041 || langID == 1042)
						{
							cultureInfo = new CultureInfo(XmlCollation.MakeLCID(cultureInfo.LCID, 1));
							goto IL_05EE;
						}
						goto IL_05EE;
					}
				}
				else if (num <= 3314303423U)
				{
					if (num != 2751005041U)
					{
						if (num == 3314303423U)
						{
							if (text2 == "bopo")
							{
								if (langID == 1028)
								{
									cultureInfo = new CultureInfo(197636);
									goto IL_05EE;
								}
								goto IL_05EE;
							}
						}
					}
					else if (text2 == "tech")
					{
						if (langID == 1038)
						{
							cultureInfo = new CultureInfo(66574);
							goto IL_05EE;
						}
						goto IL_05EE;
					}
				}
				else if (num != 3629878817U)
				{
					if (num != 3751703171U)
					{
						if (num == 3879610370U)
						{
							if (text2 == "pron")
							{
								goto IL_05EE;
							}
						}
					}
					else if (text2 == "mod")
					{
						if (langID == 1079)
						{
							cultureInfo = new CultureInfo(66615);
							goto IL_05EE;
						}
						goto IL_05EE;
					}
				}
				else if (text2 == "strk")
				{
					if (langID == 2052 || langID == 3076 || langID == 4100 || langID == 5124)
					{
						cultureInfo = new CultureInfo(XmlCollation.MakeLCID(cultureInfo.LCID, 2));
						goto IL_05EE;
					}
					goto IL_05EE;
				}
				if (!throwOnError)
				{
					return null;
				}
				throw new XslTransformException("Unsupported sort option '{0}' in collation.", new string[] { text2 });
			}
			IL_05EE:
			return new XmlCollation(cultureInfo, options);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			XmlCollation xmlCollation = obj as XmlCollation;
			return xmlCollation != null && this.options == xmlCollation.options && object.Equals(this.cultInfo, xmlCollation.cultInfo);
		}

		public override int GetHashCode()
		{
			int num = this.options;
			if (this.cultInfo != null)
			{
				num ^= this.cultInfo.GetHashCode();
			}
			return num;
		}

		internal void GetObjectData(BinaryWriter writer)
		{
			writer.Write((this.cultInfo != null) ? this.cultInfo.LCID : (-1));
			writer.Write(this.options);
		}

		internal XmlCollation(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			this.cultInfo = ((num != -1) ? new CultureInfo(num) : null);
			this.options = new XmlCollation.Options(reader.ReadInt32());
			this.compops = this.options.CompareOptions;
		}

		internal bool UpperFirst
		{
			get
			{
				return this.options.UpperFirst;
			}
		}

		internal bool EmptyGreatest
		{
			get
			{
				return this.options.EmptyGreatest;
			}
		}

		internal bool DescendingOrder
		{
			get
			{
				return this.options.DescendingOrder;
			}
		}

		internal CultureInfo Culture
		{
			get
			{
				if (this.cultInfo == null)
				{
					return CultureInfo.CurrentCulture;
				}
				return this.cultInfo;
			}
		}

		internal XmlSortKey CreateSortKey(string s)
		{
			SortKey sortKey = this.Culture.CompareInfo.GetSortKey(s, this.compops);
			if (!this.UpperFirst)
			{
				return new XmlStringSortKey(sortKey, this.DescendingOrder);
			}
			byte[] keyData = sortKey.KeyData;
			if (this.UpperFirst && keyData.Length != 0)
			{
				int num = 0;
				while (keyData[num] != 1)
				{
					num++;
				}
				do
				{
					num++;
				}
				while (keyData[num] != 1);
				do
				{
					num++;
					byte[] array = keyData;
					int num2 = num;
					array[num2] ^= byte.MaxValue;
				}
				while (keyData[num] != 254);
			}
			return new XmlStringSortKey(keyData, this.DescendingOrder);
		}

		private static int MakeLCID(int langid, int sortid)
		{
			return (langid & 65535) | ((sortid & 15) << 16);
		}

		private static int GetLangID(int lcid)
		{
			return lcid & 65535;
		}

		private const int deDE = 1031;

		private const int huHU = 1038;

		private const int jaJP = 1041;

		private const int kaGE = 1079;

		private const int koKR = 1042;

		private const int zhTW = 1028;

		private const int zhCN = 2052;

		private const int zhHK = 3076;

		private const int zhSG = 4100;

		private const int zhMO = 5124;

		private const int zhTWbopo = 197636;

		private const int deDEphon = 66567;

		private const int huHUtech = 66574;

		private const int kaGEmode = 66615;

		private CultureInfo cultInfo;

		private XmlCollation.Options options;

		private CompareOptions compops;

		private static XmlCollation cp = new XmlCollation(CultureInfo.InvariantCulture, new XmlCollation.Options(1073741824));

		private const int LOCALE_CURRENT = -1;

		private struct Options
		{
			public Options(int value)
			{
				this.value = value;
			}

			public bool GetFlag(int flag)
			{
				return (this.value & flag) != 0;
			}

			public void SetFlag(int flag, bool value)
			{
				if (value)
				{
					this.value |= flag;
					return;
				}
				this.value &= ~flag;
			}

			public bool UpperFirst
			{
				get
				{
					return this.GetFlag(4096);
				}
				set
				{
					this.SetFlag(4096, value);
				}
			}

			public bool EmptyGreatest
			{
				get
				{
					return this.GetFlag(8192);
				}
			}

			public bool DescendingOrder
			{
				get
				{
					return this.GetFlag(16384);
				}
			}

			public bool IgnoreCase
			{
				get
				{
					return this.GetFlag(1);
				}
			}

			public bool Ordinal
			{
				get
				{
					return this.GetFlag(1073741824);
				}
			}

			public CompareOptions CompareOptions
			{
				get
				{
					return (CompareOptions)(this.value & -28673);
				}
				set
				{
					this.value = (this.value & 28672) | (int)value;
				}
			}

			public static implicit operator int(XmlCollation.Options options)
			{
				return options.value;
			}

			public const int FlagUpperFirst = 4096;

			public const int FlagEmptyGreatest = 8192;

			public const int FlagDescendingOrder = 16384;

			private const int Mask = 28672;

			private int value;
		}
	}
}
