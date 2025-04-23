namespace Acme.SimpleTaskApp.Authorization
{
    public static class PermissionNames
    {
        public const string Pages_Tenants = "Pages.Tenants";

        public const string Pages_Users = "Pages.Users";
        public const string Pages_Users_Activation = "Pages.Users.Activation";

        public const string Pages_Roles = "Pages.Roles";

        //Thêm quyền CRUD cho products
        public const string Pages_Products = "Pages.Products";
        public const string Pages_Products_Create = "Pages.Products.Create";
        public const string Pages_Products_Edit = "Pages.Products.Edit";
        public const string Pages_Products_Delete = "Pages.Products.Delete";

        //Thêm quyền CRUD cho categories
        public const string Pages_Categories = "Pages.Categories";
        public const string Pages_Categories_Create = "Pages.Categories.Create";
        public const string Pages_Categories_Edit = "Pages.Categories.Edit";
        public const string Pages_Categories_Delete = "Pages.Categories.Delete";

        //Thêm quyền CRUD cho orders
        public const string Pages_Orders = "Pages.Orders";
        public const string Pages_Orders_Create = "Pages.Orders.Create";
        public const string Pages_Orders_Edit = "Pages.Orders.Edit";
        public const string Pages_Orders_Delete = "Pages.Orders.Delete";
        public const string Pages_Orders_View = "Pages.Orders.View";

        //Thêm quyền cho cart
        public const string Pages_Cart = "Pages.Cart";
        public const string Pages_Cart_View = "Pages.Cart.View";
        public const string Pages_Cart_Add = "Pages.Cart.Add";
        public const string Pages_Cart_Update = "Pages.Cart.Update";
        public const string Pages_Cart_Remove = "Pages.Cart.Remove";
        public const string Pages_Cart_Checkout = "Pages.Cart.Checkout";
    }
}
