using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mono.Net.Dns
{
	internal class DnsResponse : DnsPacket
	{
		public DnsResponse(byte[] buffer, int length)
			: base(buffer, length)
		{
		}

		public void Reset()
		{
			this.question = null;
			this.answer = null;
			this.authority = null;
			this.additional = null;
			for (int i = 0; i < this.packet.Length; i++)
			{
				this.packet[i] = 0;
			}
		}

		private ReadOnlyCollection<DnsResourceRecord> GetRRs(int count)
		{
			if (count <= 0)
			{
				return DnsResponse.EmptyRR;
			}
			List<DnsResourceRecord> list = new List<DnsResourceRecord>(count);
			for (int i = 0; i < count; i++)
			{
				list.Add(DnsResourceRecord.CreateFromBuffer(this, this.position, ref this.offset));
			}
			return list.AsReadOnly();
		}

		private ReadOnlyCollection<DnsQuestion> GetQuestions(int count)
		{
			if (count <= 0)
			{
				return DnsResponse.EmptyQS;
			}
			List<DnsQuestion> list = new List<DnsQuestion>(count);
			for (int i = 0; i < count; i++)
			{
				DnsQuestion dnsQuestion = new DnsQuestion();
				this.offset = dnsQuestion.Init(this, this.offset);
				list.Add(dnsQuestion);
			}
			return list.AsReadOnly();
		}

		public ReadOnlyCollection<DnsQuestion> GetQuestions()
		{
			if (this.question == null)
			{
				this.question = this.GetQuestions((int)base.Header.QuestionCount);
			}
			return this.question;
		}

		public ReadOnlyCollection<DnsResourceRecord> GetAnswers()
		{
			if (this.answer == null)
			{
				this.GetQuestions();
				this.answer = this.GetRRs((int)base.Header.AnswerCount);
			}
			return this.answer;
		}

		public ReadOnlyCollection<DnsResourceRecord> GetAuthority()
		{
			if (this.authority == null)
			{
				this.GetQuestions();
				this.GetAnswers();
				this.authority = this.GetRRs((int)base.Header.AuthorityCount);
			}
			return this.authority;
		}

		public ReadOnlyCollection<DnsResourceRecord> GetAdditional()
		{
			if (this.additional == null)
			{
				this.GetQuestions();
				this.GetAnswers();
				this.GetAuthority();
				this.additional = this.GetRRs((int)base.Header.AdditionalCount);
			}
			return this.additional;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.Header);
			stringBuilder.Append("Question:\r\n");
			foreach (DnsQuestion dnsQuestion in this.GetQuestions())
			{
				stringBuilder.AppendFormat("\t{0}\r\n", dnsQuestion);
			}
			stringBuilder.Append("Answer(s):\r\n");
			foreach (DnsResourceRecord dnsResourceRecord in this.GetAnswers())
			{
				stringBuilder.AppendFormat("\t{0}\r\n", dnsResourceRecord);
			}
			stringBuilder.Append("Authority:\r\n");
			foreach (DnsResourceRecord dnsResourceRecord2 in this.GetAuthority())
			{
				stringBuilder.AppendFormat("\t{0}\r\n", dnsResourceRecord2);
			}
			stringBuilder.Append("Additional:\r\n");
			foreach (DnsResourceRecord dnsResourceRecord3 in this.GetAdditional())
			{
				stringBuilder.AppendFormat("\t{0}\r\n", dnsResourceRecord3);
			}
			return stringBuilder.ToString();
		}

		private static readonly ReadOnlyCollection<DnsResourceRecord> EmptyRR = new ReadOnlyCollection<DnsResourceRecord>(new DnsResourceRecord[0]);

		private static readonly ReadOnlyCollection<DnsQuestion> EmptyQS = new ReadOnlyCollection<DnsQuestion>(new DnsQuestion[0]);

		private ReadOnlyCollection<DnsQuestion> question;

		private ReadOnlyCollection<DnsResourceRecord> answer;

		private ReadOnlyCollection<DnsResourceRecord> authority;

		private ReadOnlyCollection<DnsResourceRecord> additional;

		private int offset = 12;
	}
}
