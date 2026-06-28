using System;

public static class StringsUnitTest
{
	public static void UnitTest()
	{
		StringKey stringKey = new StringKey("a");
		StringKey stringKey2 = new StringKey("b");
		StringKey stringKey3 = new StringKey("c");
		Strings.Add(new string[] { stringKey.String, stringKey2.String, stringKey3.String, "hello" });
		StringEntry stringEntry = null;
		for (int i = 0; i < 10000; i++)
		{
			stringEntry = Strings.Get(stringKey, stringKey2, stringKey3);
		}
		DebugUtil.Assert(stringEntry.String == "hello", "Assert!");
	}
}
