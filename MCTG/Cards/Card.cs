using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Enums;

namespace MTCG.Cards
{
	public class Card
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
		public int AttackPoints { get; set; }
		public ElementsEnum.Elements Element { get; set; }
	}
}
