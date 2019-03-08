using System;
using System;
using System.Linq;
using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class CraftWindow
	{
		private readonly Craft _craft;
		private readonly Data _data;

		public CraftWindow()
		{
			InitializeComponent();
		}

		public CraftWindow(Craft craft, Item item, Data data): this()
		{
			_craft = craft;
			_data = data;

			Title = "Рецепт: " + item.Name;

			collectionControl.Init(craft.CraftItems);

			collectionControl.OnAdd = () =>
			{
				var craftItem = new CraftItem();
				var result = new CraftItemWindow(craftItem, data) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					craft.Add(craftItem);
					TuneControls();
					return craftItem;
				}
				return null;
			};

			collectionControl.OnEdit = obj =>
			{
				new CraftItemWindow((CraftItem)obj, data) { Owner = this }.ShowDialog();
				TuneControls();
			};

			collectionControl.OnRemove = obj =>
			{
				craft.Remove((CraftItem)obj);
				TuneControls();
				return true;
			};

			collectionControl.OnConvert = obj =>
			{
				return ((CraftItem)obj).ToString(data);
			};

			tbCount.Text = craft.Count.ToString();

			cbProf.ItemsSource = data.Professions.OrderBy(p => p.ToString());
			cbProf.SelectedItem = data.Professions.FirstOrDefault(p => p.Id == craft.ProfessionId);

			TuneControls();
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var profession = cbProf.SelectedItem as Profession;
				_craft.ProfessionId = profession != null ? profession.Id : Guid.Empty;
				_craft.Count = tbCount.Text.ToInt();

				DialogResult = true;
			}
			catch (Exception error)
			{				
				App.ShowWarning(error.Message);
			}
		}

		private void TuneControls()
		{
			if (_data == null)
				return;

			try
			{
				var buyPrice = _craft.GetBuyCost(_data);
				var count = tbCount.Text.ToInt();
				if (buyPrice != null)
				{
					tbSum.Text = Utils.ToString(buyPrice.Value) + _data.MainCurrency.ShortName;
					tbSumPerOne.Text = Utils.ToString(buyPrice.Value / count) + _data.MainCurrency.ShortName;
				}
				else
					tbSum.Text = string.Empty;
			}
			catch
			{
				tbSum.Text = string.Empty;
			}
		}

		private void tbCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			TuneControls();
		}

		private void OnCalcClick(object sender, RoutedEventArgs e)
		{
			new CalcCraftWindow(_craft, int.Parse(tbCalcCount.Text), _data) { Owner = this }.ShowDialog();
		}
	}
}
