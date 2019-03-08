using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Buffs.Properties;

namespace Buffs
{
	public partial class SettingsWindow
	{
		private readonly ICollection<BuffType> _buffTypes = new ObservableCollection<BuffType>();

		private BuffType SelectedBuffType
		{
			get { return _listBox.SelectedItem as BuffType; }
			set { _listBox.SelectedItem = value; }
		}

		public SettingsWindow()
		{
			InitializeComponent();

			foreach (var line in BuffType.Deserialize(Settings.Default.Types))
				_buffTypes.Add(line);

			_flashBefore.Text = Settings.Default.FlashBefore.ToString();
			_listBox.ItemsSource = _buffTypes;
		}

		private void OnSaveClick(object sender, RoutedEventArgs e)
		{
			Settings.Default.Types = new StringCollection();
			Settings.Default.Types.AddRange(_buffTypes.Select(bt => bt.Serialize()).ToArray());

			Settings.Default.FlashBefore = TimeSpan.Parse(_flashBefore.Text);

			Settings.Default.Save();
			DialogResult = true;
		}

		private void OnAddClick(object sender, RoutedEventArgs e)
		{
			var buffType = new BuffType();
			if (new BuffTypeWindow(buffType) { Owner = this }.ShowDialog() == true)
			{
				_buffTypes.Add(buffType);
				SelectedBuffType = buffType;
			}
		}

		private void OnEditClick(object sender, RoutedEventArgs e)
		{
			if (new BuffTypeWindow(SelectedBuffType) { Owner = this }.ShowDialog() == true)
			{
			}
		}

		private void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			_buffTypes.Remove(SelectedBuffType);
		}
	}
}
