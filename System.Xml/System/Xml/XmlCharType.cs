using System;
using System.Threading;

namespace System.Xml
{
	internal struct XmlCharType
	{
		private static object StaticLock
		{
			get
			{
				if (XmlCharType.s_Lock == null)
				{
					object obj = new object();
					Interlocked.CompareExchange<object>(ref XmlCharType.s_Lock, obj, null);
				}
				return XmlCharType.s_Lock;
			}
		}

		private static void InitInstance()
		{
			object staticLock = XmlCharType.StaticLock;
			lock (staticLock)
			{
				if (XmlCharType.s_CharProperties == null)
				{
					byte[] array = new byte[65536];
					XmlCharType.SetProperties(array, "\t\n\r\r  ", 1);
					XmlCharType.SetProperties(array, "AZazÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁΆΆΈΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆאתװײءغفيٱڷںھۀێېۓەەۥۦअहऽऽक़ॡঅঌএঐওনপরললশহড়ঢ়য়ৡৰৱਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹਖ਼ੜਫ਼ਫ਼ੲੴઅઋઍઍએઑઓનપરલળવહઽઽૠૠଅଌଏଐଓନପରଲଳଶହଽଽଡ଼ଢ଼ୟୡஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹఅఌఎఐఒనపళవహౠౡಅಌಎಐಒನಪಳವಹೞೞೠೡഅഌഎഐഒനപഹൠൡกฮะะาำเๅກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະະາຳຽຽເໄཀཇཉཀྵႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼΩΩKÅ℮℮ↀↂ〇〇〡〩ぁゔァヺㄅㄬ一龥가힣", 2);
					XmlCharType.SetProperties(array, "AZ__azÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁΆΆΈΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆאתװײءغفيٱڷںھۀێېۓەەۥۦअहऽऽक़ॡঅঌএঐওনপরললশহড়ঢ়য়ৡৰৱਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹਖ਼ੜਫ਼ਫ਼ੲੴઅઋઍઍએઑઓનપરલળવહઽઽૠૠଅଌଏଐଓନପରଲଳଶହଽଽଡ଼ଢ଼ୟୡஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹఅఌఎఐఒనపళవహౠౡಅಌಎಐಒನಪಳವಹೞೞೠೡഅഌഎഐഒനപഹൠൡกฮะะาำเๅກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະະາຳຽຽເໄཀཇཉཀྵႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼΩΩKÅ℮℮ↀↂ〇〇〡〩ぁゔァヺㄅㄬ一龥가힣", 4);
					XmlCharType.SetProperties(array, "-.09AZ__az··ÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁːˑ\u0300\u0345\u0360\u0361ΆΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁ\u0483\u0486ҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆ\u0591\u05a1\u05a3\u05b9\u05bb\u05bd\u05bf\u05bf\u05c1\u05c2\u05c4\u05c4אתװײءغـ\u0652٠٩\u0670ڷںھۀێېۓە\u06e8\u06ea\u06ed۰۹\u0901\u0903अह\u093c\u094d\u0951\u0954क़\u0963०९\u0981\u0983অঌএঐওনপরললশহ\u09bc\u09bc\u09be\u09c4\u09c7\u09c8\u09cb\u09cd\u09d7\u09d7ড়ঢ়য়\u09e3০ৱ\u0a02\u0a02ਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹ\u0a3c\u0a3c\u0a3e\u0a42\u0a47\u0a48\u0a4b\u0a4dਖ਼ੜਫ਼ਫ਼੦ੴ\u0a81\u0a83અઋઍઍએઑઓનપરલળવહ\u0abc\u0ac5\u0ac7\u0ac9\u0acb\u0acdૠૠ૦૯\u0b01\u0b03ଅଌଏଐଓନପରଲଳଶହ\u0b3c\u0b43\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57ଡ଼ଢ଼ୟୡ୦୯\u0b82ஃஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹ\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd\u0bd7\u0bd7௧௯\u0c01\u0c03అఌఎఐఒనపళవహ\u0c3e\u0c44\u0c46\u0c48\u0c4a\u0c4d\u0c55\u0c56ౠౡ౦౯\u0c82\u0c83ಅಌಎಐಒನಪಳವಹ\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd\u0cd5\u0cd6ೞೞೠೡ೦೯\u0d02\u0d03അഌഎഐഒനപഹ\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0d57\u0d57ൠൡ൦൯กฮะ\u0e3aเ\u0e4e๐๙ກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະ\u0eb9\u0ebbຽເໄໆໆ\u0ec8\u0ecd໐໙\u0f18\u0f19༠༩\u0f35\u0f35\u0f37\u0f37\u0f39\u0f39\u0f3eཇཉཀྵ\u0f71\u0f84\u0f86ྋ\u0f90\u0f95\u0f97\u0f97\u0f99\u0fad\u0fb1\u0fb7\u0fb9\u0fb9ႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼ\u20d0\u20dc\u20e1\u20e1ΩΩKÅ℮℮ↀↂ々々〇〇〡\u302f〱〵ぁゔ\u3099\u309aゝゞァヺーヾㄅㄬ一龥가힣", 8);
					XmlCharType.SetProperties(array, "\t\n\r\r \ud7ff\ue000\ufffd", 16);
					XmlCharType.SetProperties(array, "-.09AZ__az··ÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁːˑ\u0300\u0345\u0360\u0361ΆΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁ\u0483\u0486ҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆ\u0591\u05a1\u05a3\u05b9\u05bb\u05bd\u05bf\u05bf\u05c1\u05c2\u05c4\u05c4אתװײءغـ\u0652٠٩\u0670ڷںھۀێېۓە\u06e8\u06ea\u06ed۰۹\u0901\u0903अह\u093c\u094d\u0951\u0954क़\u0963०९\u0981\u0983অঌএঐওনপরললশহ\u09bc\u09bc\u09be\u09c4\u09c7\u09c8\u09cb\u09cd\u09d7\u09d7ড়ঢ়য়\u09e3০ৱ\u0a02\u0a02ਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹ\u0a3c\u0a3c\u0a3e\u0a42\u0a47\u0a48\u0a4b\u0a4dਖ਼ੜਫ਼ਫ਼੦ੴ\u0a81\u0a83અઋઍઍએઑઓનપરલળવહ\u0abc\u0ac5\u0ac7\u0ac9\u0acb\u0acdૠૠ૦૯\u0b01\u0b03ଅଌଏଐଓନପରଲଳଶହ\u0b3c\u0b43\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57ଡ଼ଢ଼ୟୡ୦୯\u0b82ஃஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹ\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd\u0bd7\u0bd7௧௯\u0c01\u0c03అఌఎఐఒనపళవహ\u0c3e\u0c44\u0c46\u0c48\u0c4a\u0c4d\u0c55\u0c56ౠౡ౦౯\u0c82\u0c83ಅಌಎಐಒನಪಳವಹ\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd\u0cd5\u0cd6ೞೞೠೡ೦೯\u0d02\u0d03അഌഎഐഒനപഹ\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0d57\u0d57ൠൡ൦൯กฮะ\u0e3aเ\u0e4e๐๙ກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະ\u0eb9\u0ebbຽເໄໆໆ\u0ec8\u0ecd໐໙\u0f18\u0f19༠༩\u0f35\u0f35\u0f37\u0f37\u0f39\u0f39\u0f3eཇཉཀྵ\u0f71\u0f84\u0f86ྋ\u0f90\u0f95\u0f97\u0f97\u0f99\u0fad\u0fb1\u0fb7\u0fb9\u0fb9ႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼ\u20d0\u20dc\u20e1\u20e1ΩΩKÅ℮℮ↀↂ々々〇〇〡\u302f〱〵ぁゔ\u3099\u309aゝゞァヺーヾㄅㄬ一龥가힣", 32);
					XmlCharType.SetProperties(array, " %';=\\^\ud7ff\ue000\ufffd", 64);
					XmlCharType.SetProperties(array, " !#%(;==?\ud7ff\ue000\ufffd", 128);
					Thread.MemoryBarrier();
					XmlCharType.s_CharProperties = array;
				}
			}
		}

