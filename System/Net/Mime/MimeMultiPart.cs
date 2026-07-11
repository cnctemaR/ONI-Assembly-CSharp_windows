using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading;

namespace System.Net.Mime
{
	internal class MimeMultiPart : MimeBasePart
	{
		internal MimeMultiPart(MimeMultiPartType type)
		{
			this.MimeMultiPartType = type;
		}

		internal MimeMultiPartType MimeMultiPartType
		{
			set
			{
				if (value > MimeMultiPartType.Related || value < MimeMultiPartType.Mixed)
				{
					throw new NotSupportedException(value.ToString());
				}
				this.SetType(value);
			}
		}

		private void SetType(MimeMultiPartType type)
		{
			base.ContentType.MediaType = "multipart/" + type.ToString().ToLower(CultureInfo.InvariantCulture);
			base.ContentType.Boundary = this.GetNextBoundary();
		}

		internal Collection<MimeBasePart> Parts
		{
			get
			{
				if (this.parts == null)
				{
					this.parts = new Collection<MimeBasePart>();
				}
				return this.parts;
			}
		}

		internal void Complete(IAsyncResult result, Exception e)
		{
			MimeMultiPart.MimePartContext mimePartContext = (MimeMultiPart.MimePartContext)result.AsyncState;
			if (mimePartContext.completed)
			{
				throw e;
			}
			try
			{
				mimePartContext.outputStream.Close();
			}
			catch (Exception ex)
			{
				if (e == null)
				{
					e = ex;
				}
			}
			mimePartContext.completed = true;
			mimePartContext.result.InvokeCallback(e);
		}

		internal void MimeWriterCloseCallback(IAsyncResult result)
		{
			if (result.CompletedSynchronously)
			{
				return;
			}
			((MimeMultiPart.MimePartContext)result.AsyncState).completedSynchronously = false;
			try
			{
				this.MimeWriterCloseCallbackHandler(result);
			}
			catch (Exception ex)
			{
				this.Complete(result, ex);
			}
		}

		private void MimeWriterCloseCallbackHandler(IAsyncResult result)
		{
			((MimeWriter)((MimeMultiPart.MimePartContext)result.AsyncState).writer).EndClose(result);
			this.Complete(result, null);
		}

		internal void MimePartSentCallback(IAsyncResult result)
		{
			if (result.CompletedSynchronously)
			{
				return;
			}
			((MimeMultiPart.MimePartContext)result.AsyncState).completedSynchronously = false;
			try
			{
				this.MimePartSentCallbackHandler(result);
			}
			catch (Exception ex)
			{
				this.Complete(result, ex);
			}
		}

		private void MimePartSentCallbackHandler(IAsyncResult result)
		{
			MimeMultiPart.MimePartContext mimePartContext = (MimeMultiPart.MimePartContext)result.AsyncState;
			mimePartContext.partsEnumerator.Current.EndSend(result);
			if (mimePartContext.partsEnumerator.MoveNext())
			{
				IAsyncResult asyncResult = mimePartContext.partsEnumerator.Current.BeginSend(mimePartContext.writer, this.mimePartSentCallback, this.allowUnicode, mimePartContext);
				if (asyncResult.CompletedSynchronously)
				{
					this.MimePartSentCallbackHandler(asyncResult);
				}
				return;
			}
			IAsyncResult asyncResult2 = ((MimeWriter)mimePartContext.writer).BeginClose(new AsyncCallback(this.MimeWriterCloseCallback), mimePartContext);
			if (asyncResult2.CompletedSynchronously)
			{
				this.MimeWriterCloseCallbackHandler(asyncResult2);
			}
		}

		internal void ContentStreamCallback(IAsyncResult result)
		{
			if (result.CompletedSynchronously)
			{
				return;
			}
			((MimeMultiPart.MimePartContext)result.AsyncState).completedSynchronously = false;
			try
			{
				this.ContentStreamCallbackHandler(result);
			}
			catch (Exception ex)
			{
				this.Complete(result, ex);
			}
		}

