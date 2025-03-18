using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Commons.Constants
{
    public static class AppConstants
    {        
        public const string EMAIL_PATTERN_VALIDATOR = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string SHOPPER = "Shopper";
        public const string ADMIN = "Admin";
        public const string EMPLOYEE = "Employee";
        public const string ROLE = "Role";

        public const string API_PRODUCTS = "api/products";
        public const string API_AUTH = "api/auth";
        public const string SIGN_UP = "signup";
        public const string LOG_IN = "login";


    }
}
