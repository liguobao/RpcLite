﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public override string ToString()
		{
			return string.Format("Id {0},  Name {1}", Id, Name);
		}

		public int Price { get; set; }

		public string Category { get; set; }

		public DateTime ListDate { get; set; }
	}
}