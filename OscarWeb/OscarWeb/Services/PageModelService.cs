namespace OscarWeb.Services
{
    /// <summary>
    /// Base class for all Page Model service classes. These provide behaviour to the application's Razor Pages
    /// by binding a service class to the Razor Page's PageModel class.
    /// </summary>
    public abstract class PageModelService
    {
        /// <summary>
        /// The module name should match MainMenu.DisplayText as it is used by the 
        /// function MenuServices().GetParentIdForMenuItem().
        ///  </summary>
        public abstract string ModuleName { get; }
        public LoggingService LoggingService = new LoggingService();
    }
}
