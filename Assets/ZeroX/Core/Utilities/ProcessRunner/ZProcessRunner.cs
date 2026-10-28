using System.Diagnostics;
using System.Threading.Tasks;

namespace ZeroX
{
    public static class ZProcessRunner
    {
        public static WaitToken<ZProcessResult> RunProcess(ProcessStartInfo processInfo)
        {
            WaitToken<ZProcessResult> waitToken = new WaitToken<ZProcessResult>();

            
            Task task = new Task(() =>
            {
                ZProcessResult processResult = new ZProcessResult();
                Process process = Process.Start(processInfo);
                
                //Output
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    processResult.output = e.Data;
                };
                process.BeginOutputReadLine();

                //Error
                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    processResult.error = e.Data;
                };
                process.BeginErrorReadLine();
                
                //Wait
                process.WaitForExit();
                
                //Finished
                processResult.exitCode = process.ExitCode;
                process.Close();
                
                //Set Result
                waitToken.SetResult(processResult);
            });
            
            
            
            task.Start();
            return waitToken;
        }
        
        
        public static ProcessStartInfo CreateProcessInfo_CMD(string cmd)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + cmd);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            return processInfo;
        }
        
        public static ProcessStartInfo CreateProcessInfo_CMD_WithWindow(string cmd)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/k " + cmd);
            processInfo.WindowStyle = ProcessWindowStyle.Normal;

            return processInfo;
        }
        
        public static ProcessStartInfo CreateProcessInfo(string fileName, string arguments)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo(fileName, arguments);
            return processInfo;
        }
    }
}