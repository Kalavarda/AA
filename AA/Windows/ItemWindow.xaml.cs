using System;
using System.Linq;
using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class ItemWindow
	{
		private readonly Item _item;

		public ItemWindow()
		{
			InitializeComponent();
		}

		public ItemWindow(Item item, Data data): this()
		{
			_item = item;

			tbName.Text = item.Name;
			tbShortName.Text = item.ShortName;
			cbIsMain.IsChecked = item.IsMainCurrency;

			#region buyPrices

			buyPrices.Init(item.BuyPrices);
			
			buyPrices.OnAdd = () =>
			{
				var price = new Price();
				var result = new PriceWindow(price, data) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					item.BuyPrices = item.BuyPrices.Add(price);
					return price;
				}
				return null;
			};

			buyPrices.OnEdit = obj =>
			{
				new PriceWindow((Price)obj, data) { Owner = this }.ShowDialog();
			};

			buyPrices.OnRemove = obj =>
			{
				item.BuyPrices = item.BuyPrices.Remove((Price)obj);
				return true;
			};

			buyPrices.OnConvert = obj =>
			{
				var price = (Price) obj;
				return price.ToString(data);
			};

			#endregion

			#region salePrices

			salePrices.Init(item.SalePrices);

			salePrices.OnAdd = () =>
			{
				var price = new Price();
				var result = new PriceWindow(price, data) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					item.SalePrices = item.SalePrices.Add(price);
					return price;
				}
				return null;
			};

			salePrices.OnEdit = obj =>
			{
				new PriceWindow((Price)obj, data) { Owner = this }.ShowDialog();
			};

			salePrices.OnRemove = obj =>
			{
				item.SalePrices = item.SalePrices.Remove((Price)obj);
				return true;
			};

			salePrices.OnConvert = obj =>
			{
				var price = (Price)obj;
				return price.ToString(data);
			};

			#endregion

			#region craft

			crafts.Init(item.Crafts);

			crafts.OnAdd = () =>
			{
				var craft = new Craft();
				var result = new CraftWindow(craft, item, data) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					item.Crafts = item.Crafts.Add(craft);
					return craft;
				}
				return null;
			};

			crafts.OnEdit = obj =>
			{
				new CraftWindow((Craft)obj, item, data) { Owner = this }.ShowDialog();
			};

			crafts.OnRemove = obj =>
			{
				item.Crafts = item.Crafts.Remove((Craft)obj);
				return true;
			};

			crafts.OnConvert = obj =>
			{
				var craft = (Craft)obj;
				return craft.ToString(data);
			};

			#endregion

			var stock = _item.Stocks.FirstOrDefault(st => st.CharacterName == App.CurrentCharacter.Name);
			if (stock != null)
				tbStock.Text = stock.Value.ToString();
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				_item.Name = tbName.Text;
				_item.ShortName = tbShortName.Text;

				var stock = _item.Stocks.FirstOrDefault(st => st.CharacterName == CurrentCharacter.Name);
				if (!string.IsNullOrWhiteSpace(tbStock.Text) && int.Parse(tbStock.Text) > 0)
				{
					if (stock == null)
					{
						stock = new Stock { CharacterName = App.CurrentCharacter.Name };
						_item.Add(stock);
					}
					stock.Value = int.Parse(tbStock.Text);
				}
				else
					if (stock != null)
						_item.Remove(stock);

				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowWarning(error.Message);
			}
		}

		private static Character CurrentCharacter
		{
			get { return App.CurrentCharacter; }
		}
	}
}
