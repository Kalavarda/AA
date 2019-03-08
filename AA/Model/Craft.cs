using System;
using System.Linq;

namespace AA.Model
{
	public class Craft
	{
		public CraftItem[] CraftItems { get; set; }

		public Craft()
		{
			CraftItems = new CraftItem[0];
			Count = 1;
		}

		public string ToString(Data data)
		{
			var buyCost = GetBuyCost(data);
			if (buyCost == null)
				return string.Join(", ", CraftItems.Select(ci => ci.ToString(data)));
			return Utils.ToString(buyCost.Value / Count) + data.MainCurrency.ShortName;
		}

		public void Add(CraftItem craftItem)
		{
			CraftItems = CraftItems.Add(craftItem);
		}

		public void Remove(CraftItem craftItem)
		{
			CraftItems = CraftItems.Remove(craftItem);
		}

		/// <summary>
		/// Сколько единиц ресурса получится в результате крафта
		/// </summary>
		public int Count { get; set; }

		public Guid ProfessionId { get; set; }

		/// <summary>
		/// Стоимость покупки за основную валюту
		/// </summary>
		public decimal? GetBuyCost(Data data)
		{
			if (!CraftItems.Any())
				return null;

			var sum = 0m;
			foreach (var craftItem in CraftItems)
			{
				var cost = craftItem.GetBuyPrice(data);
				if (cost == null)
					return null;

				var value = cost.Value;
				if (craftItem.ItemId == Item.WorkScoreId)
				{
					var pro = data.Professions.FirstOrDefault(p => p.Id == ProfessionId);
					if (pro != null && pro.Level > 0)
					{
						var oldCount = craftItem.Count;
						try
						{
							craftItem.Count = Math.Round(craftItem.Count * pro.GetRatio());
							value = craftItem.GetBuyPrice(data).Value;
						}
						finally
						{
							craftItem.Count = oldCount;
						}
					}
				}

				sum += value;
			}
			return sum;
		}
	}

	public class CraftItem
	{
		public Guid ItemId { get; set; }

		public decimal Count { get; set; }

		public CraftItem()
		{
			Count = 1;
		}

		public string ToString(Data data)
		{
			var item = data.Items.First(i => i.Id == ItemId);
			var buyCost = item.GetBuyCost(data, Count);
			var s = Count + " " + item.Name;
			if (buyCost != null)
				s += " " + Utils.ToString(buyCost.Value) + data.MainCurrency.ShortName;
			return s;
		}

		/// <summary>
		/// Стоимость покупки за основную валюту
		/// </summary>
		public decimal? GetBuyPrice(Data data)
		{
			var item = data.Items.First(i => i.Id == ItemId);
			return item.GetBuyCost(data, Count);
		}
	}
}
