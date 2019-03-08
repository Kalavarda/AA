using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AA.UserControls
{
	public partial class CollectionControl
	{
		private Func<object, string> _onConvert;
		private Func<object, IComparable> _onOrder;
		private readonly ObservableCollection<object> _collection = new ObservableCollection<object>();

		public Func<object> OnAdd;
		
		public Action<object> OnEdit;
	
		public Func<object, bool> OnRemove;

		public Func<object, string> OnConvert
		{
			get { return _onConvert; }
			set
			{
				_onConvert = value;
				((ItemConverter)Resources["itemConverter"]).OnConvert = value;
			}
		}

		public Func<object, IComparable> OnOrder
		{
			get { return _onOrder; }
			set
			{
				_onOrder = value;
				itemsControl.ItemsSource = _collection.OrderBy(value);
			}
		}

		public CollectionControl()
		{
			InitializeComponent();

			_onConvert = obj => obj.ToString();
			_onOrder = obj => obj.ToString();

			itemsControl.ItemsSource = _collection;

			OnSelectionChanged(this, null);
		}

		private void OnAddClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var newObject = OnAdd();
				if (newObject == null)
					return;

				_collection.Add(newObject);
				itemsControl.ItemsSource = _collection.OrderBy(OnOrder);
				itemsControl.SelectedItem = newObject;
				itemsControl.Focus();
			}
			catch (Exception error)
			{
				ShowError(error);
			}
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			btnEdit.IsEnabled = itemsControl.SelectedItem != null;
			btnRemove.IsEnabled = itemsControl.SelectedItems.Count > 0;
			btnClear.IsEnabled = _collection.Any();
		}

		private void OnEditClick(object sender, RoutedEventArgs e)
		{
			var item = itemsControl.SelectedItem;
			OnEdit(item);

			itemsControl.ItemsSource = null;
			itemsControl.ItemsSource = _collection.OrderBy(OnOrder);
			itemsControl.SelectedItem = item;
			itemsControl.Focus();
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
				OnEditClick(sender, e);
		}

		private void OnRemoveClick(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Удаление", "Удалить выбранные элементы?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
				return;

			try
			{
				var toRemove = itemsControl.SelectedItems.Cast<object>().ToList();

				foreach (var item in toRemove.Where(item => OnRemove(item)))
					_collection.Remove(item);

				itemsControl.ItemsSource = null;
				itemsControl.ItemsSource = _collection.OrderBy(OnOrder);
				itemsControl.Focus();
			}
			catch (Exception error)
			{
				ShowError(error);
			}
		}

		private static void ShowError(Exception error)
		{
			MessageBox.Show(error.Message, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		private void OnClearClick(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Удаление", "Удалить все элементы?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
				return;

			var toRemove = _collection.ToList();

			foreach (var item in toRemove.Where(item => OnRemove(item)))
				_collection.Remove(item);

			itemsControl.ItemsSource = null;
			itemsControl.ItemsSource = _collection.OrderBy(OnOrder);
			itemsControl.Focus();
		}

		public void Init(IEnumerable<object> items)
		{
			foreach (var item in items)
				_collection.Add(item);
			itemsControl.ItemsSource = _collection.OrderBy(OnOrder);
			OnSelectionChanged(this, null);
		}

		public void Clear()
		{
			_collection.Clear();
		}

		public void Refresh()
		{
			var selectedItem = itemsControl.SelectedItem;
			itemsControl.ItemsSource = null;
			itemsControl.ItemsSource = _collection.OrderBy(OnOrder);
			itemsControl.SelectedItem = selectedItem;
		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
		{
			var text = ((TextBox)sender).Text;
			if (string.IsNullOrWhiteSpace(text))
			{
				Refresh();
				return;
			}

			var filtered = (from item in _collection
							let s = OnConvert != null ? OnConvert(item) : item.ToString()
							where s.ToLowerInvariant().Contains(text.ToLowerInvariant())
							select item)
				.ToList();
			itemsControl.ItemsSource = filtered.OrderBy(OnOrder);
		}
	}

	public class ItemConverter: IValueConverter
	{
		public Func<object, string> OnConvert { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return OnConvert == null ? value.ToString() : OnConvert(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
