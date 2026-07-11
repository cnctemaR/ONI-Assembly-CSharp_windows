using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public class YamlStream : IEnumerable<YamlDocument>, IEnumerable
	{
		public IList<YamlDocument> Documents
		{
			get
			{
				return this.documents;
			}
		}

		public YamlStream()
		{
		}

		public YamlStream(params YamlDocument[] documents)
			: this(documents)
		{
		}

		public YamlStream(IEnumerable<YamlDocument> documents)
		{
			foreach (YamlDocument yamlDocument in documents)
			{
				this.documents.Add(yamlDocument);
			}
		}

		public void Add(YamlDocument document)
		{
			this.documents.Add(document);
		}

		public void Load(TextReader input)
		{
			this.Load(new Parser(input));
		}

		public void Load(IParser parser)
		{
			this.documents.Clear();
			parser.Expect<StreamStart>();
			while (!parser.Accept<StreamEnd>())
			{
				YamlDocument yamlDocument = new YamlDocument(parser);
				this.documents.Add(yamlDocument);
			}
			parser.Expect<StreamEnd>();
		}

		public void Save(TextWriter output)
		{
			this.Save(output, true);
		}

		public void Save(TextWriter output, bool assignAnchors)
		{
			IEmitter emitter = new Emitter(output);
			emitter.Emit(new StreamStart());
			foreach (YamlDocument yamlDocument in this.documents)
			{
				yamlDocument.Save(emitter, assignAnchors);
			}
			emitter.Emit(new StreamEnd());
		}

		public void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public IEnumerator<YamlDocument> GetEnumerator()
		{
			return this.documents.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly IList<YamlDocument> documents = new List<YamlDocument>();
	}
}
