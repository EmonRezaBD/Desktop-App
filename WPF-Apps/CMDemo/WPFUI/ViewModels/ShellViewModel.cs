using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using WPFUI.Models;

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
		//propfull tab tab


		public string FullName
		{
			get { return $"{FirstName} {LastName}"; }
		}

		private BindableCollection<PersonModel> _people = new BindableCollection<PersonModel>();

		public BindableCollection<PersonModel> People
		{
			get { return _people; }
			set { _people = value; }
		}

		private PersonModel _selectedPerson;

		public PersonModel SelectedPerson
		{
			get { return _selectedPerson; }
			set { _selectedPerson = value; }
		}
	}
}
