﻿using MSHC.WPF.MVVM;
using System;

namespace CommonNote.WPF.Converter
{
	class DateTimeToDisplay : OneWayConverter<DateTimeOffset, string>
	{
		public DateTimeToDisplay() { }

		protected override string Convert(DateTimeOffset value, object parameter)
		{
			return value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
		}
	}
}