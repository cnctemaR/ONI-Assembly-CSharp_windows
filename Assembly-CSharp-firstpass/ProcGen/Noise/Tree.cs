using System;
using System.Collections.Generic;
using LibNoiseDotNet.Graphics.Tools.Noise;

namespace ProcGen.Noise
{
	public class Tree
	{
		public SampleSettings settings { get; set; }

		public List<NodeLink> links { get; set; }

		public Dictionary<string, Primitive> primitives { get; set; }

		public Dictionary<string, Filter> filters { get; set; }

		public Dictionary<string, Transformer> transformers { get; set; }

		public Dictionary<string, Selector> selectors { get; set; }

		public Dictionary<string, Modifier> modifiers { get; set; }

		public Dictionary<string, Combiner> combiners { get; set; }

		public Dictionary<string, FloatList> floats { get; set; }

		public Dictionary<string, ControlPointList> controlpoints { get; set; }

		public Tree()
		{
			this.settings = new SampleSettings();
			this.links = new List<NodeLink>();
			this.primitives = new Dictionary<string, Primitive>();
			this.filters = new Dictionary<string, Filter>();
			this.transformers = new Dictionary<string, Transformer>();
			this.selectors = new Dictionary<string, Selector>();
			this.modifiers = new Dictionary<string, Modifier>();
			this.combiners = new Dictionary<string, Combiner>();
			this.floats = new Dictionary<string, FloatList>();
			this.controlpoints = new Dictionary<string, ControlPointList>();
		}

		public void ClearEmptyLists()
		{
			if (this.links.Count == 0)
			{
				this.links = null;
			}
			if (this.primitives.Count == 0)
			{
				this.primitives = null;
			}
			if (this.filters.Count == 0)
			{
				this.filters = null;
			}
			if (this.transformers.Count == 0)
			{
				this.transformers = null;
			}
			if (this.selectors.Count == 0)
			{
				this.selectors = null;
			}
			if (this.modifiers.Count == 0)
			{
				this.modifiers = null;
			}
			if (this.combiners.Count == 0)
			{
				this.combiners = null;
			}
			if (this.floats.Count == 0)
			{
				this.floats = null;
			}
			if (this.controlpoints.Count == 0)
			{
				this.controlpoints = null;
			}
		}

		public void CreateEmptyLists()
		{
			if (this.links == null)
			{
				this.links = new List<NodeLink>();
			}
			if (this.primitives == null)
			{
				this.primitives = new Dictionary<string, Primitive>();
			}
			if (this.filters == null)
			{
				this.filters = new Dictionary<string, Filter>();
			}
			if (this.transformers == null)
			{
				this.transformers = new Dictionary<string, Transformer>();
			}
			if (this.selectors == null)
			{
				this.selectors = new Dictionary<string, Selector>();
			}
			if (this.modifiers == null)
			{
				this.modifiers = new Dictionary<string, Modifier>();
			}
			if (this.combiners == null)
			{
				this.combiners = new Dictionary<string, Combiner>();
			}
			if (this.floats == null)
			{
				this.floats = new Dictionary<string, FloatList>();
			}
			if (this.controlpoints == null)
			{
				this.controlpoints = new Dictionary<string, ControlPointList>();
			}
		}