		private static void SetProperties(byte[] chProps, string ranges, byte value)
		{
			for (int i = 0; i < ranges.Length; i += 2)
			{
				int j = (int)ranges[i];
				int num = (int)ranges[i + 1];
				while (j <= num)
				{
					int num2 = j;
					chProps[num2] |= value;
					j++;
				}
			}
		}

		private XmlCharType(byte[] charProperties)
		{
			this.charProperties = charProperties;
		}

		public static XmlCharType Instance
		{
			get
			{
				if (XmlCharType.s_CharProperties == null)
				{
					XmlCharType.InitInstance();
				}
				return new XmlCharType(XmlCharType.s_CharProperties);
			}
		}

		public bool IsWhiteSpace(char ch)
		{
			return (this.charProperties[(int)ch] & 1) > 0;
		}

		public bool IsExtender(char ch)
		{
			return ch == '·';
		}

		public bool IsNCNameSingleChar(char ch)
		{
			return (this.charProperties[(int)ch] & 8) > 0;
		}

		public bool IsStartNCNameSingleChar(char ch)
		{
			return (this.charProperties[(int)ch] & 4) > 0;
		}

		public bool IsNameSingleChar(char ch)
		{
			return this.IsNCNameSingleChar(ch) || ch == ':';
		}

		public bool IsStartNameSingleChar(char ch)
		{
			return this.IsStartNCNameSingleChar(ch) || ch == ':';
		}

