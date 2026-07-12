using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	public class ComposablePartCatalogChangeEventArgs : EventArgs
	{
		public ComposablePartCatalogChangeEventArgs(IEnumerable<ComposablePartDefinition> addedDefinitions, IEnumerable<ComposablePartDefinition> removedDefinitions, AtomicComposition atomicComposition)
		{
			Requires.NotNull<IEnumerable<ComposablePartDefinition>>(addedDefinitions, "addedDefinitions");
			Requires.NotNull<IEnumerable<ComposablePartDefinition>>(removedDefinitions, "removedDefinitions");
			this._addedDefinitions = addedDefinitions.AsArray<ComposablePartDefinition>();
			this._removedDefinitions = removedDefinitions.AsArray<ComposablePartDefinition>();
			this.AtomicComposition = atomicComposition;
		}

		public IEnumerable<ComposablePartDefinition> AddedDefinitions
		{
			get
			{
				return this._addedDefinitions;
			}
		}

		public IEnumerable<ComposablePartDefinition> RemovedDefinitions
		{
			get
			{
				return this._removedDefinitions;
			}
		}

		public AtomicComposition AtomicComposition { get; private set; }

		private readonly IEnumerable<ComposablePartDefinition> _addedDefinitions;

		private readonly IEnumerable<ComposablePartDefinition> _removedDefinitions;
	}
}
