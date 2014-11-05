PLL (05/11/2014)
Ported the example found on the blog below to Caliburn.micro 2.x

Easy property validation for Caliburn Micro
http://www.lyquidity.com/devblog/?p=71

----------------------------------------------------------------------

Caliburn Micro for WPF
http://caliburnmicro.codeplex.com/

Update Feb 18th

Added an example of validating a password entered using the PasswordBox
element.  The main property of the PasswordBox, Password, is not a
dependency property so cannot be bound.  This make view model validation
non-standard.

Update Feb 19th

Added support for validation groups so properties can be validated within 
named groups.  New HasErrorsByGroup methods allow a caller to define the
group name to use.

Update Feb 22nd

Added support for validating only when a guard property is enabled.  By
default the guard property is Can[PropertyName].  But the guard property 
a validation controller will use can be set explicitly.

Building the application

The application has been created using VS 2010 and .NET Framework 4.0. 
If you'd like a version which uses .NET 3.5 (and a functioning copy of
CM which works with .NET 3.5) email caliburnexample@lyquidity.com

To compile you will need to add a reference to Caliburn.Micro.dll and to
the blend System.Windows.Interactivity.dll.

Background

This example shows how property validation can be defined for any CM screen
just by decorating the properties with ValidationAttribute instances such
as [Required()], [Range(<min>,<max>)] or your own custom validation
attributes.

The CM ConfigurationManager will enable validation if the model being bound 
to a screen implements IDataErrorInfo.  However I think its fairly common, 
especially in quick and dirty application written to get a job done, that 
either:

1) validation rules are defined in Xaml which means the model is no longer
   source of all logic for the form so model testing cannot cover all
   scenarios; or
2) there's a big, custom switch statement in the 'IDataErrorInfo.Items'
   property of the model.

ValidatingScreen

This example sub-classes the CM Screen implementation to provide a common class
from which your view model can be derived.  This new class, here called 
'ValidatingScreen', adds the logic necessary to determine if a property has 
been decorated with one or more validators and, if so, will test each 
validators IsValid(<propertyName>) method and return the corresponding error 
message if any validator is not valid.

This mechanism makes it really simple to validate property values and provide a
corresponding validation failure message so WPF error templates will be activated 
automatically.  There's no need to mess around with IDataErrorInfo as it all 
done automatically.  All you have to is decorate properties which are bound to 
your view. For example, suppose there's a view field to collect an email 
address which must be entered.  You might create a view model class like
(irrelevant details omitted):

public class MyViewModel : ValidatingScreen<MyViewModel>, IShell, IActivate, ...
{
	...

	[Required(ErrorMessage="Email is required")];
	[EmailValidator(ErrorMessage="The format of the email address is not valid")]
	pubic string EmailAddress
	{
		get { return _privateEmailAddressMember; }
		set
		{
			_privateEmailAddressMember = value;
			NotifyOfPropertyChange(() => EmailAddress);
		}
	}
}

Even the bulk of this example is boilerplate code.  The relevant lines are
the attributes decorating the EmailAddress property.  With these declarations
the ValidatingScreen implementation will ensure errors are raised when either
condition is not valid.  It will also ensure the corresponding message is
returned by the IDataErrorInfo.Item property so WPF can report it.

Validators

Part of this simplicity is possible because of the validators.  The validators
are instances of sub-classes of the abstract ValidationAttribute a class 
introduced in .NET 3.5 mainly to simplify validation in ASP.NET MVC. 

A ValidationAttribute instance performs much the same role as ValidationRule
but can be declared on the affected property and so, importantly, are part of
the view model so all unit testing can include the effect of validation errors.

The .NET Framework provides some example ValidationAttribute implementations 
such as the RequiredAttribute used in the example, RangeAttribute, 
RegularExpressionAttribute and StringLengthAttribute.

A custom ValidationAttribute is easy to define and can be as complex as 
required.  The EmailValidatorAttribute used in the example shows how simple it
can be to roll your own validator.  This example is lifted from Scott Guthries
blog post which introduces this decorative style of property validation.

Importantly for me, a European, the base ValidationAttribute class supports
localization.  The exmaple used hard coded error messages.  But when a 
ValidationAttribute instance is used to decorate a property the string to
report can be defined as a string resource by using the option to specify
a class and property from which the string should be retrieved.  The example
shows this option being used.

Note: The ValidationAttribute class can also be used to decorate methods and
      method parameters but these two features are not supported by the
	  ValidatingScreen implementation.  You can still declare validators this
	  way but they will be ignored by the ValidatingScreen class.

WPF ErrorTemplate

Generating an error and message is one part of the example.  The other is
reporting it in the view.  The example uses the control error template created
by Beth Massi in her post about displaying data validation messages in WPF.

In her article Beth creates a reusable template defined to be used as a 
static resource in Xaml which can then be applied as the Validation.ErrorTemplate
for many controls.

In the example the template is defined like:

<Style x:Key="myErrorTemplate" TargetType="Control">
	...
</Style>

And is applied to a control style, here a TextBox, as:

<Style TargetType="TextBox" BasedOn="{StaticResource myErrorTemplate}" >
	... any other style defintions ...
</Style>

In the main Xaml then any <TextBox /> instances will report validation errors
using the template.  See Beth's article or run the example to see how it looks.
Of course you don't have to stick with this visual you can create your own.

Summary

The example shows how it's possible, dropping a couple of 'black-boxes' into
your application code and with only declarative 'coding' (you need to change 
the parent of the view model, decorate your view bound properties with 
validators and set the error template on your Xaml controls) you can have
great validation and reporting in your application no matter how rough and
ready it is.

References

The main idea for this example comes from this post:
http://caliburnmicro.codeplex.com/discussions/243212

There are a couple of changes to this original post:

1) the class inherits from Screen so it can provide the functionality for
   a CM application easily.
2) the errors are reported using ValidationAttribute.FormatErrorMessage not
   ValidationAttribute.ErrorMessage because the former supports localization.


Beth Massi's article about error templates can be found here:
http://blogs.msdn.com/b/bethmassi/archive/2008/06/27/displaying-data-validation-messages-in-wpf.aspx

Bill Seddon
Lyquidity Solutions Limited
http://www.lyquidity.com