		private void ContentStreamCallbackHandler(IAsyncResult result)
		{
			MimeMultiPart.MimePartContext mimePartContext = (MimeMultiPart.MimePartContext)result.AsyncState;
			mimePartContext.outputStream = mimePartContext.writer.EndGetContentStream(result);
			mimePartContext.writer = new MimeWriter(mimePartContext.outputStream, base.ContentType.Boundary);
			if (mimePartContext.partsEnumerator.MoveNext())
			{
				MimeBasePart mimeBasePart = mimePartContext.partsEnumerator.Current;
				this.mimePartSentCallback = new AsyncCallback(this.MimePartSentCallback);
				IAsyncResult asyncResult = mimeBasePart.BeginSend(mimePartContext.writer, this.mimePartSentCallback, this.allowUnicode, mimePartContext);
				if (asyncResult.CompletedSynchronously)
				{
					this.MimePartSentCallbackHandler(asyncResult);
				}
				return;
			}
			IAsyncResult asyncResult2 = ((MimeWriter)mimePartContext.writer).BeginClose(new AsyncCallback(this.MimeWriterCloseCallback), mimePartContext);
			if (asyncResult2.CompletedSynchronously)
			{
				this.MimeWriterCloseCallbackHandler(asyncResult2);
			}
		}

		internal override IAsyncResult BeginSend(BaseWriter writer, AsyncCallback callback, bool allowUnicode, object state)
		{
			this.allowUnicode = allowUnicode;
			base.PrepareHeaders(allowUnicode);
			writer.WriteHeaders(base.Headers, allowUnicode);
			MimeBasePart.MimePartAsyncResult mimePartAsyncResult = new MimeBasePart.MimePartAsyncResult(this, state, callback);
			MimeMultiPart.MimePartContext mimePartContext = new MimeMultiPart.MimePartContext(writer, mimePartAsyncResult, this.Parts.GetEnumerator());
			IAsyncResult asyncResult = writer.BeginGetContentStream(new AsyncCallback(this.ContentStreamCallback), mimePartContext);
			if (asyncResult.CompletedSynchronously)
			{
				this.ContentStreamCallbackHandler(asyncResult);
			}
			return mimePartAsyncResult;
		}

		internal override void Send(BaseWriter writer, bool allowUnicode)
		{
			base.PrepareHeaders(allowUnicode);
			writer.WriteHeaders(base.Headers, allowUnicode);
			Stream contentStream = writer.GetContentStream();
			MimeWriter mimeWriter = new MimeWriter(contentStream, base.ContentType.Boundary);
			foreach (MimeBasePart mimeBasePart in this.Parts)
			{
				mimeBasePart.Send(mimeWriter, allowUnicode);
			}
			mimeWriter.Close();
			contentStream.Close();
		}

		internal string GetNextBoundary()
		{
			return "--boundary_" + (Interlocked.Increment(ref MimeMultiPart.boundary) - 1).ToString(CultureInfo.InvariantCulture) + "_" + Guid.NewGuid().ToString(null, CultureInfo.InvariantCulture);
		}

		private Collection<MimeBasePart> parts;

		private static int boundary;

		private AsyncCallback mimePartSentCallback;

		private bool allowUnicode;

		internal class MimePartContext
		{
			internal MimePartContext(BaseWriter writer, LazyAsyncResult result, IEnumerator<MimeBasePart> partsEnumerator)
			{
				this.writer = writer;
				this.result = result;
				this.partsEnumerator = partsEnumerator;
			}

			internal IEnumerator<MimeBasePart> partsEnumerator;

			internal Stream outputStream;

			internal LazyAsyncResult result;

			internal BaseWriter writer;

			internal bool completed;

			internal bool completedSynchronously = true;
		}
	}
}