		public bool IsCharData(char ch)
		{
			return (this.charProperties[(int)ch] & 16) > 0;
		}

		public bool IsPubidChar(char ch)
		{
			return ch < '\u0080' && ((int)"␀\0ﾻ꿿\uffff蟿\ufffe߿"[(int)(ch >> 4)] & (1 << (int)(ch & '\u000f'))) != 0;
		}

		internal bool IsTextChar(char ch)
		{
			return (this.charProperties[(int)ch] & 64) > 0;
		}

		internal bool IsAttributeValueChar(char ch)
		{
			return (this.charProperties[(int)ch] & 128) > 0;
		}

		public bool IsLetter(char ch)
		{
			return (this.charProperties[(int)ch] & 2) > 0;
		}

		public bool IsNCNameCharXml4e(char ch)
		{
			return (this.charProperties[(int)ch] & 32) > 0;
		}

		public bool IsStartNCNameCharXml4e(char ch)
		{
			return this.IsLetter(ch) || ch == '_';
		}

		public bool IsNameCharXml4e(char ch)
		{
			return this.IsNCNameCharXml4e(ch) || ch == ':';
		}

		public bool IsStartNameCharXml4e(char ch)
		{
			return this.IsStartNCNameCharXml4e(ch) || ch == ':';
		}

		public static bool IsDigit(char ch)
		{
			return XmlCharType.InRange((int)ch, 48, 57);
		}

		public static bool IsHexDigit(char ch)
		{
			return XmlCharType.InRange((int)ch, 48, 57) || XmlCharType.InRange((int)ch, 97, 102) || XmlCharType.InRange((int)ch, 65, 70);
		}

		internal static bool IsHighSurrogate(int ch)
		{
			return XmlCharType.InRange(ch, 55296, 56319);
		}

		internal static bool IsLowSurrogate(int ch)
		{
			return XmlCharType.InRange(ch, 56320, 57343);
		}

		internal static bool IsSurrogate(int ch)
		{
			return XmlCharType.InRange(ch, 55296, 57343);
		}

		internal static int CombineSurrogateChar(int lowChar, int highChar)
		{
			return (lowChar - 56320) | ((highChar - 55296 << 10) + 65536);
		}

		internal static void SplitSurrogateChar(int combinedChar, out char lowChar, out char highChar)
		{
			int num = combinedChar - 65536;
			lowChar = (char)(56320 + num % 1024);
			highChar = (char)(55296 + num / 1024);
		}

		internal bool IsOnlyWhitespace(string str)
		{
			return this.IsOnlyWhitespaceWithPos(str) == -1;
		}

