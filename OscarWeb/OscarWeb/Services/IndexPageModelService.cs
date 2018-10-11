using OscarWeb.Constants;

namespace OscarWeb.Services
{
    public class IndexPageModelService : PageModelService
    {
        public override string ModuleName { get; }

        public IndexPageModelService()
        {
            ModuleName = ModuleNameConstants.HomePage;
        }
    }
}
