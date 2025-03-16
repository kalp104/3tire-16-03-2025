using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]
        public string? ToEmail { get; set; }
    }
}

/*
Email validation string :
[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]

Password validation string: 
[RegularExpression(@"^(?=.[a-z])(?=.[A-Z])(?=.\d)(?=.[@#$^+=!()%-])[A-Za-z\d@#$^+=!()%-]{8,}$", ErrorMessage = "please enter strong password")]

Zipcode :
[RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Invalid Zipcode, should be exactly 6 digits.")]

Phone: 
[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid phone number, should be exactly 10 digits.")]

*/