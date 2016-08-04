using System.Data.SqlClient;

namespace DevExpressGridInconsistencyDemo
{
    public class MainWindowViewModel
    {
        private IEntityManager EntityManager { get; set; }
        public IDataGridRepository DataGridRepository { get; set; }

        public MainWindowViewModel()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = Properties.Settings.Default.DBInstance,
                InitialCatalog = Properties.Settings.Default.Database,
                IntegratedSecurity = Properties.Settings.Default.DBWindowsLogin
            };

            if (!Properties.Settings.Default.DBWindowsLogin)
            {
                sqlConnectionStringBuilder.UserID = Properties.Settings.Default.DBUser;
                sqlConnectionStringBuilder.Password = Properties.Settings.Default.DBPassword;
            }

            DemoDataGenerator.DropAndFillTable(sqlConnectionStringBuilder.ConnectionString, "TEST");

            EntityManager = new EntityManager(sqlConnectionStringBuilder.ConnectionString, "TEST");
            DataGridRepository = new DataGridServerModeCore(EntityManager);;
        }
    }
}