using System;
using System.Linq;
using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class CraftItemWindow
	{
		private readonly CraftItem _craftItem;
		private readonly Data _data;

		public CraftItemWindow()
		{
			InitializeComponent();
		}

		public CraftItemWindow(CraftItem craftItem, Data data): this()
		{
			_craftItem = craftItem;
			_data = data;

			cbItem.ItemsSource = data.Items.OrderBy(i => i.Name);
			cbItem.SelectedItem = data.Items.FirstOrDefault(i => i.Id == craftItem.ItemId);

			tbCount.Text = craftItem.Count.ToString();
			
			TuneControls();
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				_craftItem.ItemId = ((Item) cbItem.SelectedItem).Id;

				_craftItem.Count = tbCount.Text.ToDecimal();

				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowWarning(error.Message);
			}
		}

		private void tbCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			TuneControls();
		}

		private void TuneControls()
		{
			if (_data == null)
				return;

			try
			{
				var item = (Item) cbItem.SelectedItem;
				var count = tbCount.Text.ToInt();

				var buyPrice = item.GetBuyCost(_data, count);
				if (buyPrice != null)
					tbSum.Text = Utils.ToString(buyPrice.Value) + _data.MainCurrency.ShortName;
				else
					tbSum.Text = string.Empty;
			}
			catch
			{
				tbSum.Text = string.Empty;
			}
		}

		private void cbItem_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			TuneControls();
		}
	}
}
