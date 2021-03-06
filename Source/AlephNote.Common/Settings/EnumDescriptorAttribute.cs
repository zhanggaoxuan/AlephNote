﻿using System;

namespace AlephNote.Common.Settings
{
	[AttributeUsage(AttributeTargets.All)]
	public class EnumDescriptorAttribute : Attribute
	{
		public readonly string Description;
		public readonly bool Visible;

		public EnumDescriptorAttribute(string value, bool visible = true)
		{
			Description = value;
			Visible = visible;
		}
	}
}
