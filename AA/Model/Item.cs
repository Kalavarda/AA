using System;
using System.Linq;

namespace AA.Model
{
	public class Item
	{
		/// <summary>
		/// Золото
		/// </summary>
		public static readonly Guid GoldId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		/// <summary>
		/// Очки работы
		/// </summary>
		public static readonly Guid WorkScoreId = Guid.Parse("22222222-2222-2222-2222-222222222222");

		/// <summary>
		/// Ремесленная репутация
		/// </summary>
		public static readonly Guid CraftsReputationId = Guid.Parse("33333333-3333-3333-3333-333333333333");

		public Guid Id { get; set; }

		public string Name { get; set; }

		public string ShortName { get; set; }

		public bool IsMainCurrency
		{
			get { return Id == GoldId; }
		}

		/// <summary>
		/// Варианты покупки (альтернативные)
		/// </summary>
		public Price[] BuyPrices { get; set; }

		/// <summary>
		/// Варианты продажи (альтернативные)
		/// </summary>
		public Price[] SalePrices { get; set; }

		public Craft[] Crafts { get; set; }

		public Stock[] Stocks { get; set; }

		/// <summary>
		/// Стоимость покупки за основную валюту
		/// </summary>
		public decimal? GetBuyCost(Data data, decimal count = 1)
		{
			if (data.MainCurrency == null)
				return null;

			var mainCurrencyId = data.MainCurrency.Id;
			var mainPrice = BuyPrices.FirstOrDefault(p => p.CurrencyId == mainCurrencyId);
			if (mainPrice != null)
				return mainPrice.Value * count;

			return null;
		}

		public Item()
		{
			BuyPrices = new Price[0];
			SalePrices = new Price[0];
			Crafts = new Craft[0];
			Stocks = new Stock[0];
		}

		public static Item CreateGoldItem()
		{
			return new Item
			{
				Id = GoldId,
				Name = "_Золото",
				ShortName = "г."
			};
		}

		public static Item CreateWorkScoreItem()
		{
			return new Item
			{
				Id = WorkScoreId,
				Name = "_Очки работы",
				ShortName = "ОР"
			};
		}

		public static Item CreateCraftsReputationItem()
		{
			return new Item
			{
				Id = CraftsReputationId,
				Name = "_Ремесленная репутация",
				ShortName = "РР"
			};
		}

		public override string ToString()
		{
			var stock = App.CurrentCharacter != null ? Stocks.FirstOrDefault(st => st.CharacterName == App.CurrentCharacter.Name) : null;
			if (stock != null)
				return Name + " (" + stock.Value + " шт.)";

			return Name;
		}

		public void Add(Stock stock)
		{
			if (stock == null) throw new ArgumentNullException("stock");

			if (Stocks.Any(st => st.CharacterName == stock.CharacterName))
				throw new Exception("Уже есть");

			Stocks = Stocks.Add(stock);
		}

		public void Remove(Stock stock)
		{
			if (stock == null) throw new ArgumentNullException("stock");

			var s = Stocks.FirstOrDefault(st => st.CharacterName == stock.CharacterName);
			if (s == null)
				throw new Exception("Нету такова");

			Stocks = Stocks.Remove(s);
		}
	}

	public class Stock
	{
		public string CharacterName { get; set; }

		public int Value { get; set; }
	}
}
