using System;

namespace UnityEngine.Assertions
{
	internal class AssertionMessageUtil
	{
		public static string GetMessage(string failureMessage)
		{
			return string.Format("{0} {1}", "Assertion failure.", failureMessage);
		}

		public static string GetMessage(string failureMessage, string expected)
		{
			return AssertionMessageUtil.GetMessage(string.Format("{0}{1}{2} {3}", new object[]
			{
				failureMessage,
				Environment.NewLine,
				"Expected:",
				expected
			}));
		}

		public static string GetEqualityMessage(object actual, object expected, bool expectEqual)
		{
			return AssertionMessageUtil.GetMessage(string.Format("Values are {0}equal.", expectEqual ? "not " : ""), string.Format("{0} {2} {1}", actual, expected, expectEqual ? "==" : "!="));
		}

		public static string NullFailureMessage(object value, bool expectNull)
		{
			return AssertionMessageUtil.GetMessage(string.Format("Value was {0}Null", expectNull ? "not " : ""), string.Format("Value was {0}Null", expectNull ? "" : "not "));
		}

		public static string BooleanFailureMessage(bool expected)
		{
			return AssertionMessageUtil.GetMessage("Value was " + (!expected).ToString(), expected.ToString());
		}

		private const string k_Expected = "Expected:";

		private const string k_AssertionFailed = "Assertion failure.";
	}
}
