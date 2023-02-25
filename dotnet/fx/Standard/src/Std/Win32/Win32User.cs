namespace Bearz.Std.Win32;

public static class Win32User
{
    private static bool? s_isUserAdmin;

    public static bool IsAdmin
    {
        get
        {
            if (s_isUserAdmin != null)
                return s_isUserAdmin.Value;

            if (!Env.IsWindows())
            {
                s_isUserAdmin = false;
                return s_isUserAdmin.Value;
            }

            var user = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(user);
            s_isUserAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

            return s_isUserAdmin.Value;
        }
    }
}