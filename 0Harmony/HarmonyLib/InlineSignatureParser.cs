using System;
using System.IO;
using System.Reflection;

namespace HarmonyLib
{
	internal static class InlineSignatureParser
	{
		internal static InlineSignature ImportCallSite(Module moduleFrom, byte[] data)
		{
			InlineSignatureParser.<>c__DisplayClass0_0 CS$<>8__locals1 = new InlineSignatureParser.<>c__DisplayClass0_0();
			CS$<>8__locals1.moduleFrom = moduleFrom;
			InlineSignature inlineSignature = new InlineSignature();
			InlineSignature inlineSignature2;
			using (MemoryStream memoryStream = new MemoryStream(data, false))
			{
				CS$<>8__locals1.reader = new BinaryReader(memoryStream);
				try
				{
					CS$<>8__locals1.<ImportCallSite>g__ReadMethodSignature|0(inlineSignature);
					inlineSignature2 = inlineSignature;
				}
				finally
				{
					if (CS$<>8__locals1.reader != null)
					{
						((IDisposable)CS$<>8__locals1.reader).Dispose();
					}
				}
			}
			return inlineSignature2;
		}
	}
}
