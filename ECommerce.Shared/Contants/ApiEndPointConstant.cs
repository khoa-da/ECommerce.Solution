using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        public static class Category
        {
            public const string CategoriesEndpoint = ApiEndpoint + "/categories";
            public const string CategoryEndpoint = CategoriesEndpoint + "/{id}";
            public const string CategoryByParentIdEndpoint = CategoriesEndpoint + "/parent/{parentId}";
        }
        public static class Store
        {
            public const string StoresEndpoint = ApiEndpoint + "/stores";
            public const string StoreEndpoint = StoresEndpoint + "/{id}";
            
        }
        public static class StoreProduct
        {
            public const string StoreProductsEndpoint = ApiEndpoint + "/store-products";
            public const string StoreProductEndpoint = StoreProductsEndpoint + "/{storeId}/{productId}";
          
        }
    }
}
