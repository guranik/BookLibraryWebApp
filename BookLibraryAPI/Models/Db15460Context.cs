namespace BookLibraryAPI.Models
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Data.SqlClient;

    public class Db15460Context : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public Db15460Context(DbContextOptions<Db15460Context> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<IssuedBook> IssuedBooks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string projectRoot = Directory.GetCurrentDirectory();
        //    while (projectRoot.Contains("bin"))
        //    {
        //        projectRoot = Directory.GetParent(projectRoot).FullName;
        //    }

        //    ConfigurationBuilder builder = new();

        //    builder.SetBasePath(projectRoot);

        //    builder.AddJsonFile("appsettings.json");

        //    IConfigurationRoot configuration = builder.AddUserSecrets<Program>().Build();

        //    string connectionString = "";


        //    string secretPass = configuration["RemoteDb:password"];
        //    string secretUser = configuration["RemoteDb:login"];
        //    SqlConnectionStringBuilder sqlConnectionStringBuilder = new(configuration.GetConnectionString("RemoteConnection"))
        //    {
        //        Password = secretPass,
        //        UserID = secretUser
        //    };

        //    connectionString = sqlConnectionStringBuilder.ConnectionString;

        //    _ = optionsBuilder
        //                    .UseSqlServer(connectionString)
        //                    .Options;
        //    optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
        //}
    }
}
