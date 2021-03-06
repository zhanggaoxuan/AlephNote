﻿
using AlephNote.Common.Themes;
using AlephNote.WPF.Converter;

namespace AlephNote.WPF.Extensions
{
	public static class AlephThemeExtension
	{
		public static System.Drawing.Color ToDCol(this ColorRef cref)
		{
			return System.Drawing.Color.FromArgb(cref.A, cref.R, cref.G, cref.B);
		}

		public static System.Windows.Media.Color ToWCol(this ColorRef cref)
		{
			return System.Windows.Media.Color.FromArgb(cref.A, cref.R, cref.G, cref.B);
		}

		public static System.Windows.Media.Brush ToWBrush(this ColorRef cref)
		{
			return ColorRefToBrush.Convert(cref);
		}

		public static System.Windows.Media.Brush ToWBrush(this BrushRef cref)
		{
			return BrushRefToBrush.Convert(cref);
		}

		public static System.Windows.Thickness ToWThickness(this ThicknessRef tref)
		{
			return new System.Windows.Thickness(tref.Left, tref.Top, tref.Right, tref.Bottom);
		}

		public static System.Windows.CornerRadius ToCornerRad(this CornerRadiusRef tref)
		{
			return new System.Windows.CornerRadius(tref.Left, tref.Top, tref.Right, tref.Bottom);
		}
	}
}
