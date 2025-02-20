using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace WPFUI.ViewModels
{
    public class ShellViewModel:Screen
    {
		private string _firstName="Reza";

		public string FirstName
        {
			get 
			{ 
				return _firstName; 
			}
			set 
			{
				_firstName = value; 
			}
		}

	}
}
