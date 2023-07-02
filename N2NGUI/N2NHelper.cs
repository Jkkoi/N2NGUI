using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;

namespace N2NGUI;

public class N2NHelper
{
    private Process? _n2nProcess;
    public event EventHandler<string> LogEvent;
    private static N2NHelper instance;
    private static readonly object lockObject = new object();

    private N2NHelper()
    {
    }

    public static N2NHelper Instance
    {
        get
        {
            // 使用双重检查锁定确保线程安全
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new N2NHelper();
                    }
                }
            }

            return instance;
        }
    }

    public void RunN2N()
    {
        _n2nProcess ??= new Process();

        _n2nProcess.StartInfo.FileName = "edge.exe";
        _n2nProcess.StartInfo.Arguments = Config.GenerateN2NArguments();
        _n2nProcess.StartInfo.UseShellExecute = false; //设置false只能启动一个程序
        _n2nProcess.StartInfo.RedirectStandardInput = false;
        _n2nProcess.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
        _n2nProcess.StartInfo.RedirectStandardError = true; //重定向标准错误输出
        _n2nProcess.StartInfo.CreateNoWindow = true;

        _n2nProcess.Start();
        HandleLog();
    }

    public void ShutDownN2N()
    {
        if (!AttachConsole((uint)_n2nProcess.Id)) return;
        // 我们自己的进程需要忽略掉 Ctrl+C 信号，否则自己也会退出。
        SetConsoleCtrlHandler(null, true);

        // 将 Ctrl+C 信号发送到前面已关联（附加）的控制台进程中。
        GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);

        // 拾前面已经附加的控制台。
        FreeConsole();

        _n2nProcess.WaitForExit(10000);

        // 重新恢复我们自己的进程对 Ctrl+C 信号的响应。
        SetConsoleCtrlHandler(null, false);
    }

    private async Task HandleLog()
    {
        while (_n2nProcess.HasExited == false || _n2nProcess.StandardOutput.EndOfStream == false)
        {
            OnLogEvent(await _n2nProcess.StandardOutput.ReadLineAsync() ?? string.Empty);
        }
    }

    protected virtual void OnLogEvent(string logString)
    {
        LogEvent?.Invoke(this, logString);
    }

    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate? HandlerRoutine, bool Add);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

    private delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

    enum CtrlTypes : uint
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }
}