		internal int IsOnlyWhitespaceWithPos(string str)
		{
			if (str != null)
			{
				for (int i = 0; i < str.Length; i++)
				{
					if ((this.charProperties[(int)str[i]] & 1) == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		internal int IsOnlyCharData(string str)
		{
			if (str != null)
			{
				for (int i = 0; i < str.Length; i++)
				{
					if ((this.charProperties[(int)str[i]] & 16) == 0)
					{
						if (i + 1 >= str.Length || !XmlCharType.IsHighSurrogate((int)str[i]) || !XmlCharType.IsLowSurrogate((int)str[i + 1]))
						{
							return i;
						}
						i++;
					}
				}
			}
			return -1;
		}

		internal static bool IsOnlyDigits(string str, int startPos, int len)
		{
			for (int i = startPos; i < startPos + len; i++)
			{
				if (!XmlCharType.IsDigit(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsOnlyDigits(char[] chars, int startPos, int len)
		{
			for (int i = startPos; i < startPos + len; i++)
			{
				if (!XmlCharType.IsDigit(chars[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal int IsPublicId(string str)
		{
			if (str != null)
			{
				for (int i = 0; i < str.Length; i++)
				{
					if (!this.IsPubidChar(str[i]))
					{
						return i;
					}
				}
			}
			return -1;
		}

		private static bool InRange(int value, int start, int end)
		{
			return value - start <= end - start;
		}

		internal const int SurHighStart = 55296;

		internal const int SurHighEnd = 56319;

		internal const int SurLowStart = 56320;

		internal const int SurLowEnd = 57343;

		internal const int SurMask = 64512;

		internal const int fWhitespace = 1;

		internal const int fLetter = 2;

		internal const int fNCStartNameSC = 4;

		internal const int fNCNameSC = 8;

		internal const int fCharData = 16;

		internal const int fNCNameXml4e = 32;

		internal const int fText = 64;

		internal const int fAttrValue = 128;

		private const string s_PublicIdBitmap = "␀\0ﾻ꿿\uffff蟿\ufffe߿";

		private const uint CharPropertiesSize = 65536U;

		internal const string s_Whitespace = "\t\n\r\r  ";

		private const string s_NCStartName = "AZ__azÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁΆΆΈΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆאתװײءغفيٱڷںھۀێېۓەەۥۦअहऽऽक़ॡঅঌএঐওনপরললশহড়ঢ়য়ৡৰৱਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹਖ਼ੜਫ਼ਫ਼ੲੴઅઋઍઍએઑઓનપરલળવહઽઽૠૠଅଌଏଐଓନପରଲଳଶହଽଽଡ଼ଢ଼ୟୡஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹఅఌఎఐఒనపళవహౠౡಅಌಎಐಒನಪಳವಹೞೞೠೡഅഌഎഐഒനപഹൠൡกฮะะาำเๅກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະະາຳຽຽເໄཀཇཉཀྵႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼΩΩKÅ℮℮ↀↂ〇〇〡〩ぁゔァヺㄅㄬ一龥가힣";

		private const string s_NCName = "-.09AZ__az··ÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁːˑ\u0300\u0345\u0360\u0361ΆΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁ\u0483\u0486ҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆ\u0591\u05a1\u05a3\u05b9\u05bb\u05bd\u05bf\u05bf\u05c1\u05c2\u05c4\u05c4אתװײءغـ\u0652٠٩\u0670ڷںھۀێېۓە\u06e8\u06ea\u06ed۰۹\u0901\u0903अह\u093c\u094d\u0951\u0954क़\u0963०९\u0981\u0983অঌএঐওনপরললশহ\u09bc\u09bc\u09be\u09c4\u09c7\u09c8\u09cb\u09cd\u09d7\u09d7ড়ঢ়য়\u09e3০ৱ\u0a02\u0a02ਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹ\u0a3c\u0a3c\u0a3e\u0a42\u0a47\u0a48\u0a4b\u0a4dਖ਼ੜਫ਼ਫ਼੦ੴ\u0a81\u0a83અઋઍઍએઑઓનપરલળવહ\u0abc\u0ac5\u0ac7\u0ac9\u0acb\u0acdૠૠ૦૯\u0b01\u0b03ଅଌଏଐଓନପରଲଳଶହ\u0b3c\u0b43\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57ଡ଼ଢ଼ୟୡ୦୯\u0b82ஃஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹ\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd\u0bd7\u0bd7௧௯\u0c01\u0c03అఌఎఐఒనపళవహ\u0c3e\u0c44\u0c46\u0c48\u0c4a\u0c4d\u0c55\u0c56ౠౡ౦౯\u0c82\u0c83ಅಌಎಐಒನಪಳವಹ\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd\u0cd5\u0cd6ೞೞೠೡ೦೯\u0d02\u0d03അഌഎഐഒനപഹ\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0d57\u0d57ൠൡ൦൯กฮะ\u0e3aเ\u0e4e๐๙ກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະ\u0eb9\u0ebbຽເໄໆໆ\u0ec8\u0ecd໐໙\u0f18\u0f19༠༩\u0f35\u0f35\u0f37\u0f37\u0f39\u0f39\u0f3eཇཉཀྵ\u0f71\u0f84\u0f86ྋ\u0f90\u0f95\u0f97\u0f97\u0f99\u0fad\u0fb1\u0fb7\u0fb9\u0fb9ႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼ\u20d0\u20dc\u20e1\u20e1ΩΩKÅ℮℮ↀↂ々々〇〇〡\u302f〱〵ぁゔ\u3099\u309aゝゞァヺーヾㄅㄬ一龥가힣";

		private const string s_CharData = "\t\n\r\r \ud7ff\ue000\ufffd";

		private const string s_PublicID = "\n\n\r\r !#%';==?Z__az";

		private const string s_Text = " %';=\\^\ud7ff\ue000\ufffd";

		private const string s_AttrValue = " !#%(;==?\ud7ff\ue000\ufffd";

		private const string s_LetterXml4e = "AZazÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁΆΆΈΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆאתװײءغفيٱڷںھۀێېۓەەۥۦअहऽऽक़ॡঅঌএঐওনপরললশহড়ঢ়য়ৡৰৱਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹਖ਼ੜਫ਼ਫ਼ੲੴઅઋઍઍએઑઓનપરલળવહઽઽૠૠଅଌଏଐଓନପରଲଳଶହଽଽଡ଼ଢ଼ୟୡஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹఅఌఎఐఒనపళవహౠౡಅಌಎಐಒನಪಳವಹೞೞೠೡഅഌഎഐഒനപഹൠൡกฮะะาำเๅກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະະາຳຽຽເໄཀཇཉཀྵႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼΩΩKÅ℮℮ↀↂ〇〇〡〩ぁゔァヺㄅㄬ一龥가힣";

		private const string s_NCNameXml4e = "-.09AZ__az··ÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁːˑ\u0300\u0345\u0360\u0361ΆΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁ\u0483\u0486ҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆ\u0591\u05a1\u05a3\u05b9\u05bb\u05bd\u05bf\u05bf\u05c1\u05c2\u05c4\u05c4אתװײءغـ\u0652٠٩\u0670ڷںھۀێېۓە\u06e8\u06ea\u06ed۰۹\u0901\u0903अह\u093c\u094d\u0951\u0954क़\u0963०९\u0981\u0983অঌএঐওনপরললশহ\u09bc\u09bc\u09be\u09c4\u09c7\u09c8\u09cb\u09cd\u09d7\u09d7ড়ঢ়য়\u09e3০ৱ\u0a02\u0a02ਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹ\u0a3c\u0a3c\u0a3e\u0a42\u0a47\u0a48\u0a4b\u0a4dਖ਼ੜਫ਼ਫ਼੦ੴ\u0a81\u0a83અઋઍઍએઑઓનપરલળવહ\u0abc\u0ac5\u0ac7\u0ac9\u0acb\u0acdૠૠ૦૯\u0b01\u0b03ଅଌଏଐଓନପରଲଳଶହ\u0b3c\u0b43\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57ଡ଼ଢ଼ୟୡ୦୯\u0b82ஃஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹ\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd\u0bd7\u0bd7௧௯\u0c01\u0c03అఌఎఐఒనపళవహ\u0c3e\u0c44\u0c46\u0c48\u0c4a\u0c4d\u0c55\u0c56ౠౡ౦౯\u0c82\u0c83ಅಌಎಐಒನಪಳವಹ\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd\u0cd5\u0cd6ೞೞೠೡ೦೯\u0d02\u0d03അഌഎഐഒനപഹ\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0d57\u0d57ൠൡ൦൯กฮะ\u0e3aเ\u0e4e๐๙ກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະ\u0eb9\u0ebbຽເໄໆໆ\u0ec8\u0ecd໐໙\u0f18\u0f19༠༩\u0f35\u0f35\u0f37\u0f37\u0f39\u0f39\u0f3eཇཉཀྵ\u0f71\u0f84\u0f86ྋ\u0f90\u0f95\u0f97\u0f97\u0f99\u0fad\u0fb1\u0fb7\u0fb9\u0fb9ႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼ\u20d0\u20dc\u20e1\u20e1ΩΩKÅ℮℮ↀↂ々々〇〇〡\u302f〱〵ぁゔ\u3099\u309aゝゞァヺーヾㄅㄬ一龥가힣";

		private static object s_Lock;

		private static volatile byte[] s_CharProperties;

		internal byte[] charProperties;
	}
}
