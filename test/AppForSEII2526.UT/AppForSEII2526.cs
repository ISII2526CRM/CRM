namespace AppForSEII2526.UT {
    public class AppForSEII25264SqliteUT{
        protected readonly DbConnection _connection;
        protected readonly ApplicationDbContext _context;
        protected readonly DbContextOptions<ApplicationDbContext> _contextOptions;
         
        protected ApplicationDbContext CreateContext() => new(_contextOptions);
        ////This code is the same one as the above line. 
        //ApplicationDBContext CreateContext() { 
        //    new ApplicationDBContext(_contextOptions); 
        //}

        void Dispose() => _connection.Dispose();
        public AppForSEII25264SqliteUT() {
            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection).Options;

            // Create the schema and seed some data
            _context = new ApplicationDbContext(_contextOptions);

            // C#
            _context = new ApplicationDbContext(_contextOptions);
            _context.Database.EnsureCreated(); // crea todas las tablas en la BD en memoria
        }
    }
}