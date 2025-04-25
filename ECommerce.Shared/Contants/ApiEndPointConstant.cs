namespace ECommerce.Shared.Contants
{
    public static class ApiEndPointConstant
    {
        static ApiEndPointConstant()
        {
        }

        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndpoint + "/auth";
            public const string Login = AuthenticationEndpoint + "/login";
            public const string RefreshToken = AuthenticationEndpoint + "/refresh-token";

        }
        public static class User
        {
            public const string UsersEndpoint = ApiEndpoint + "/users";
            public const string UserEndpoint = UsersEndpoint + "/{id}";
            public const string UserByFieldEndpoint = UsersEndpoint + "/{field}/{value}";
            public const string OrderByUserIdEndpoint = UserEndpoint + "/order";
        }
        public static class ProductImage
        {
            public const string ProductImagesEndpoint = ApiEndpoint + "/product-images";
            public const string ProductImageEndpoint = ProductImagesEndpoint + "/{id}";
            public const string ProductImagesByProductIdEndpoint = ProductImagesEndpoint + "/product/{productId}";
            public const string SetMainImageEndpoint = ProductImageEndpoint + "/set-main";
            public const string SetDisplayOrderEndpoint = ProductImageEndpoint + "/set-display-order";
        }
        public static class Product
        {
            public const string ProductsEndpoint = ApiEndpoint + "/products";
            public const string ProductEndpoint = ProductsEndpoint + "/{id}";
            public const string ProductByBrandIdEndpoint = ProductsEndpoint + "/brand/{brand}";
            public const string ImagesInProductEndpoint = ProductEndpoint + "/images";
            public const string AddToCartEndpoint = ProductsEndpoint + "/add-to-cart";
            public const string ProductByStoreIdEndpoint = ProductEndpoint + "/store/{storeId}";
        }
        public static class Category
        {
            public const string CategoriesEndpoint = ApiEndpoint + "/categories";
            public const string CategoryEndpoint = CategoriesEndpoint + "/{id}";
            public const string CategoryByParentIdEndpoint = CategoriesEndpoint + "/parent/{parentId}";
            public const string ProductsInCategoryEndpoint = CategoryEndpoint + "/products";
            public const string ProductsInParentCategoryEndpoint = CategoriesEndpoint + "/parent/{parentId}/products";
        }
        public static class Store
        {
            public const string StoresEndpoint = ApiEndpoint + "/stores";
            public const string StoreEndpoint = StoresEndpoint + "/{id}";
            public const string ProductsInStoreEndpoint = StoreEndpoint + "/products";

        }
        public static class StoreProduct
        {
            public const string StoreProductsEndpoint = ApiEndpoint + "/store-products";
            public const string StoreProductEndpoint = StoreProductsEndpoint + "/{storeId}/{productId}";

        }
        public static class OrderItem
        {
            public const string OrderItemsEndpoint = ApiEndpoint + "/order-items";
            public const string OrderItemEndpoint = OrderItemsEndpoint + "/{id}";
        }

        public static class Cart
        {
            public const string CartsEndpoint = ApiEndpoint + "/carts";
            public const string CartItemEndpoint = CartsEndpoint + "/items";
            public const string MergeCartEndpoint = CartsEndpoint + "/merge";
            public const string CartItemEndpointByProductId = CartItemEndpoint + "/{productId}";
        }
        public static class Order
        {
            public const string OrdersEndpoint = ApiEndpoint + "/orders";
            public const string OrderEndpoint = OrdersEndpoint + "/{id}";
            public const string CancelOrderEndpoint = OrdersEndpoint + "/cancel";
            //public const string OrderByUserIdEndpoint = OrdersEndpoint + "/user/{userId}";
            //public const string OrderByStoreIdEndpoint = OrdersEndpoint + "/store/{storeId}";
            //public const string OrderByStatusEndpoint = OrdersEndpoint + "/status/{status}";
        }
    }
}
