using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AA.Model;

namespace AA.Windows
{
	public partial class CalcCraftWindow
	{
		private readonly Craft _craft;
		private readonly int _count;
		private readonly Data _data;

		public CalcCraftWindow()
		{
			InitializeComponent();
		}

		public CalcCraftWindow(Craft craft, int count, Data data)
			: this()
		{
			_craft = craft;
			_count = count;
			_data = data;
			if (craft == null) throw new ArgumentNullException("craft");
			if (count < 1) throw new ArgumentNullException("count");
			if (data == null) throw new ArgumentNullException("data");

			Title = "Craft " + data.Items.First(i => i.Crafts.Contains(craft)).Name;

			TuneControls(craft, count, data);
		}

		private void TuneControls(Craft craft, int count, Data data)
		{
			_panel.Children.Clear();
			foreach (var craftItem in craft.CraftItems)
			{
				var item = data.Items.First(i => i.Id == craftItem.ItemId);
				var stock = item.Stocks.FirstOrDefault(st => st.CharacterName == App.CurrentCharacter.Name);
				var exists = stock != null ? stock.Value : 0;
				var need = craftItem.Count*count;

				var textBlock = new TextBlock
				{
					Text = string.Format("{0}   {1}/{2}", item.Name, exists, need),
					Foreground = exists >= need ? Brushes.Black : Brushes.Red,
					Tag = item
				};
				textBlock.MouseLeftButtonDown += textBlock_MouseLeftButtonDown;
				_panel.Children.Add(textBlock);
			}

			var nonCraftableItems = new Dictionary<Item, decimal>();
			foreach (var tuple in GetNonCraftableItems(craft, data))
			{
				if (!nonCraftableItems.ContainsKey(tuple.Item1))
					nonCraftableItems.Add(tuple.Item1, 0);
				nonCraftableItems[tuple.Item1] += tuple.Item2;
			}

			var list = new List<Tuple<Item, int, int, decimal>>();
			foreach (var pair in nonCraftableItems)
			{
				var item = pair.Key;
				var stock = item.Stocks.FirstOrDefault(st => st.CharacterName == App.CurrentCharacter.Name);
				var exists = stock != null ? stock.Value : 0;
				var need = (int)(pair.Value * count);

				var dSum = 0m;
				if (exists < need)
					if (item.BuyPrices.Any(p => p.CurrencyId == Item.GoldId))
					{
						var price = item.BuyPrices.First(p => p.CurrencyId == Item.GoldId);
						dSum = (need - exists) * price.Value;
					}

				list.Add(new Tuple<Item, int, int, decimal>(item, exists, need, dSum));
			}

			_panel2.Children.Clear();
			var gold = data.Items.First(i => i.Id == Item.GoldId);
			foreach (var tuple in list.OrderByDescending(t => t.Item4))
			{
				var exists = tuple.Item2;
				var need = tuple.Item3;

				var text = string.Format("{0}   {1}/{2}", tuple.Item1.Name, exists, need);
				if (exists < need)
				{
					text += "   (нужно " + (need - exists) + ")";
					if (tuple.Item4 > 0)
						text += "   " + Utils.ToString(tuple.Item4) + " " + gold.ShortName;
				}

				var textBlock = new TextBlock
				{
					Text = text,
					Foreground = exists >= need ? Brushes.Black : Brushes.Red,
					Tag = tuple.Item1
				};
				textBlock.MouseLeftButtonDown += textBlock_MouseLeftButtonDown;
				_panel2.Children.Add(textBlock);
			}

			tbTotal.Text = Utils.ToString(list.Sum(t => t.Item4)) + " " + gold.ShortName;
		}

		void textBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				var item = (Item)((FrameworkElement)sender).Tag;
				if (new ItemWindow(item, _data) { Owner = this }.ShowDialog() == true)
					TuneControls(_craft, _count, _data);
			}
		}

		private static IReadOnlyCollection<Tuple<Item, decimal>> GetNonCraftableItems(Craft craft, Data data)
		{
			var list = new List<Tuple<Item, decimal>>();

			foreach (var craftItem in craft.CraftItems)
			{
				var item = data.Items.First(i => i.Id == craftItem.ItemId);
				if (item.Crafts.Any())
				{
					var firstCraft = item.Crafts.First();		// TODO: по идее надо определить самый дешёвый способ крафта
					for (var i = 0; i < craftItem.Count; i++)
						list.AddRange(GetNonCraftableItems(firstCraft, data));
				}
				else
					list.Add(new Tuple<Item, decimal>(item, craftItem.Count));
			}

			return list;
		}
	}
}
