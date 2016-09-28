using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace DevExpressGridInconsistencyDemo
{
    public static class DemoDataGenerator
    {
        public const uint StringMaxLength = 50;
        private const int MaxRowsPerInsert = 500;
        // Tuple: ColumnName, Type, Nullable
        private static readonly IEnumerable<Tuple<string, ColumnType, bool>> Columns = new List<Tuple<string, ColumnType, bool>>()
            {
                new Tuple<string, ColumnType, bool>("GroupByColumn", ColumnType.String, false),
                new Tuple<string, ColumnType, bool>("RandomNumberColumn", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("NameColumn", ColumnType.String, false),
                new Tuple<string, ColumnType, bool>("NullableCompanyColumn", ColumnType.String, true),
                new Tuple<string, ColumnType, bool>("FlipACoinColumn", ColumnType.Bool, false),
                new Tuple<string, ColumnType, bool>("NullableRandomNumberColumn", ColumnType.Int, true),
                new Tuple<string, ColumnType, bool>("PlaceholderColumn1", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("PlaceholderColumn2", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("PlaceholderColumn3", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("PlaceholderColumn4", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("PlaceholderColumn5", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("PlaceholderColumn6", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("PlaceholderColumn7", ColumnType.Int, false),
                new Tuple<string, ColumnType, bool>("SampleEnumColumn", ColumnType.SampleEnum, true)
            };

        private const string Null = "NULL";
        private static readonly IEnumerable<string> DistinctGroupByValues = new List<string>()
        { 
            "Ollolai", "Termes", "Montgomery", "Novoli", "Mangalore", "Sala Baganza", "Vandoeuvre-lès-Nancy", "Villers-Perwin", "Westkapelle", "Hoyerswerda", "Cobourg", "Fossato Serralta",
            "Oxford County", "Liverpool", "Columbia", "Móstoles", "Allentown", "Diego de Almagro", "Illapel", "Wrigley", "Shawville", "Hindupur", "North Vancouver", "Auckland", "Duncan",
            "Paradise", "Brusson", "Halen", "Valcourt", "Milton Keynes", "Valley East", "Troon", "Pica", "Port Augusta", "Thurso", "Temuka", "Grand-Hallet", "Schwaz", "Tamines", "Champlain",
            "Emarèse", "Workum", "Huizingen", "Herstappe", "Crieff", "Ostrowiec Świętokrzyski", "Wichita", "Mores", "Ospedaletto Lodigiano", "Calder", "Saint-Hilarion", "Hénin-Beaumont",
            "Ingolstadt", "Lens-Saint-Remy", "Kirkintilloch", "Fumal", "Castelnuovo Magra", "Quenast", "Rio Grande", "Tierra Amarilla", "Roosdaal", "Castiglione di Garfagnana", "Santa Juana",
            "Saintes", "Spalbeek", "Ponte nelle Alpi", "Port Harcourt", "Rengo", "San Isidro", "Ecluse", "Newmarket", "Precenicco", "Ashbourne", "Cambridge", "Kitscoty", "Hospet", "San Calogero",
            "Lissewege", "Fogo", "Karnal", "Arrone", "Bruck an der Mur", "Indianapolis", "Laino Castello", "Asnières-sur-Seine", "Houtain-le-Val", "Cimitile", "Guardia Sanframondi", "Taltal", "Dieppe",
            "Sioux City", "Tarragona", "Ujjain", "Hull", "Rae Lakes", "Monteroni de Arbia", "Landau", "San Clemente", "Waarmaarde", "Terragnolo"
        };
        private const uint MinGroupRowCount = 20;
        private const uint MaxGroupRowCount = 500; 
        private static readonly List<string> RandomNames = new List<string>()
        {
            "Autumn Mclaughlin", "Kylan Floyd", "Nasim Stevens", "Ross Pennington", "Pamela Walton", "Whitney England", "Nadine Decker", "Marshall Ashley", "Noel Carroll", "Audra Dillard",
            "Tucker Robles", "Jennifer Cox", "Meghan Lowery", "Robert Flores", "Aubrey Buckley", "Fritz Ware", "Sawyer Shannon", "Darrel Coleman", "Debra Hampton", "Jacob Potts", "Maite Moran",
            "Harper Robles", "Fitzgerald Phillips", "Francis Frye", "Ramona Landry", "Steel Banks", "Joelle Roberts", "Yolanda Finley", "Sarah Santos", "Violet Crosby", "Alma Collier", "Nadine Church",
            "Yuri Huffman", "Kevyn Osborn", "Echo Andrews", "Petra Santos", "Michael Larson", "Virginia Perry", "Keiko Farley", "Mona Fuentes", "Jana Chang", "Tanner Glover", "Lydia Poole", "Kevyn Dotson",
            "Levi Hansen", "Anthony Baird", "Madison Oneill", "Cameran Moody", "Zenaida Wise", "Cheyenne Warren", "Lewis Moon", "Willow Lara", "Julie Ayers", "Ivy Deleon", "Axel Hudson", "Amela Duffy",
            "Scarlett Roach", "Fatima Beasley", "Quincy Thompson", "Rosalyn Moses", "Althea Crawford", "Molly West", "Janna Malone", "Anthony Travis", "Anjolie Coffey", "Keely Patel", "Wade Daugherty",
            "Carolyn Kaufman", "Eden Anthony", "Garth King", "Jared Mccormick", "Ulric Bridges", "Deirdre White", "Xander Wright", "Laura Smith", "Aspen Mann", "Maggy Gaines", "Wang Baird", "Regina Baird",
            "Melvin Boone", "Elvis Hahn", "Herrod Moran", "Baker Beck", "Aspen Ingram", "Debra Roach", "Nathaniel Kemp", "Taylor Hyde", "Emmanuel Wolf", "Constance Hood", "Warren Poole", "Quinn Harding",
            "Amos Lang", "Jelani Mayo", "Cooper Benson", "Elton Bullock", "Buffy Middleton", "Quin Paul", "Chiquita Warren", "Ella Hobbs", "Kenyon Crawford"
        };
        private static readonly List<string> RandomCompanies = new List<string>()
        {
            "Cursus A Limited", "Nulla LLC", "Vel Arcu Eu LLC", "Sit Amet Consulting", "Conubia Nostra Associates", "Est LLP", "Sit Amet Industries", "Faucibus Ut LLC", "Enim Consulting", "Aliquam Arcu LLC",
            "Duis Mi Enim Limited", "Ac Institute", "Sit Amet Ante Inc.", "Semper Limited", "Rutrum Lorem Industries", "In Tincidunt Congue Associates", "A Ltd", "Nullam Nisl Corp.", "Nec Quam Corporation",
            "Maecenas Ornare Egestas Industries", "Ultrices Duis Ltd", "Metus Corp.", "Pharetra Felis Eget Corporation", "Euismod Ac Fermentum Corporation", "Purus Sapien LLC", "Sed Diam Lorem Corp.",
            "Posuere Limited", "Eget Metus Corporation", "Nibh Corporation", "Ipsum Incorporated", "Tempor Arcu Inc.", "Et Magnis Dis Corp.", "Vestibulum Inc.", "Lacus Etiam Bibendum Foundation",
            "Elit Corporation", "Ornare In Faucibus Incorporated", "Donec Non Institute", "Tempor Erat Neque Company", "Nibh Corporation", "Id Associates", "Mauris Non Corporation", "Consectetuer Rhoncus LLC",
            "Dictum Limited", "Praesent Industries", "Magnis LLC", "Libero LLP", "Ac LLC", "Sollicitudin Commodo Ipsum Foundation", "Sem LLC", "Tincidunt Congue Turpis Company", "Mauris LLP", "Tristique PC",
            "In Faucibus Industries", "Tristique Industries", "Gravida Industries", "A Arcu Ltd", "Mus Aenean Eget Incorporated", "Vitae Risus Duis Associates", "Tempor Est Ac Corporation",
            "Massa Suspendisse Eleifend Limited", "Molestie Associates", "Morbi Incorporated", "Sed Sem Egestas Ltd", "Interdum Nunc Associates", "Urna Convallis Erat Institute", "Dictum Sapien LLP",
            "Tortor Foundation", "Adipiscing Lacus Ut Corporation", "Consequat Institute", "Commodo Hendrerit Corp.", "Diam Dictum Sapien Incorporated", "Quis Urna Nunc Industries", "Aliquet Corporation",
            "Urna Incorporated", "Gravida Foundation", "Dignissim Magna Industries", "Et Pede Nunc LLC", "Enim Nec Tempus Industries", "Varius Nam Corporation", "Suspendisse Industries", "Lorem Fringilla Inc.",
            "Nec Corporation", "Suspendisse Sed LLP", "Cum Sociis Natoque Corporation", "Duis Consulting", "Nam Tempor Foundation", "Aliquam Erat Industries", "Nisl Elementum Corporation",
            "Vitae Semper Egestas LLP", "Nisl Quisque Fringilla LLC", "Euismod Mauris Eu Consulting", "Aenean Massa Foundation", "Adipiscing Ltd", "Non Feugiat Nec Ltd", "Consequat Nec Inc.",
            "Feugiat Placerat Velit Consulting", "Malesuada Integer Associates", "Sit Amet Incorporated", "Tempor Diam PC", "Risus LLC"
        };

        public static void DropAndFillTable(string connectionString, string tableName)
        {
            string statement = PrepareDropAndFillStatement(tableName.ToUpper());
            using (var connection = new SqlConnection(connectionString))
            {
                Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string[] lines = regex.Split(statement);

                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.Transaction = transaction;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            cmd.CommandText = line;
                            cmd.CommandType = CommandType.Text;

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                transaction.Commit();
            }
        }

        private static string PrepareDropAndFillStatement(string tableName)
        {
            return
                $"IF OBJECT_ID('{tableName}', 'U') IS NOT NULL" + Environment.NewLine +
                $"    DROP TABLE [{tableName}]" + Environment.NewLine +
                Environment.NewLine +
                "GO" + Environment.NewLine +
                $"CREATE TABLE [{tableName}] (" + Environment.NewLine +
                BuildColumnsForCreateTable(tableName) + Environment.NewLine +
                ") ON [PRIMARY]" + Environment.NewLine +
                "GO" + Environment.NewLine +
                Environment.NewLine +
                BuildInsertStatements(tableName);         
        }

        private static object BuildInsertStatements(string tableName)
        {
            IEnumerable<string> rows = CreateRandomDataForInsert(tableName);
            List<string> insertStatements = new List<string>();
            for (int i = 0; i < (rows.Count() / MaxRowsPerInsert + 1); i++)
            {
                insertStatements.Add(
                    $"INSERT INTO [{tableName}] ({String.Join(", ", Columns.Select(column => $"[{column.Item1}]"))}) VALUES" + Environment.NewLine +
                    String.Join("," + Environment.NewLine, rows.Skip(i * MaxRowsPerInsert).Take(MaxRowsPerInsert)) + Environment.NewLine +
                    "GO"
                );
            }
            return String.Join(Environment.NewLine + Environment.NewLine, insertStatements);
        }

        private static IEnumerable<string> CreateRandomDataForInsert(string tableName)
        {
            List<string> rows = new List<string>();
            Random rnd = new Random();
            foreach (string distinctGroupByValue in DistinctGroupByValues)
            {
                uint rowsForGroup = (uint) rnd.Next((int)MinGroupRowCount, (int)MaxGroupRowCount);
                for (int i = 0; i < rowsForGroup; i++)
                {
                    rows.Add($"('{distinctGroupByValue}', {CreateRandomRowData(rnd)})");
                }
            }
            rows.Shuffle();
            return rows;
        }

        private static string CreateRandomRowData(Random rnd)
        {
            List<string> values = new List<string>();
            foreach (Tuple<string, ColumnType, bool> column in Columns.Skip(1))
            {
                values.Add(RandomValueForColumn(rnd, column));
            }
            return String.Join(", ", values);
        }

        private static string RandomValueForColumn(Random rnd, Tuple<string, ColumnType, bool> column)
        {
            switch (column.Item2)
            {
                case ColumnType.Int:
                    if (column.Item3)
                        if (rnd.Next(10) == 0)
                            return Null;
                    return rnd.Next().ToString();
                case ColumnType.String:
                    if (column.Item3)
                        if (rnd.Next(10) == 0)
                            return Null;
                    return $"'{(column.Item3 ? RandomCompanies[rnd.Next(RandomCompanies.Count)] : RandomNames[rnd.Next(RandomNames.Count)])}'";
                case ColumnType.Bool:
                    return (rnd.Next()%2).ToString();
                case ColumnType.SampleEnum:
                    if (column.Item3)
                        if (rnd.Next(10) == 0)
                            return Null;
                    return rnd.Next(6).ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string BuildColumnsForCreateTable(string tableName)
        {
            return 
                $"[{IdColumn(tableName)}] [int] IDENTITY(1,1) NOT NULL," + Environment.NewLine + 
                String.Join("," + Environment.NewLine, Columns.Select(column => $"[{column.Item1}] {column.Item2.SQLDataType()} " + (column.Item3 ? "" : "NOT ") + "NULL")) + "," + Environment.NewLine + 
                $"CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED (" + $"[{IdColumn(tableName)}]" + ") WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        }

        private static string IdColumn(string tableName)
        {
            return $"{tableName.ToUpper()}_ID";
        }
    }

    enum ColumnType
    {
        Int,
        String,
        Bool,
        SampleEnum
    }

    static class UtilsExtensions
    {
        public static string SQLDataType(this ColumnType ct)
        {
            switch (ct)
            {
                case ColumnType.Int:
                    return "[int]";
                case ColumnType.String:
                    return $"[nvarchar]({DemoDataGenerator.StringMaxLength})";
                case ColumnType.Bool:
                    return "[bit]";
                case ColumnType.SampleEnum:
                    return "[smallint]";
                default:
                    throw new ArgumentOutOfRangeException(nameof(ct), ct, null);
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}