using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	internal class CompositionScopeDefinitionDebuggerProxy
	{
		public CompositionScopeDefinitionDebuggerProxy(CompositionScopeDefinition compositionScopeDefinition)
		{
			Requires.NotNull<CompositionScopeDefinition>(compositionScopeDefinition, "compositionScopeDefinition");
			this._compositionScopeDefinition = compositionScopeDefinition;
		}

		public ReadOnlyCollection<ComposablePartDefinition> Parts
		{
			get
			{
				return this._compositionScopeDefinition.Parts.ToReadOnlyCollection<ComposablePartDefinition>();
			}
		}

		public IEnumerable<ExportDefinition> PublicSurface
		{
			get
			{
				return this._compositionScopeDefinition.PublicSurface.ToReadOnlyCollection<ExportDefinition>();
			}
		}

		public virtual IEnumerable<CompositionScopeDefinition> Children
		{
			get
			{
				return this._compositionScopeDefinition.Children.ToReadOnlyCollection<CompositionScopeDefinition>();
			}
		}

		private readonly CompositionScopeDefinition _compositionScopeDefinition;
	}
}
