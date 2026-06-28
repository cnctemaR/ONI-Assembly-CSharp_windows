using System;
using System.Text;

namespace FileHelpers
{
	internal sealed class ForwardReader : IDisposable
	{
		internal ForwardReader(IRecordReader reader, int forwardLines)
			: this(reader, forwardLines, 0)
		{
		}

		internal ForwardReader(IRecordReader reader, int forwardLines, int startLine)
		{
			this.mReader = reader;
			this.mFowardLines = forwardLines;
			this.mLineNumber = startLine;
			this.mFowardStrings = new string[this.mFowardLines + 1];
			this.mRemaingLines = this.mFowardLines + 1;
			for (int i = 0; i < this.mFowardLines + 1; i++)
			{
				this.mFowardStrings[i] = this.mReader.ReadRecordString();
				this.mLineNumber++;
				if (this.mFowardStrings[i] == null)
				{
					this.mRemaingLines = i;
					return;
				}
			}
		}

		public int RemainingLines
		{
			get
			{
				return this.mRemaingLines;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.mLineNumber - 1 - this.mFowardLines;
			}
		}

		public bool DiscardForward
		{
			get
			{
				return this.mDiscardForward;
			}
			set
			{
				this.mDiscardForward = value;
			}
		}

		public int FowardLines
		{
			get
			{
				return this.mFowardLines;
			}
		}

		public string ReadNextLine()
		{
			if (this.mRemaingLines <= 0)
			{
				return null;
			}
			string text = this.mFowardStrings[this.mForwardIndex];
			if (this.mRemaingLines == this.mFowardLines + 1)
			{
				this.mFowardStrings[this.mForwardIndex] = this.mReader.ReadRecordString();
				this.mLineNumber++;
				if (this.mFowardStrings[this.mForwardIndex] == null)
				{
					this.mRemaingLines--;
				}
			}
			else
			{
				this.mRemaingLines--;
				if (this.mDiscardForward)
				{
					return null;
				}
			}
			this.mForwardIndex = (this.mForwardIndex + 1) % (this.mFowardLines + 1);
			return text;
		}

		public string RemainingText
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				for (int i = 0; i < this.mRemaingLines + 1; i++)
				{
					stringBuilder.Append(this.mFowardStrings[(this.mForwardIndex + i) % (this.mFowardLines + 1)] + StringHelper.NewLine);
				}
				return stringBuilder.ToString();
			}
		}

		public void Close()
		{
			if (this.mReader != null)
			{
				this.mReader.Close();
			}
		}

		void IDisposable.Dispose()
		{
			this.Close();
			GC.SuppressFinalize(this);
		}

		private readonly IRecordReader mReader;

		private readonly string[] mFowardStrings;

		private int mForwardIndex;

		private int mRemaingLines;

		private int mLineNumber;

		private bool mDiscardForward;

		private readonly int mFowardLines;
	}
}
