using System.Windows;
using AA.Model;

namespace AA
{
	public partial class App
	{
		public static void ShowWarning(string warning)
		{
			MessageBox.Show(warning, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static Character CurrentCharacter { get; set; }
	}
}
