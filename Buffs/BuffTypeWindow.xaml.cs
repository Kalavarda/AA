using System;
using System.Windows;
using Microsoft.Win32;

namespace Buffs
{
	public partial class BuffTypeWindow
	{
		private readonly BuffType _buffType;

		public BuffTypeWindow()
		{
			InitializeComponent();
		}

		public BuffTypeWindow(BuffType buffType): this()
		{
			_buffType = buffType;

			_fileName.Text = _buffType.ImageUri;
			_duration.Text = _buffType.Duration.ToString();
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			_buffType.ImageUri = _fileName.Text;
			_buffType.Duration = TimeSpan.Parse(_duration.Text);

			DialogResult = true;
		}

		private void OnBrowseClick(object sender, RoutedEventArgs e)
		{
			var fileDialog = new OpenFileDialog();
			if (fileDialog.ShowDialog() == true)
				_fileName.Text = fileDialog.FileName;
		}
	}
}
