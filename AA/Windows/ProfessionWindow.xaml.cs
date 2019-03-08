using System;
using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class ProfessionWindow
	{
		private readonly Profession _profession;

		public ProfessionWindow()
		{
			InitializeComponent();
		}

		public ProfessionWindow(Profession profession): this()
		{
			_profession = profession;
			tbName.Text = profession.Name;
			tbLevel.Text = profession.Level.ToString();
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				_profession.Name = tbName.Text;
				_profession.Level = tbLevel.Text.ToInt();

				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowWarning(error.Message);
			}
		}
	}
}
