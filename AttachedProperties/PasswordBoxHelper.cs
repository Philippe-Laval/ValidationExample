using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ValidationExample.AttachedProperties
{
	/// <summary>
	/// Facilitates binding to a PasswordBox.  The PasswordBox's password
	/// property is not a dependency property so cannot be bound.  This
	/// class provides a partial, not a complete solution to the binding
	/// issue.
	/// </summary>
	/// <example>
	/// This is how a PasswordBox might be defined.  Then you can bind to 
	/// the PasswordHelper.PasswordProperty
	/// <PasswordBox x:Name="Password" Grid.Column="1" Padding="0,0,20,0"  Margin="0,4"
	///				 properties:PasswordHelper.Attach="True"
	///	/>
	/// </example>
	/// <remarks>
	/// This is provided for information but is not used in this example.
	/// It is taken from this article:
	/// http://www.wpftutorial.net/PasswordBox.html
	/// </remarks>
	public static class PasswordHelper
	{
		public static readonly DependencyProperty PasswordProperty =
			DependencyProperty.RegisterAttached("Password",
			typeof(string), typeof(PasswordHelper),
			new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

		public static readonly DependencyProperty AttachProperty =
			DependencyProperty.RegisterAttached("Attach",
			typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

		private static readonly DependencyProperty IsUpdatingProperty =
		   DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
		   typeof(PasswordHelper));


		public static void SetAttach(DependencyObject dp, bool value)
		{
			dp.SetValue(AttachProperty, value);
		}

		public static bool GetAttach(DependencyObject dp)
		{
			return (bool)dp.GetValue(AttachProperty);
		}

		public static string GetPassword(DependencyObject dp)
		{
			return (string)dp.GetValue(PasswordProperty);
		}

		public static void SetPassword(DependencyObject dp, string value)
		{
			dp.SetValue(PasswordProperty, value);
		}

		private static bool GetIsUpdating(DependencyObject dp)
		{
			return (bool)dp.GetValue(IsUpdatingProperty);
		}

		private static void SetIsUpdating(DependencyObject dp, bool value)
		{
			dp.SetValue(IsUpdatingProperty, value);
		}

		private static void OnPasswordPropertyChanged(DependencyObject sender,
			DependencyPropertyChangedEventArgs e)
		{
			PasswordBox passwordBox = sender as PasswordBox;
			passwordBox.PasswordChanged -= PasswordChanged;

			if (!(bool)GetIsUpdating(passwordBox))
			{
				passwordBox.Password = (string)e.NewValue;
			}
			passwordBox.PasswordChanged += PasswordChanged;
		}

		private static void Attach(DependencyObject sender,
			DependencyPropertyChangedEventArgs e)
		{
			PasswordBox passwordBox = sender as PasswordBox;

			if (passwordBox == null)
				return;

			if ((bool)e.OldValue)
			{
				passwordBox.PasswordChanged -= PasswordChanged;
			}

			if ((bool)e.NewValue)
			{
				passwordBox.PasswordChanged += PasswordChanged;
			}
		}

		private static void PasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordBox passwordBox = sender as PasswordBox;
			SetIsUpdating(passwordBox, true);
			SetPassword(passwordBox, passwordBox.Password);
			SetIsUpdating(passwordBox, false);
		}
	}

}