		private IModule3D GetModuleFromLink(Link link)
		{
			if (link == null)
			{
				return null;
			}
			switch (link.type)
			{
			case Link.Type.Primitive:
				if (this.primitiveLookup.ContainsKey(link.name))
				{
					return this.primitiveLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in primitives");
				break;
			case Link.Type.Filter:
				if (this.filterLookup.ContainsKey(link.name))
				{
					return this.filterLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in filters");
				break;
			case Link.Type.Transformer:
				if (this.transformerLookup.ContainsKey(link.name))
				{
					return this.transformerLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in transformers");
				break;
			case Link.Type.Selector:
				if (this.selectorLookup.ContainsKey(link.name))
				{
					return this.selectorLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in selectors");
				break;
			case Link.Type.Modifier:
				if (this.modifierLookup.ContainsKey(link.name))
				{
					return this.modifierLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in modifiers");
				break;
			case Link.Type.Combiner:
				if (this.combinerLookup.ContainsKey(link.name))
				{
					return this.combinerLookup[link.name];
				}
				Debug.LogError("Couldnt find [" + link.name + "] in combiners");
				break;
			case Link.Type.Terminator:
				return null;
			}
			Debug.LogError(string.Concat(new string[]
			{
				"Couldnt find link [",
				link.name,
				"] [",
				link.type.ToString(),
				"]"
			}));
			return null;
		}

		public IModule3D BuildFinalModule(int globalSeed)
		{
			IModule3D module3D = null;
			this.primitiveLookup.Clear();
			this.filterLookup.Clear();
			this.modifierLookup.Clear();
			this.selectorLookup.Clear();
			this.transformerLookup.Clear();
			this.combinerLookup.Clear();
			foreach (KeyValuePair<string, Primitive> keyValuePair in this.primitives)
			{
				this.primitiveLookup.Add(keyValuePair.Key, keyValuePair.Value.CreateModule(globalSeed));
			}
			foreach (KeyValuePair<string, Filter> keyValuePair2 in this.filters)
			{
				this.filterLookup.Add(keyValuePair2.Key, keyValuePair2.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Modifier> keyValuePair3 in this.modifiers)
			{
				this.modifierLookup.Add(keyValuePair3.Key, keyValuePair3.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Selector> keyValuePair4 in this.selectors)
			{
				this.selectorLookup.Add(keyValuePair4.Key, keyValuePair4.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Transformer> keyValuePair5 in this.transformers)
			{
				this.transformerLookup.Add(keyValuePair5.Key, keyValuePair5.Value.CreateModule());
			}
			foreach (KeyValuePair<string, Combiner> keyValuePair6 in this.combiners)
			{
				this.combinerLookup.Add(keyValuePair6.Key, keyValuePair6.Value.CreateModule());
			}
			for (int i = 0; i < this.links.Count; i++)
			{
				NodeLink nodeLink = this.links[i];
				IModule3D moduleFromLink = this.GetModuleFromLink(nodeLink.target);
				if (nodeLink.target.type == Link.Type.Terminator)
				{
					module3D = this.GetModuleFromLink(nodeLink.source0);
				}
				else
				{
					switch (nodeLink.target.type)
					{
					case Link.Type.Filter:
					{
						IModule3D module3D2 = this.GetModuleFromLink(nodeLink.source0);
						this.filters[nodeLink.target.name].SetSouces(moduleFromLink, module3D2);
						((FilterModule)moduleFromLink).Primitive3D = module3D2;
						break;
					}
					case Link.Type.Transformer:
					{
						IModule3D module3D2 = this.GetModuleFromLink(nodeLink.source0);
						IModule3D module3D3 = this.GetModuleFromLink(nodeLink.source1);
						IModule3D module3D4 = this.GetModuleFromLink(nodeLink.source2);
						IModule3D moduleFromLink2 = this.GetModuleFromLink(nodeLink.source3);
						this.transformers[nodeLink.target.name].SetSouces(moduleFromLink, module3D2, module3D3, module3D4, moduleFromLink2);
						break;
					}
					case Link.Type.Selector:
					{
						IModule3D module3D2 = this.GetModuleFromLink(nodeLink.source0);
						IModule3D module3D3 = this.GetModuleFromLink(nodeLink.source1);
						IModule3D module3D4 = this.GetModuleFromLink(nodeLink.source2);
						this.selectors[nodeLink.target.name].SetSouces(moduleFromLink, module3D4, module3D2, module3D3);
						break;
					}
					case Link.Type.Modifier:
					{
						IModule3D module3D2 = this.GetModuleFromLink(nodeLink.source0);
						ControlPointList controlPointList = null;
						if (nodeLink.source1 != null && nodeLink.source1.type == Link.Type.ControlPoints && this.controlpoints.ContainsKey(nodeLink.source1.name))
						{
							controlPointList = this.controlpoints[nodeLink.source1.name];
						}
						FloatList floatList = null;
						if (nodeLink.source1 != null && nodeLink.source1.type == Link.Type.FloatPoints && this.floats.ContainsKey(nodeLink.source1.name))
						{
							floatList = this.floats[nodeLink.source1.name];
						}
						this.modifiers[nodeLink.target.name].SetSouces(moduleFromLink, module3D2, floatList, controlPointList);
						break;
					}
					case Link.Type.Combiner:
					{
						IModule3D module3D2 = this.GetModuleFromLink(nodeLink.source0);
						IModule3D module3D3 = this.GetModuleFromLink(nodeLink.source1);
						this.combiners[nodeLink.target.name].SetSouces(moduleFromLink, module3D2, module3D3);
						break;
					}
					}
				}
			}
			Debug.Assert(module3D != null, "Missing Terminus module");
			return module3D;
		}

		public string[] GetPrimitiveNames()
		{
			string[] array = new string[this.primitives.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Primitive> keyValuePair in this.primitives)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}

		public string[] GetFilterNames()
		{
			string[] array = new string[this.filters.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Filter> keyValuePair in this.filters)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}

		private Dictionary<string, IModule3D> primitiveLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> filterLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> modifierLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> selectorLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> transformerLookup = new Dictionary<string, IModule3D>();

		private Dictionary<string, IModule3D> combinerLookup = new Dictionary<string, IModule3D>();
	}
}
