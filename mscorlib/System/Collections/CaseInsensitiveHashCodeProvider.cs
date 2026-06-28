using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Obsolete("Please use StringComparer instead.")]
	[Serializable]
	public class CaseInsensitiveHashCodeProvider : IHashCodeProvider
	{
		public CaseInsensitiveHashCodeProvider()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			if (!CaseInsensitiveHashCodeProvider.AreEqual(currentCulture, CultureInfo.InvariantCulture))
			{
				this.m_text = CultureInfo.CurrentCulture.TextInfo;
			}
		}

		public CaseInsensitiveHashCodeProvider(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (!CaseInsensitiveHashCodeProvider.AreEqual(culture, CultureInfo.InvariantCulture))
			{
				this.m_text = culture.TextInfo;
			}
		}

		public static CaseInsensitiveHashCodeProvider Default
		{
			get
			{
				object obj = CaseInsensitiveHashCodeProvider.sync;
				CaseInsensitiveHashCodeProvider caseInsensitiveHashCodeProvider;
				lock (obj)
				{
					if (CaseInsensitiveHashCodeProvider.singleton == null)
					{
						CaseInsensitiveHashCodeProvider.singleton = new CaseInsensitiveHashCodeProvider();
					}
					else if (CaseInsensitiveHashCodeProvider.singleton.m_text == null)
					{
						if (!CaseInsensitiveHashCodeProvider.AreEqual(CultureInfo.CurrentCulture, CultureInfo.InvariantCulture))
						{
							CaseInsensitiveHashCodeProvider.singleton = new CaseInsensitiveHashCodeProvider();
						}
					}
					else if (!CaseInsensitiveHashCodeProvider.AreEqual(CaseInsensitiveHashCodeProvider.singleton.m_text, CultureInfo.CurrentCulture))
					{
						CaseInsensitiveHashCodeProvider.singleton = new CaseInsensitiveHashCodeProvider();
					}
					caseInsensitiveHashCodeProvider = CaseInsensitiveHashCodeProvider.singleton;
				}
				return caseInsensitiveHashCodeProvider;
			}
		}

		private static bool AreEqual(CultureInfo a, CultureInfo b)
		{
			return a.LCID == b.LCID;
		}

		private static bool AreEqual(TextInfo info, CultureInfo culture)
		{
			return info.LCID == culture.LCID;
		}

		public static CaseInsensitiveHashCodeProvider DefaultInvariant
		{
			get
			{
				return CaseInsensitiveHashCodeProvider.singletonInvariant;
			}
		}

		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string text = obj as string;
			if (text == null)
			{
				return obj.GetHashCode();
			}
			int num = 0;
			if (this.m_text != null && !CaseInsensitiveHashCodeProvider.AreEqual(this.m_text, CultureInfo.InvariantCulture))
			{
				foreach (char c in this.m_text.ToLower(text))
				{
					num = num * 31 + (int)c;
				}
			}
			else
			{
				for (int j = 0; j < text.Length; j++)
				{
					char c = char.ToLower(text[j], CultureInfo.InvariantCulture);
					num = num * 31 + (int)c;
				}
			}
			return num;
		}

		private static readonly CaseInsensitiveHashCodeProvider singletonInvariant = new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture);

		private static CaseInsensitiveHashCodeProvider singleton;

		private static readonly object sync = new object();

		private TextInfo m_text;
	}
}
