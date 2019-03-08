using System;

namespace AA.Model
{
	public class Profession
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public int Level { get; set; }

		public Profession()
		{
			Level = 0;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1} ур.)", Name, Level);
		}

		public decimal GetRatio()
		{
			switch (Level)
			{
				case 0:
					return 1;
				case 1:
					return 0.95m;
				case 2:
					return 0.90m;
				case 3:
					return 0.85m;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
