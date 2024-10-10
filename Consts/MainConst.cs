using System.Text.RegularExpressions;

namespace Sheas_Dop.Consts;

internal partial class MainConst
{
    internal static string HostsConfStartMarker => "# Pixiv Nginx Start\n";
    internal static string HostsConfEndMarker => "# Pixiv Nginx End";
    internal static string NginxRootCertSubjectName => "CN=Pixiv Nginx Cert Root";
    internal static string NginxChildCertSubjectName => "CN=Pixiv Nginx Cert Child";

    [GeneratedRegex(@"^(https?:\/\/)?[a-zA-Z0-9](-*[a-zA-Z0-9])*(\.[a-zA-Z0-9](-*[a-zA-Z0-9])*)+(:\d{1,5})?(\/[a-zA-Z0-9.\-_\~\!\$\&\'\(\)\*\+\,\;\=\:\@\%]*)*$")]
    internal static partial Regex SingleUrlRegex();
}