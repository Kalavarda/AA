using System;
using System.Linq;
using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class PriceWindow
	{
		private readonly Price _price;

		public PriceWindow()
		{
			InitializeComponent();
		}

		public PriceWindow(Price price, Data data): this()
		{
			_price = price;

			cbVendor.ItemsSource = data.Vendors.OrderBy(v => v.Name);
			cbVendor.SelectedItem = data.Vendors.FirstOrDefault(v => v.Id == price.VendorId);

			cbCurrency.ItemsSource = data.Items.OrderBy(v => v.Name);
			cbCurrency.SelectedItem = data.Items.FirstOrDefault(v => v.Id == price.CurrencyId);

			tbValue.Text = price.Value.ToString();
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var vendor = (Vendor)cbVendor.SelectedItem;
				var currency = (Item)cbCurrency.SelectedItem;

				_price.VendorId = vendor.Id;
				_price.CurrencyId = currency.Id;
				var count = tbCount.Text.ToDecimal();
				var price = tbValue.Text.ToDecimal() / count;
				if (_price.Value != price)
					_price.Value = price;

				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowWarning(error.Message);
			}
		}
	}
}
