﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AlephNote.WPF.Controls
{
   [TemplatePart(Name = "PART_InputBox", Type = typeof(AutoCompleteBox))]
	[TemplatePart(Name = "PART_DeleteTagButton", Type = typeof(Button))]
	[TemplatePart(Name = "PART_TagButton", Type = typeof(Button))]
	public class TokenizedTagItem : Control
	{
		static TokenizedTagItem()
		{
			// lookless control, get default style from generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TokenizedTagItem), new FrameworkPropertyMetadata(typeof(TokenizedTagItem)));
		}

		private readonly TokenizedTagControl _parent;

		public TokenizedTagItem(string text, TokenizedTagControl parent)
		{
			_parent = parent;
			this.Text = text;
		}

		// Text
		public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TokenizedTagItem), new PropertyMetadata(null));

		// IsEditing, readonly
		public bool IsEditing { get { return (bool)GetValue(IsEditingProperty); } internal set { SetValue(IsEditingPropertyKey, value); } }
		private static readonly DependencyPropertyKey IsEditingPropertyKey = DependencyProperty.RegisterReadOnly("IsEditing", typeof(bool), typeof(TokenizedTagItem), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

		/// <summary>
		/// Wires up delete button click and focus lost 
		/// </summary>
		public override void OnApplyTemplate()
		{
			AutoCompleteBox inputBox = this.GetTemplateChild("PART_InputBox") as AutoCompleteBox;
			if (inputBox != null)
			{
				//inputBox.DropDownClosed +=inputBox_DropDownClosed;
//                inputBox.SelectionChanged += inputBox_SelectionChanged;
				inputBox.LostKeyboardFocus += inputBox_LostFocus; //GotFocus
//                inputBox.LostFocus += inputBox_LostFocus;
				inputBox.Loaded += inputBox_Loaded;
				/*
				inputBox.KeyDown += (s, e) =>
				{
					switch (e.Key)
					{
						case (Key.Enter):  // accept tag
							var parent = GetParent();
							if (parent != null)
								parent.RaiseTagClick(this); // raise the TagClick event of the TokenizedTagControl
							break;
					}
				};
				 * */
			}

			Button btn = this.GetTemplateChild("PART_TagButton") as Button;
			if (btn != null)
			{
				//btn.LostKeyboardFocus += inputBox_LostFocus;

				btn.Loaded += (s, e) =>
				{
					Button b = s as Button;
					var btnDelete = b.Template.FindName("PART_DeleteTagButton", b) as Button; // will only be found once button is loaded
					if (btnDelete != null)
					{
						btnDelete.Click -= btnDelete_Click; // make sure the handler is applied just once
						btnDelete.Click += btnDelete_Click;
					}
				};

				btn.Click += (s, e) => //btn.Click 
				{
					_parent.RaiseTagClick(this); // raise the TagClick event of the TokenizedTagControl

					if (_parent.IsSelectable)
					{
						//e.Handled = true;
						_parent.SelectedItem = this;
					}
					//parent.SelectedItem = this;
					//PART_TagBorder
				};

				btn.MouseDoubleClick += (s, e) =>
				{
					_parent.RaiseTagDoubleClick(this); // raise the TagClick event of the TokenizedTagControl
					if (_parent.IsSelectable)
					{
						//e.Handled = true;
						_parent.SelectedItem = this;
					}
				};

			}

			base.OnApplyTemplate();
		}


		/// <summary>
		/// Handles the click on the delete glyph of the tag button.
		/// Removes the tag from the collection.
		/// </summary>
		void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			var item = FindUpVisualTree<TokenizedTagItem>(sender as FrameworkElement);
			if (item != null) _parent.RemoveTag(item);

			e.Handled = true; // bubbling would raise the tag click event
		}

		/// <summary>
		/// When an AutoCompleteBox is created, set the focus to the textbox.
		/// Wire PreviewKeyDown event to handle Escape/Enter keys
		/// </summary>
		/// <remarks>AutoCompleteBox.Focus() is broken: http://stackoverflow.com/questions/3572299/autocompletebox-focus-in-wpf</remarks>
		void inputBox_Loaded(object sender, RoutedEventArgs e)
		{
			AutoCompleteBox acb = sender as AutoCompleteBox;
			
			if (acb != null)
			{
				var tb = acb.Template.FindName("Text", acb) as TextBox;

				if (tb != null)
					tb.Focus();
				/*
				acb.SelectionChanged += (s, e1) =>
				{
					var parent = GetParent();
					if (parent != null)
					{
						parent.InitializeNewTag();
					}
				};
				*/
				// PreviewKeyDown, because KeyDown does not bubble up for Enter
				acb.PreviewKeyDown += (s, e1) =>
				{
					switch (e1.Key)
					{
						case (Key.Enter):  // accept tag
							if (!string.IsNullOrWhiteSpace(this.Text))
							{
								_parent.OnApplyTemplate(this);
								_parent.SelectedItem = _parent.InitializeNewTag(); //creates another tag
							}
							else
								_parent.Focus();
							break;

						case (Key.Escape): // reject tag
							_parent.Focus();
							//parent.RemoveTag(this, true); // do not raise RemoveTag event
							break;

						case (Key.Back):
							if (string.IsNullOrWhiteSpace(this.Text))
							{
								inputBox_LostFocus(this, new RoutedEventArgs());
								var previousTagIndex = ((IList)_parent.ItemsSource).Count - 1;
								if (previousTagIndex < 0) break;

								//parent.RemoveTag((((IList)parent.ItemsSource)[previousTagIndex] as TokenizedTagItem));
								var previousTag = (((IList)_parent.ItemsSource)[previousTagIndex] as TokenizedTagItem);
								previousTag.Focus();
								previousTag.IsEditing = true;
							}
							//parent.Focus();
							//parent.RemoveTag(this, true); // do not raise RemoveTag event
							//((IList)parent.ItemsSource).RemoveAt(((IList)parent.ItemsSource).Count - 2);
							break;
					}
				};
			}
		}

		/// <summary>
		/// Set IsEditing to false when the AutoCompleteBox loses keyboard focus.
		/// This will change the template, displaying the tag as a button.
		/// </summary>
		void inputBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.Text))
			{
				if (!((AutoCompleteBox) sender).IsDropDownOpen)
				{
					this.IsEditing = false;
					//e.Handled = true;
				}
			}
			else
			{
				_parent.RemoveTag(this, true); // do not raise RemoveTag event
			}

			_parent.IsEditing = false;
		}
		/*
		private void inputBox_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
		{
			var parent = GetParent();
			this.IsEditing = false;
			if (parent != null)
			{
				parent.IsEditing = false;
			}
		}
		*/
//        void inputBox_SelectionChanged(object sender, RoutedPropertyChangedEventArgs e)
//        {
//        }

		/// <summary>
		/// Walks up the visual tree to find object of type T, starting from initial object
		/// http://www.codeproject.com/Tips/75816/Walk-up-the-Visual-Tree
		/// </summary>
		private static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
		{
			DependencyObject current = initial;
			while (current != null && current.GetType() != typeof(T))
			{
				current = VisualTreeHelper.GetParent(current);
			}
			return current as T;
		}
}
}
