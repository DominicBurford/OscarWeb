using OscarWeb.Constants;

namespace OscarWeb.Services
{
    public class AdministrationPageModelService : PageModelService
    {
        public AdministrationPageModelService()
        {
            ModuleName = ModuleNameConstants.Administration;
        }

        public override string ModuleName { get; }
    }
}
