using System;
using System.ComponentModel;

namespace TemplateClasses
{
	[Serializable]
	public class Cell
	{
		public Cell()
		{
		}

		public Cell(int loc_x, int loc_y, int gameCell)
		{
			this.location_x = loc_x;
			this.location_y = loc_y;
			this.element = Grid.Element[gameCell].id;
			this.temperature = Grid.Temperature[gameCell];
			this.mass = Grid.Mass[gameCell];
			this.diseaseName = ((Grid.DiseaseIdx[gameCell] != byte.MaxValue) ? Db.Get().Diseases[(int)Grid.DiseaseIdx[gameCell]].Id : null);
			this.diseaseCount = Grid.DiseaseCount[gameCell];
			this.preventFoWReveal = Grid.PreventFogOfWarReveal[gameCell];
			if (BackwallManager.HasBackwall(gameCell))
			{
				this.backwallElement = BackwallManager.At(gameCell).Element.id;
				this.backwallTemperature = BackwallManager.At(gameCell).Temperature;
				this.backwallMass = BackwallManager.At(gameCell).Mass;
				return;
			}
			this.backwallElement = SimHashes.Vacuum;
			this.backwallTemperature = 0f;
			this.backwallMass = 0f;
		}

		public Cell(int loc_x, int loc_y, SimHashes _element, float _temperature, float _mass, string _diseaseName, int _diseaseCount, bool _preventFoWReveal = false, SimHashes _backwallElement = SimHashes.Vacuum, float _backwallMass = 0f, float _backwallTemperature = 0f)
		{
			this.location_x = loc_x;
			this.location_y = loc_y;
			this.element = _element;
			this.temperature = _temperature;
			this.mass = _mass;
			this.diseaseName = _diseaseName;
			this.diseaseCount = _diseaseCount;
			this.preventFoWReveal = _preventFoWReveal;
			this.backwallElement = _backwallElement;
			this.backwallMass = _backwallMass;
			this.backwallTemperature = _backwallTemperature;
		}

		public SimHashes element { get; set; }

		public float mass { get; set; }

		public float temperature { get; set; }

		public string diseaseName { get; set; }

		public int diseaseCount { get; set; }

		public int location_x { get; set; }

		public int location_y { get; set; }

		public bool preventFoWReveal { get; set; }

		[DefaultValue(SimHashes.Vacuum)]
		public SimHashes backwallElement { get; set; }

		public float backwallTemperature { get; set; }

		public float backwallMass { get; set; }
	}
}
