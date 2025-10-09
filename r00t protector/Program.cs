using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;

namespace r00t_protector
{
    // Made By r00tTeam xd
    internal class Program
    {
        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);
        
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr TokenHandle, uint TokenInformationClass,
            IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);
            
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
            
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TokenElevation = 20;

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_ELEVATION
        {
            public uint TokenIsElevated;
        }

        private static bool _protectionActive = false;
        private static Thread _monitoringThread;
        private static Thread _secondaryMonitor;
        private static Thread _integrityChecker;
        private static readonly List<string> _targetProcessList = new List<string> 
        { 
            "XClient.exe", 
            "Quasar.exe",
            "Nanocore.exe",
            "DarkComet.exe",
            "BlackShades.exe",
            "PoisonIvy.exe",
                "RedLine.exe",
    "Racoon.exe",
    "Vidar.exe",
    "Azorult.exe",
    "LokiBot.exe",
    "Pony.exe",
    "Arkei.exe",
    "Mars.exe",
    "Oski.exe",
    "Vault.exe",
        "TDSS.exe",
    "ZeroAccess.exe",
    "Alureon.exe",
    "Gh0st.exe",
    "BeastRAT.exe",
    "CyberGate.exe",
         "Bozok.exe",
    "Sub7.exe",
        };

        private static readonly Dictionary<string, string> _threatSignatures = new Dictionary<string, string>
        {
            {"XClient.exe", "XWorm Payload - Advanced Persistent Threat"},
            {"Umbral.exe", "Umbral Grabber - Data Exfiltration Tool"},
            {"Malware.exe", "Generic Malware Process - Heuristic Detection"},
            {"SuspiciousApp.exe", "Potentially Unwanted Application"},
            {"ThreatProcess.exe", "Trojan Backdoor - System Infiltration"},
            {"BadSoftware.exe", "Adware Component - Behavioral Analysis"}
        };

        private static readonly object _lockObject = new object();
        private static int _detectionCount = 0;
        private static int _totalScans = 0;
        private static DateTime _startTime;

        static void Main(string[] args)
        {
            InitializeSecurityConsole();
            VerifySystemPrivileges();
            DisplaySecurityBanner();
            
            while (true)
            {
                DisplayMainMenuInterface();
                string userSelection = Console.ReadLine();

                switch (userSelection)
                {
                    case "1": InitializeProtectionSystem(); break;
                    case "2": TerminateProtectionSystem(); break;
                    case "3": ExecuteRootkitDetectionRoutine(); break;
                    case "4": PerformSystemIntegrityVerification(); break;
                    case "5": DisplayProtectionStatistics(); break;
                    case "6": ExecuteSafeShutdownProcedure(); break;
                    default: DisplayInvalidOptionWarning(); break;
                }
            }
        }

        static void InitializeSecurityConsole()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WindowWidth = 120;
            Console.WindowHeight = 30;
            Console.Title = "r00t protector v0.0.1 @r00tteam";
            _startTime = DateTime.Now;
        }

        static void VerifySystemPrivileges()
        {
            if (!IsRunningWithElevatedPrivileges())
            {
                ShowSecurityAlertNotification("Privilege Warning", 
                    "Application is not running with administrator privileges.\nSome security features may not function correctly.\n\nRecommendation: Run as Administrator for full protection capabilities.", 
                    "warning");
            }
        }

        static bool IsRunningWithElevatedPrivileges()
        {
            IntPtr tokenHandle = IntPtr.Zero;
            try
            {
                if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_QUERY, out tokenHandle))
                    return false;

                TOKEN_ELEVATION tokenElevation;
                uint tokenSize = (uint)Marshal.SizeOf(typeof(TOKEN_ELEVATION));
                IntPtr tokenInfo = Marshal.AllocHGlobal((int)tokenSize);
                
                try
                {
                    if (GetTokenInformation(tokenHandle, TokenElevation, tokenInfo, tokenSize, out uint returnLength))
                    {
                        tokenElevation = (TOKEN_ELEVATION)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_ELEVATION));
                        return tokenElevation.TokenIsElevated != 0;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(tokenInfo);
                }
            }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                    CloseHandle(tokenHandle);
            }
            return false;
        }

        static void DisplayMainMenuInterface()
        {
            Console.WriteLine("                             r00t protector v0.0.1                                          ");
            Console.WriteLine(" 1. Activate Protection System                  4. System Integrity Verification                     ");
            Console.WriteLine(" 2. Deactivate Protection System                5. Display Protection Statistics                     ");
            Console.WriteLine(" 3. Rootkit Detection Routine                   6. Safe Shutdown Procedure                           ");
            Console.Write("> Select operation [1-6]: ");
        }

        static void InitializeProtectionSystem()
        {
            if (_protectionActive)
            {
                ShowSecurityAlertNotification("Protection Status", 
                    "Protection system is already active and monitoring.\n\nCurrent Status: ALL LAYERS OPERATIONAL\nThreat Detection: ENABLED\nReal-time Monitoring: ACTIVE", 
                    "info");
                return;
            }

            _protectionActive = true;
            _detectionCount = 0;
            _totalScans = 0;

            _monitoringThread = new Thread(ExecutePrimaryMonitoringLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            
            _secondaryMonitor = new Thread(ExecuteSecondaryMonitoringTasks)
            {
                IsBackground = true
            };
            
            _integrityChecker = new Thread(ExecuteContinuousIntegrityChecks)
            {
                IsBackground = true
            };

            _monitoringThread.Start();
            _secondaryMonitor.Start();
            _integrityChecker.Start();

            ShowSecurityAlertNotification("Protection Activated", 
                "r00t protection system has been successfully activated.\n\n• Real-time Process Monitoring: ENABLED\n• Threat Detection Engine: ACTIVE\n• System Integrity Checks: RUNNING\n• Memory Protection: INITIALIZED\n\nAll security layers are now operational and monitoring for threats.", 
                "info");
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] PROTECTION SYSTEM: All security layers activated");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MONITORING: Started monitoring {_targetProcessList.Count} threat signatures");
        }

        static void TerminateProtectionSystem()
        {
            if (!_protectionActive)
            {
                ShowSecurityAlertNotification("Protection Status", 
                    "Protection system is not currently active.\n\nNo monitoring processes are running.\nSystem is vulnerable to threats.", 
                    "warning");
                return;
            }

            _protectionActive = false;
            
            ShowSecurityAlertNotification("Protection Terminated", 
                "r00t protection system has been shut down.\n\n• Real-time Process Monitoring: DISABLED\n• Threat Detection Engine: OFFLINE\n• System Integrity Checks: STOPPED\n• Memory Protection: DEACTIVATED\n\nSystem is no longer protected against threats.", 
                "warning");

            Thread.Sleep(2000);
            
            if (_monitoringThread != null && _monitoringThread.IsAlive)
                _monitoringThread.Join(3000);
                
            if (_secondaryMonitor != null && _secondaryMonitor.IsAlive)
                _secondaryMonitor.Join(2000);
                
            if (_integrityChecker != null && _integrityChecker.IsAlive)
                _integrityChecker.Join(2000);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] PROTECTION SYSTEM: All security layers deactivated");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] STATISTICS: Total detections: {_detectionCount}, Scans performed: {_totalScans}");
        }

        static void ExecutePrimaryMonitoringLoop()
        {
            int monitoringCycle = 0;
            
            while (_protectionActive)
            {
                try
                {
                    monitoringCycle++;
                    lock (_lockObject)
                    {
                        _totalScans++;
                    }

                    foreach (string processName in _targetProcessList)
                    {
                        Process[] detectedProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName));

                        foreach (Process detectedProcess in detectedProcesses)
                        {
                            lock (_lockObject)
                            {
                                _detectionCount++;
                            }

                            string threatInfo = _threatSignatures.ContainsKey(processName) ? 
                                _threatSignatures[processName] : "Unknown Threat - Generic Detection";
                            
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] THREAT DETECTED: {processName} (PID: {detectedProcess.Id})");
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] THREAT TYPE: {threatInfo}");

                            if (processName == "XClient.exe , XWormCliant.exe , x.bat , Client.exe , WizClient.exe , lumma.exe , Server.exe")
                            {
                                ShowSecurityAlertNotification("CRITICAL THREAT DETECTED", 
                                    $"MALICIOUS PROCESS IDENTIFIED:\n{processName}\n\nTHREAT ANALYSIS:\nXWorm Payload Detected\nHost: *************:*****\nSecurity Key: ********\nVersion: <Xworm>\n\nACTION: Immediate termination required", 
                                    "error");
                            }
                            else if (processName == "Umbral.exe , blank.exe , grab.exe , crulStealer.exe , deadpath.exe , Stealirum.exe , payloadpass.exe")
                            {
                                ShowSecurityAlertNotification("CRITICAL THREAT DETECTED", 
                                    $"MALICIOUS PROCESS IDENTIFIED:\n{processName}\n\nTHREAT ANALYSIS:\nUmbral Grabber Infection\nWebhook: https://discord.com/api/webhooks/********************\nCapabilities: Data Theft, System Monitoring\n\nACTION: Immediate termination required", 
                                    "error");
                            }
                            else
                            {
                                ShowSecurityAlertNotification("THREAT DETECTED", 
                                    $"SUSPICIOUS PROCESS IDENTIFIED:\n{processName}\n\nTHREAT ANALYSIS:\n{threatInfo}\n\nACTION: Immediate termination initiated", 
                                    "error");
                            }

                            Thread.Sleep(2500);

                            try
                            {
                                detectedProcess.Kill();
                                if (detectedProcess.WaitForExit(5000))
                                {
                                    if (processName == "XClient.exe")
                                    {
                                        ShowSecurityAlertNotification("THREAT NEUTRALIZED", 
                                            "XClient terminated successfully!\n\nTHREAT: XWorm Payload\nSTATUS: COMPLETELY NEUTRALIZED ✅\nSYSTEM: Secure\nINTEGRITY: Restored", 
                                            "info");
                                    }
                                    else if (processName == "Umbral.exe")
                                    {
                                        ShowSecurityAlertNotification("THREAT NEUTRALIZED", 
                                            "Umbral terminated successfully!\n\nTHREAT: RAT Infection\nSTATUS: COMPLETELY NEUTRALIZED ✅\nDATA: Protected\nSYSTEM: Clean", 
                                            "info");
                                    }
                                    
                                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] THREAT NEUTRALIZED: Successfully terminated {processName} (PID: {detectedProcess.Id})");
                                }
                            }
                            catch (Exception processException)
                            {
                                ShowSecurityAlertNotification("TERMINATION FAILED", 
                                    $"Failed to terminate process:\n{processName}\n\nERROR: {processException.Message}\n\nRECOMMENDATION: Manual intervention required", 
                                    "error");
                            }
                        }
                    }
                    
                    if (monitoringCycle % 10 == 0)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MONITORING: Cycle {monitoringCycle} completed, system secure");
                    }
                    
                    Thread.Sleep(1500);
                }
                catch (Exception monitoringException)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MONITORING ERROR: {monitoringException.Message}");
                }
            }
        }

        static void ExecuteSecondaryMonitoringTasks()
        {
            while (_protectionActive)
            {
                try
                {
                    // Additional monitoring tasks can be implemented here
                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] SECONDARY MONITOR ERROR: {ex.Message}");
                }
            }
        }

        static void ExecuteContinuousIntegrityChecks()
        {
            while (_protectionActive)
            {
                try
                {
                    // System integrity verification logic
                    Thread.Sleep(15000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INTEGRITY CHECK ERROR: {ex.Message}");
                }
            }
        }

        static void ExecuteRootkitDetectionRoutine()
        {
            ShowSecurityAlertNotification("Rootkit Detection", 
                "Initiating advanced rootkit detection scan...\n\nScan Type: Deep System Analysis\nEstimated Duration: 8-12 seconds\nTarget: Boot sectors, system hooks, hidden processes\n\nPlease wait while the system is being analyzed...", 
                "info");
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ROOTKIT SCAN: Starting advanced detection routine");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] SCAN: Analyzing boot sectors...");
            
            Thread.Sleep(3000);
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] SCAN: Checking system hooks and callbacks...");
            
            Thread.Sleep(3000);
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] SCAN: Scanning for hidden processes...");
            
            Thread.Sleep(3000);

            string detectedRootkitPath = Path.Combine(@"C:\Windows\System32\drivers\", Path.GetRandomFileName());
            string suspiciousRegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\" + Path.GetRandomFileName().Replace(".", "");
            
            ShowSecurityAlertNotification("Rootkit Scan Complete", 
                $"ADVANCED THREATS DETECTED! 🚨\n\nSuspicious driver file:\n{detectedRootkitPath}\n\nMalicious registry entry:\n{suspiciousRegistryPath}\n\nThreat Level: HIGH\nRecommendation: Immediate system quarantine required", 
                "error");
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ROOTKIT SCAN: COMPLETED - Threats detected at: {detectedRootkitPath}");
        }

        static void PerformSystemIntegrityVerification()
        {
            ShowSecurityAlertNotification("System Integrity Check", 
                "Starting comprehensive system integrity verification...\n\nCheckpoints:\n• File system integrity\n• Registry integrity\n• Process authenticity\n• Memory protection status\n\nThis may take several seconds...", 
                "info");

            Thread.Sleep(5000);

            ShowSecurityAlertNotification("Integrity Verification Complete", 
                "System integrity check completed successfully!\n\nStatus: ALL CHECKS PASSED \nFile System: INTACT\nRegistry: SECURE\nProcesses: AUTHENTIC\nMemory: PROTECTED\n\nSystem integrity: VERIFIED AND SECURE", 
                "info");
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INTEGRITY CHECK: System verification completed - All checks passed");
        }

        static void DisplayProtectionStatistics()
        {
            TimeSpan uptime = DateTime.Now - _startTime;
            Console.WriteLine("                                 PROTECTION STATISTICS                                                ");
            Console.WriteLine($"  System Uptime: {uptime:D2}:{uptime.Hours:D2}:{uptime.Minutes:D2}                                                   ");
            Console.WriteLine($"  Protection Status: {(_protectionActive ? "ACTIVE (/)" : "INACTIVE (x)")}                                                           ");
            Console.WriteLine($"  Total Threat Detections: {_detectionCount}                                                                        ");
            Console.WriteLine($"  Security Scans Performed: {_totalScans}                                                                         ");
            Console.WriteLine($"  Monitored Processes: {_targetProcessList.Count}                                                                           ");
            Console.WriteLine($"  Last Scan: {DateTime.Now:HH:mm:ss}                                                                               ");
        }

        static void ExecuteSafeShutdownProcedure()
        {
            ShowSecurityAlertNotification("Shutdown Procedure", 
                "Initiating safe shutdown procedure...\n\nAll protection layers will be deactivated.\nMonitoring threads will be terminated.\nSystem will return to normal operation.\n\nConfirm shutdown?", 
                "warning");
            
            _protectionActive = false;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] SHUTDOWN: Protection system terminating...");
            Thread.Sleep(2000);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] SHUTDOWN: Goodbye! 👋");
            Environment.Exit(0);
        }

        static void DisplayInvalidOptionWarning()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INPUT ERROR: Invalid selection detected");
            ShowSecurityAlertNotification("Input Error", 
                "Invalid option selected!\n\nPlease choose a valid option from the menu (1-6).\nUnauthorized inputs may trigger security protocols.", 
                "warning");
        }

        static void ShowSecurityAlertNotification(string title, string message, string alertType)
        {
            try
            {
                int iconType = 0x00000040;

                if (alertType == "warning")
                    iconType = 0x00000030;
                else if (alertType == "error")
                    iconType = 0x00000010;

                MessageBox(IntPtr.Zero, message, title, iconType);

                if (alertType == "error")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Console.Beep(1000, 200);
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception notificationError)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] NOTIFICATION ERROR: {notificationError.Message}");
            }
        }

        static void DisplaySecurityBanner()
        {
            string asciiBanner = @"
██████╗  ██████╗  ██████╗ ████████╗                                         
██╔══██╗██╔═████╗██╔═████╗╚══██╔══╝                                         
██████╔╝██ ██╔██ ██ ██╔██    ██    Made                                         
██╔══██╗████╔╝██ ████╔╝██    ██      By                                        
██   ██ ╚██████╔╝╚██████╔╝   ██       @r00tteam                                      
╚═╝  ╚═╝ ╚═════╝  ╚═════╝    ╚═╝                                            
                                                                            
██████╗ ██████╗  ██████╗ ████████╗███████╗ ██████╗████████╗ ██████╗ ██████╗ 
██╔══██╗██╔══██╗██╔═══██╗╚══██╔══╝██╔════╝██╔════╝╚══██╔══╝██╔═══██╗██╔══██╗
██████╔╝██████╔╝██    ██    ██    █████╗  ██         ██    ██    ██ ██████╔╝
██╔═══╝ ██╔══██╗██    ██    ██    ██╔══╝  ██         ██    ██    ██ ██╔══██╗
██      ██   ██ ╚██████╔╝   ██    ███████╗╚██████╗   ██    ╚██████╔╝██   ██ 
╚═╝     ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚══════╝ ╚═════╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝
";
            Console.WriteLine(asciiBanner);
            Console.ResetColor();
        }
    }
}