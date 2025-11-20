using Hangfire.Dashboard;

namespace SearchEngine.API.Filters;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        //dockerda 401 almamk için buraya TODO: admin kontrol eklebilir
        return true;
    }
}

