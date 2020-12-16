using System.ComponentModel;
using System.Configuration.Install;

namespace WinCron
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            using (System.ServiceProcess.ServiceController sc = new
                System.ServiceProcess.ServiceController(serviceInstaller1.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
