using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Trading_Sidekick.Data
{
	public class Item
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string Rarity { get; set; }
		public int Id { get; set; }
		public string Icon { get; set; }
	}

	public class ItemReference
	{
		public string Name { get; set; }
		public int Id { get; set; }
	}
}
