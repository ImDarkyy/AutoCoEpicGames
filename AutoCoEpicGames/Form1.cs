using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace AutoCoEpicGames



{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool MyShowWindow(IntPtr hWnd, int nCmdShow);

        public string configFilePath = "./config.ini";
        public Form1()
        {
            InitializeComponent();
            // Bloquer le bouton de mise en plein écran
            this.MaximizeBox = false;

            // Désactiver le redimensionnement de la fenêtre
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            ConfigManager configManager = new ConfigManager(configFilePath);

            // Lire les comptes à partir du fichier de configuration
            List<string> accounts = ReadAllSections("Nom");

            Dictionary<Button, int> buttonIndices = new Dictionary<Button, int>();
            int buttonMargin = 10; // Marge entre les boutons
            int x = 50;
            int y = 50;

            for (int i = 0; i < accounts.Count; i++)
            {
                string account = accounts[i];

                Button button = new Button();
                button.Text = account;
                button.Tag = i; // Stocker l'index du compte en utilisant la propriété Tag du bouton

                button.Click += (btnSender, btnE) =>
                {
                    int accountIndex = (int)((Button)btnSender).Tag; // Récupérer l'index du compte à partir de la propriété Tag
                    ConnexionCompte(accountIndex);
                };

                button.Location = new Point(x, y);
                this.Controls.Add(button);

                x += button.Width + buttonMargin;
            }

            this.ClientSize = new Size(x, y + this.Controls[accounts.Count - 1].Height + buttonMargin);

        }

        public List<string> ReadAllSections(string key)
        {
            List<string> values = new List<string>();

            try
            {
                if (File.Exists(configFilePath))
                {
                    using (StreamReader reader = new StreamReader(configFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                int equalsIndex = line.IndexOf('=');
                                if (equalsIndex >= 0)
                                {
                                    string lineKey = line.Substring(0, equalsIndex).Trim();
                                    if (lineKey == key)
                                    {
                                        string value = line.Substring(equalsIndex + 1).Trim();
                                        values.Add(value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la lecture du fichier de configuration : {ex.Message}");
            }

            return values;
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            ConnexionCompte(0);
        }

        [DllImport("user32.dll")]
        private static extern bool MyShowWindowCmdShow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWMAXIMIZED = 3;

        public class ProcessManager
        {
            public static void StartOrAttachProcess(string processName, string pathToExecutable)
            {
                // Vérifier si le processus est déjà ouvert
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    // Le processus est déjà ouvert, on attache à la première instance
                    Process process = processes[0];
                    Console.WriteLine($"Le processus {processName} est déjà ouvert (ID : {process.Id}).");
                    Console.WriteLine("Attacher au processus existant...");
                }
                else
                {
                    // Le processus n'est pas ouvert, on le démarre
                    try
                    {
                        Process.Start(pathToExecutable);
                        Console.WriteLine($"Le processus {processName} a été démarré.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Impossible de démarrer le processus {processName}: {ex.Message}");
                    }
                }
            }
        }
        public class ConfigManager
        {
            private string configFilePath;

            public ConfigManager(string filePath)
            {
                configFilePath = filePath;
            }

            public string ReadValue(string section, string key)
            {
                try
                {
                    if (File.Exists(configFilePath))
                    {
                        IniFile iniFile = new IniFile(configFilePath);
                        return iniFile.Read(section, key);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la lecture du fichier de configuration : {ex.Message}");
                }

                return null;
            }
        }


        public class IniFile
        {
            private string filePath;

            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string filePath);

            [DllImport("user32.dll")]
            private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            private const int SW_SHOWMAXIMIZED = 3;


            public IniFile(string filePath)
            {
                this.filePath = filePath;
            }

            public void Write(string section, string key, string value)
            {
                WritePrivateProfileString(section, key, value, filePath);
            }

            public string Read(string section, string key)
            {
                StringBuilder value = new StringBuilder(255);
                GetPrivateProfileString(section, key, "", value, 255, filePath);
                return value.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConnexionCompte(1);
        }

        private void ConnexionCompte(int darkyy)
        {
 
            // Lecture des valeurs du fichier de configuration
            string configFilePath = "./config.ini";
            string username = string.Empty;
            string password = string.Empty;
            ConfigManager configManager = new ConfigManager(configFilePath);
            if (darkyy == 0)
            {
                username = configManager.ReadValue("Credentials", "Username");
                
                password = configManager.ReadValue("Credentials", "Password");
            }
            if (darkyy == 1)
            {
               username = configManager.ReadValue("Credentials2", "Username");

               password = configManager.ReadValue("Credentials2", "Password");
            }
            if (darkyy == 2)
            {
                username = configManager.ReadValue("Credentials3", "Username");

                password = configManager.ReadValue("Credentials3", "Password");
            }
            if (darkyy == 3)
            {
                username = configManager.ReadValue("Credentials4", "Username");

                password = configManager.ReadValue("Credentials4", "Password");
            }
            if (darkyy == 4)
            {
                username = configManager.ReadValue("Credentials5", "Username");

                password = configManager.ReadValue("Credentials5", "Password");
            }
            if (darkyy == 5)
            {
                username = configManager.ReadValue("Credentials6", "Username");

                password = configManager.ReadValue("Credentials6", "Password");
            }
            if (darkyy == 6)
            {
                username = configManager.ReadValue("Credentials7", "Username");

                password = configManager.ReadValue("Credentials7", "Password");
            }

            // Utilisation des valeurs pour effectuer une action
            // Par exemple, vous pouvez les passer à la fonction ExecuteKeyboardShortcutsOnProcess
            string processName = "EpicGamesLauncher";
            string pathToExecutable = @"C:\Program Files (x86)\Epic Games\Launcher\Portal\Binaries\Win32\EpicGamesLauncher.exe";
            

            // Vérifier si le processus est déjà ouvert
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                // Le processus est déjà ouvert, on attache à la première instance
                Process process = processes[0];
                Console.WriteLine($"Le processus {processName} est déjà ouvert (ID : {process.Id}).");
                Console.WriteLine("Attacher au processus existant...");

                IntPtr handle = process.MainWindowHandle;
                ShowWindow(handle, SW_SHOWMAXIMIZED);
                SetForegroundWindow(handle);
            }
            else
            {
                // Le processus n'est pas ouvert, on le démarre
                try
                {
                    Process.Start(pathToExecutable);
                    Console.WriteLine($"Le processus {processName} a été démarré.");
                    Thread.Sleep(5000); // Attendre que la fenêtre du processus soit prête
                    IntPtr handle = Process.GetProcessesByName(processName)[0].MainWindowHandle;
                    MyShowWindow(handle, SW_SHOWMAXIMIZED);
                    SetForegroundWindow(handle);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Impossible de démarrer le processus {processName}: {ex.Message}");
                }
            }

            // Saisie de la chaîne de caractères pour le champ de login
            Thread.Sleep(5000);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(2000);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(username);

            // Saisie de la chaîne de caractères pour le champ de mot de passe
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(password);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{ENTER}");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}





