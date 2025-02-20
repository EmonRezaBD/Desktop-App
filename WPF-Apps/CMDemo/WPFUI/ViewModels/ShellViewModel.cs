using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace WPFUI.ViewModels
{
	public class ShellViewModel : Screen
	{
		private string _firstName = "Reza";

		public string FirstName
		{
			get
			{
				return _firstName;
			}
			set
			{
				_firstName = value;
				NotifyOfPropertyChange(() => FirstName);
				NotifyOfPropertyChange(()=>FullName);
			}
		}

		private string _lastName;

		public string LastName
		{
			get { return _lastName; }
			set
			{
				_lastName = value;
				NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => FullName);//when first or last name changes, the full name change

            }
        }
		//propfull tab tba


		public string FullName
		{
			get { return $"{FirstName} {LastName}"; }
		}


	}
}
