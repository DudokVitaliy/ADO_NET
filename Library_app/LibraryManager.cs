using System;
using System.Configuration;
using System.Data.SqlClient;

public class LibraryManager
{
    private readonly string connectionString;

    public LibraryManager()
    {
        connectionString = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;
    }

    public void AddBook(string title, string[] authorNames)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
                var cmd = new SqlCommand("INSERT INTO Books (Title) OUTPUT INSERTED.Id VALUES (@Title)", conn, transaction);
                cmd.Parameters.AddWithValue("@Title", title);
                int bookId = (int)cmd.ExecuteScalar();

                foreach (var author in authorNames)
                {
                    int authorId = GetOrCreateAuthor(conn, transaction, author);
                    var linkCmd = new SqlCommand("INSERT INTO BooksAuthors (BookId, AuthorId) VALUES (@BookId, @AuthorId)", conn, transaction);
                    linkCmd.Parameters.AddWithValue("@BookId", bookId);
                    linkCmd.Parameters.AddWithValue("@AuthorId", authorId);
                    linkCmd.ExecuteNonQuery();
                }

                transaction.Commit();
                Console.WriteLine("Книгу додано.");
            }
            catch
            {
                transaction.Rollback();
                Console.WriteLine("Помилка при додаванні книги.");
            }
        }
    }

    private int GetOrCreateAuthor(SqlConnection conn, SqlTransaction transaction, string name)
    {
        var checkCmd = new SqlCommand("SELECT Id FROM Authors WHERE FullName = @Name", conn, transaction);
        checkCmd.Parameters.AddWithValue("@Name", name);
        var result = checkCmd.ExecuteScalar();

        if (result != null)
            return (int)result;

        var insertCmd = new SqlCommand("INSERT INTO Authors (FullName) OUTPUT INSERTED.Id VALUES (@Name)", conn, transaction);
        insertCmd.Parameters.AddWithValue("@Name", name);
        return (int)insertCmd.ExecuteScalar();
    }

    public void CountVisitors()
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Visitors", conn);
            int count = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Кількість відвідувачів: {count}");
        }
    }

    public void ListDebtors()
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT FullName FROM Visitors WHERE IsDebtor = 1", conn);
            var reader = cmd.ExecuteReader();
            Console.WriteLine("Боржники:");
            while (reader.Read())
                Console.WriteLine(reader["FullName"]);
        }
    }

    public void ListAuthorsByBook(string title)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand(@"
                SELECT A.FullName 
                FROM Authors A 
                JOIN BooksAuthors BA ON A.Id = BA.AuthorId 
                JOIN Books B ON B.Id = BA.BookId
                WHERE B.Title = @Title", conn);
            cmd.Parameters.AddWithValue("@Title", title);
            var reader = cmd.ExecuteReader();
            Console.WriteLine($"Автори книги '{title}':");
            while (reader.Read())
                Console.WriteLine(reader["FullName"]);
        }
    }

    public void ListAvailableBooks()
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT Title FROM Books WHERE IsAvailable = 1", conn);
            var reader = cmd.ExecuteReader();
            Console.WriteLine("Доступні книги:");
            while (reader.Read())
                Console.WriteLine(reader["Title"]);
        }
    }

    public void ListBooksOfUser(string visitorName)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand(@"
                SELECT B.Title 
                FROM Books B 
                JOIN IssuedBooks I ON B.Id = I.BookId 
                JOIN Visitors V ON I.VisitorId = V.Id 
                WHERE V.FullName = @Name", conn);
            cmd.Parameters.AddWithValue("@Name", visitorName);
            var reader = cmd.ExecuteReader();
            Console.WriteLine($"Книги у {visitorName}:");
            while (reader.Read())
                Console.WriteLine(reader["Title"]);
        }
    }

    public void ClearAllDebts()
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("UPDATE Visitors SET IsDebtor = 0 WHERE IsDebtor = 1", conn);
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine($"Знято борги з {rows} відвідувачів.");
        }
    }
}
