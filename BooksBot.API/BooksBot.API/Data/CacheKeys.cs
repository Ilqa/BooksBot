namespace GenPsych.Application.CacheKeys
{
    public static class CacheKeys
    {
        public static string BooksListKey => "BooksListKey";

        public static string SelectListKey => "ProductSelectList";

        public static string GetKey(int productId) => $"Product-{productId}";

        public static string GetDetailsKey(int productId) => $"ProductDetails-{productId}";
    }
}