using System;
using System.Collections.Generic;

namespace System.Net.Mail
{
	internal static class MailHeaderInfo
	{
		static MailHeaderInfo()
		{
			for (int i = 0; i < MailHeaderInfo.s_headerInfo.Length; i++)
			{
				MailHeaderInfo.s_headerDictionary.Add(MailHeaderInfo.s_headerInfo[i].NormalizedName, i);
			}
		}

		internal static string GetString(MailHeaderID id)
		{
			if (id == MailHeaderID.Unknown || id == (MailHeaderID)33)
			{
				return null;
			}
			return MailHeaderInfo.s_headerInfo[(int)id].NormalizedName;
		}

		internal static MailHeaderID GetID(string name)
		{
			int num;
			if (!MailHeaderInfo.s_headerDictionary.TryGetValue(name, out num))
			{
				return MailHeaderID.Unknown;
			}
			return (MailHeaderID)num;
		}

		internal static bool IsUserSettable(string name)
		{
			int num;
			return !MailHeaderInfo.s_headerDictionary.TryGetValue(name, out num) || MailHeaderInfo.s_headerInfo[num].IsUserSettable;
		}

		internal static bool IsSingleton(string name)
		{
			int num;
			return MailHeaderInfo.s_headerDictionary.TryGetValue(name, out num) && MailHeaderInfo.s_headerInfo[num].IsSingleton;
		}

		internal static string NormalizeCase(string name)
		{
			int num;
			if (!MailHeaderInfo.s_headerDictionary.TryGetValue(name, out num))
			{
				return name;
			}
			return MailHeaderInfo.s_headerInfo[num].NormalizedName;
		}

		internal static bool AllowsUnicode(string name)
		{
			int num;
			return !MailHeaderInfo.s_headerDictionary.TryGetValue(name, out num) || MailHeaderInfo.s_headerInfo[num].AllowsUnicode;
		}

		private static readonly MailHeaderInfo.HeaderInfo[] s_headerInfo = new MailHeaderInfo.HeaderInfo[]
		{
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Bcc, "Bcc", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Cc, "Cc", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Comments, "Comments", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ContentDescription, "Content-Description", true, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ContentDisposition, "Content-Disposition", true, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ContentID, "Content-ID", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ContentLocation, "Content-Location", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ContentTransferEncoding, "Content-Transfer-Encoding", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ContentType, "Content-Type", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Date, "Date", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.From, "From", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Importance, "Importance", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.InReplyTo, "In-Reply-To", true, true, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Keywords, "Keywords", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Max, "Max", false, true, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.MessageID, "Message-ID", true, true, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.MimeVersion, "MIME-Version", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Priority, "Priority", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.References, "References", true, true, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ReplyTo, "Reply-To", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ResentBcc, "Resent-Bcc", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ResentCc, "Resent-Cc", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ResentDate, "Resent-Date", false, true, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ResentFrom, "Resent-From", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ResentMessageID, "Resent-Message-ID", false, true, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ResentSender, "Resent-Sender", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.ResentTo, "Resent-To", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Sender, "Sender", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.Subject, "Subject", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.To, "To", true, false, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.XPriority, "X-Priority", true, false, false),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.XReceiver, "X-Receiver", false, true, true),
			new MailHeaderInfo.HeaderInfo(MailHeaderID.XSender, "X-Sender", true, true, true)
		};

		private static readonly Dictionary<string, int> s_headerDictionary = new Dictionary<string, int>(33, StringComparer.OrdinalIgnoreCase);

		private readonly struct HeaderInfo
		{
			public HeaderInfo(MailHeaderID id, string name, bool isSingleton, bool isUserSettable, bool allowsUnicode)
			{
				this.ID = id;
				this.NormalizedName = name;
				this.IsSingleton = isSingleton;
				this.IsUserSettable = isUserSettable;
				this.AllowsUnicode = allowsUnicode;
			}

			public readonly string NormalizedName;

			public readonly bool IsSingleton;

			public readonly MailHeaderID ID;

			public readonly bool IsUserSettable;

			public readonly bool AllowsUnicode;
		}
	}
}
