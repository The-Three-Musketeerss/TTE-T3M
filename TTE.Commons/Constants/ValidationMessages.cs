﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Commons.Constants
{
    public static class ValidationMessages
    {
        public const string MESSAGE_EMAIL_FAIL = "Invalid Email format";
        public const string MESSAGE_REQUIRED_FIELD = "The following element is required:";

        public const string MESSAGE_INVALID_SECURITY_QUESTION_ID = "Invalid security question ID";
        public const string MESSAGE_ROL_NOT_FOUND = "Role not found";

        public const string CATEGORY_NOT_FOUND = "Category not found";
        public const string CATEGORIES_RETRIEVED_SUCCESSFULLY = "Categories retrieved successfully";
        public const string CATEGORY_DELETED_SUCCESSFULLY = "Category deleted successfully";
        public const string CATEGORY_DELETED_EMPLOYEE_SUCCESSFULLY = "Category deleted successfully, waiting for approval";
    }
}
