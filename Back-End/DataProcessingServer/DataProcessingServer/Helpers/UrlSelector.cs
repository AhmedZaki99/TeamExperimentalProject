namespace DataProcessingServer
{
    public static class UrlSelector
    {

        #region Constants

        public const string PagesPath = "~/pages";
        
        public const string IndexPage = "~/index.html";

        public const string ErrorPage = $"{PagesPath}/error.html";
        public const string AboutPage = $"{PagesPath}/about.html";

        public const string AccountPage = $"{PagesPath}/account.html";
        public const string RegisterPage = $"{PagesPath}/register.html";
        public const string LoginPage = $"{PagesPath}/login.html";
        public const string LockoutPage = $"{PagesPath}/lockout.html";

        #endregion

    }
}
