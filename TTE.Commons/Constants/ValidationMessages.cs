namespace TTE.Commons.Constants
{
    public static class ValidationMessages
    {
        public const string MESSAGE_EMAIL_FAIL = "Invalid Email format.";
        public const string MESSAGE_REQUIRED_FIELD = "The following element is required:";

        public const string MESSAGE_INVALID_SECURITY_QUESTION_ID = "Invalid security question ID.";
        public const string MESSAGE_ROLE_NOT_FOUND = "Role not found.";
        public const string MESAGGE_ID_NOT_FOUND = "ID not found.";

        public const string MESSAGE_PRODUCT_NOT_FOUND = "Product not found.";
        public const string MESSAGE_PRODUCT_CREATED_SUCCESSFULLY = "Product created successfully.";
        public const string MESSAGE_PRODUCT_UPDATED_SUCCESSFULLY = "Product updated successfully.";
        public const string MESSAGE_PRODUCT_DELETED_SUCCESSFULLY = "Product deleted successfully.";
        public const string MESSAGE_PRODUCT_DELETED_EMPLOYEE_SUCCESSFULLY = "Product deleted successfully, waiting for approval.";

        public const string CATEGORY_NOT_FOUND = "Category not found.";
        public const string CATEGORY_ALREADY_EXISTS = "Category already exists.";
        public const string CATEGORY_CREATED_SUCCESSFULLY = "Category created successfully.";
        public const string CATEGORY_CREATED_EMPLOYEE_SUCCESSFULLY = "Category created successfully, waiting for approval.";
        public const string CATEGORIES_RETRIEVED_SUCCESSFULLY = "Categories retrieved successfully.";
        public const string CATEGORY_DELETED_SUCCESSFULLY = "Category deleted successfully.";
        public const string CATEGORY_DELETED_EMPLOYEE_SUCCESSFULLY = "Category deleted successfully, waiting for approval.";
        public const string CATEGORY_UPDATED_SUCCESSFULLY = "Category updated successfully.";
        public const string CATEGORY_HAS_PRODUCTS = "Category cannot be deleted because it has associated products.";

        public const string MESSAGE_USER_NOT_FOUND = "User not found.";
        public const string MESSAGE_USER_UPDATED_SUCCESSFULLY = "User {0} has been updated successfully.";
        public const string MESSAGE_EMAIL_ALREADY_EXISTS = "Email already registered.";
        public const string MESSAGE_USERNAME_ALREADY_EXISTS = "USERNAME already registered";
        public const string USER_DELETED_SUCCESSFULLY = "Users deleted successfully.";
        public const string MESSAGE_USERS_RETRIEVED_SUCCESSFULLY = "Users retrieved successfully.";

        public const string MESSAGE_WISHLIST_RETRIEVED_SUCCESSFULLY = "Wishlist retrieved successfully.";
        public const string MESSAGE_PRODUCT_ALREADY_IN_WISHLIST = "Product already in wishlist.";
        public const string MESSAGE_WISHLIST_PRODUCT_ADDED = "Product added to wishlist.";
        public const string MESSAGE_WISHLIST_PRODUCT_REMOVED = "Product removed from wishlist.";
        public const string MESSAGE_WISHLIST_PRODUCT_NOT_FOUND = "Product not found in wishlist.";

        public const string MESSAGE_COUPON_CREATED_SUCCESSFULLY = "Coupon created successfully.";
        public const string MESSAGE_COUPON_UPDATED_SUCCESSFULLY = "Coupon updated successfully.";
        public const string MESSAGE_COUPON_DELETED_SUCCESSFULLY = "Coupon deleted successfully.";
        public const string MESSAGE_COUPON_CODE_ALREADY_EXISTS = "Coupon code already exists.";
        public const string MESSAGE_COUPON_NOT_FOUND = "Coupon not found.";
        public const string MESSAGE_COUPON_APPLIED_SUCCESSFULLY = "Coupon applied successfully.";
        public const string MESSAGE_COUPONS_RETRIEVED_SUCCESSFULLY = "Coupons retrieved successfully.";

        public const string MESSAGE_CART_NOT_FOUND = "Cart not found.";
        public const string MESSAGE_CART_ITEM_NOT_FOUND = "Cart item not found.";
        public const string MESSAGE_CART_ITEM_DELETED = "Cart item deleted successfully.";
        public const string MESSAGE_CART_RETRIEVED_SUCCESSFULLY = "Cart retrieved successfully.";
        public const string MESSAGE_CART_ITEM_ADDED = "Product added/updated in cart.";

        public const string MESSAGE_ORDER_CREATED_SUCCESSFULLY = "Order created successfully.";
        public const string MESSAGE_ORDERS_RETRIEVED_SUCCESSFULLY = "Orders retrieved successfully.";
        public const string MESSAGE_CART_EMPTY = "Cart is empty.";
        public const string MESSAGE_INVENTORY_NOT_FOUND = "Inventory not found for a product in the cart.";
        public const string MESSAGE_INVENTORY_NOT_ENOUGH = "Not enough inventory for product {0}. Available: {1}, Requested: {2}";

        public const string MESSAGE_RATING_NOT_VALID = "Rating must be between 1 and 5.";
        public const string MESSAGE_REVIEW_ADDED_SUCCESSFULLY = "Review added successfully.";

        public const string MESSAGE_JOBS_PENDING = "Jobs pending";
        public const string MESSAGE_JOB_NOT_FOUND = "Job not found.";
        public const string MESSAGE_JOB_ALREADY_REVIEWED = "Job has already been reviewed.";
        public const string MESSAGE_JOB_INVALID_ACTION = "Invalid action. Use 'approve' or 'decline'.";
        public const string MESSAGE_JOB_REVIEW_SUCCESS = "Job {0}d successfully.";
    }
}
