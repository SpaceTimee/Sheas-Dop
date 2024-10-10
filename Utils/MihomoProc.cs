using System;
using System.Diagnostics;
using SheasCore;

namespace Sheas_Dop.Utils;

internal class MihomoProc : Proc
{
    internal MihomoProc() : base("Dopping-Mihomo.exe")
    {
    }

    public override void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
    }
    public override void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
    }
    public override void Process_Exited(object sender, EventArgs e)
    {
    }
}