using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTCG.Enums;

namespace MTCG.Cards
{
	public class Goblin : Monster
	{
		public Goblin (string id, string name, int attackPoints, ElementsEnum.Elements element)
        {
			Id = id;
			Name = name;
			AttackPoints = attackPoints;
			Element = element;
        }
	}
}
