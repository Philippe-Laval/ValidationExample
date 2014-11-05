using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using Caliburn.Micro.Validation;

namespace ValidationExample
{
	/// <summary>
	/// Demos some validation techniques
	/// </summary>
	[Export(typeof(IShell))]
	public class ShellViewModel : ValidatingScreen<ShellViewModel>, IShell
	{
		#region Backing variables

		private string _privateEmailAddressMember;
		private int _age;

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ShellViewModel()
		{
			Age = "25";
			EmailAddress = "example@mydomain.com";
			Password = "aZxxS1@34d";
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Allows this class to refine the error(s) reported
		/// </summary>
		/// <param name="columnName">The name of the field being validated</param>
		/// <param name="errors">Any existing errors and their type.</param>
		protected override void OnColumnrror(string columnName, System.Collections.Generic.Dictionary<System.Type, string> errors)
		{
			// Do nothing at the moment.
		}

		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			ShellView shellView = view as ShellView;

			shellView.Password.Password = Password;
		}

		#endregion

		#region Bound and validated properties

		/// <summary>
		/// Property bound automatically to the EmailAddress element in the view
		/// The text box entry is validated but because the property is decorated
		/// with a non-default group name but the Close button validates on the
		/// default group name its validation state will not affect the Close button
		/// </summary>
		[Required(ErrorMessage="Email is required")]
		[EmailValidator(ErrorMessage="The format of the email address is not valid")]
		[ValidationGroup(IncludeInErrorsValidation=false, GroupName="ExcludeFromButton")]
		public string EmailAddress
		{
			get { return _privateEmailAddressMember; }
			set
			{
				_privateEmailAddressMember = value;
				NotifyOfPropertyChange(() => EmailAddress);

				// Bump the NoValidationErrors property so the IsEnabled state of the close button will be updated
				NotifyOfPropertyChange(() => NoValidationErrors);
			}
		}

		/// <summary>
		/// The age property is validated by decorating the 
		/// property with the built in Range ValiationAttribute
		/// </summary>
		/// <remarks>
		/// This example also shows how to use messages which 
		/// can be localized.  In this case the string will be
		/// pulled from the resources.resx file.  Note the
		/// Access Modifier of the resources must be set to
		/// 'Public' for this to work.  If the string is pulled
		/// from some other class property that property must
		/// also be declared public.
		/// </remarks>
		[Range(18, 65, ErrorMessageResourceType = typeof(Properties.Resources), ErrorMessageResourceName = "ErrorMessageAge")]
		public string Age
		{
			get { return _age.ToString(); }
			set
			{
				_age = 0;
				int.TryParse(value, out _age);
				NotifyOfPropertyChange(() => Age);

				// Bump the NoValidationErrors property so the IsEnabled state of the close button will be updated
				NotifyOfPropertyChange(() => NoValidationErrors);
			}
		}

		/// <summary>
		/// Example guard method
		/// </summary>
		public bool Guard { get; set; }

		string password = "";
		/// <summary>
		/// Collects a strong password
		/// </summary>
		/// <remarks>
		/// The RequiredEx validation attribute illustrates the options which can be used to control
		/// the validation.  By default the validator will look for an use the value of a guard 
		/// property which will be called CanPassword in this case.  But the guard property 'Guard'
		/// will be used in this case.
		/// </remarks>
		[RequiredEx(ErrorMessage = "A password is required", GuardProperty = "Guard", ValidateWhileDisabled = false)]
		[StrongPasswordValidator(ErrorMessage="The password must be at least 8 characters and contain 1 lower-case, 1 upper-case, 1 number and 1 of &^%$#@!")]
		public string Password
		{
			get { return password; }
			set
			{
				password = value;
				NotifyOfPropertyChange(() => Password);

				// Bump the NoValidationErrors property so the IsEnabled state of the close button will be updated
				NotifyOfPropertyChange(() => NoValidationErrors);
			}
		}

		/// <summary>
		/// The PasswordBox.Password cannot be bound because its not a dependency property
		/// So the Xaml is marked with the Caliburn Message.Attach attached property to
		/// fire this method.  It can then save the password and update the notification.
		/// </summary>
		/// <param name="control"></param>
		public void PasswordChanged(Control control)
		{
			if (!(control is PasswordBox)) return;
			this.Password = (control as PasswordBox).Password;
		}

		#endregion

		#region Close button

		/// <summary>
		/// This property is bound to the IsEnabled property of the 'Close' 
		/// button so the enabled state can be controlled by the validation 
		/// state of all controls
		/// </summary>
		/// <remarks>HasErrors is implemented by ValidatingScreen</remarks>
		public bool NoValidationErrors
		{
			get { return !HasErrorsByGroup();  }
		}

		/// <summary>
		/// Just displays an acknowledgement that the button was pressed
		/// </summary>
		public void Close()
		{
			System.Windows.MessageBox.Show("Can close");
		}

		#endregion
	}
}
