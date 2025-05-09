using Librery_App;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

public class LibraryRepository : ILibraryRepository
{
    private readonly string _connectionString;

    public LibraryRepository()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;
    }


    public void AddBook(string title, List<string> authors)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("insert into Books (Title) output inserted.BookId values (@title)", conn);
            cmd.Parameters.AddWithValue("@title", title);
            int bookId = (int)cmd.ExecuteScalar();

            foreach (var author in authors)
            {
                int authorId;

                var checkCmd = new SqlCommand("select AuthorId from Authors where Name = @name", conn);
                checkCmd.Parameters.AddWithValue("@name", author);
                var result = checkCmd.ExecuteScalar();

                if (result == null)
                {
                    var insertAuthor = new SqlCommand("insert into Authors (Name) output inserted.AuthorId values (@name)", conn);
                    insertAuthor.Parameters.AddWithValue("@name", author);
                    authorId = (int)insertAuthor.ExecuteScalar();
                }
                else
                {
                    authorId = (int)result;
                }

                var baCmd = new SqlCommand("insert into BookAuthors (BookId, AuthorId) values (@bookId, @authorId)", conn);
                baCmd.Parameters.AddWithValue("@bookId", bookId);
                baCmd.Parameters.AddWithValue("@authorId", authorId);
                baCmd.ExecuteNonQuery();
            }
        }
    }

    public int GetRegisteredUsersCount()
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("select count(*) from Visitors", conn);
            return (int)cmd.ExecuteScalar();
        }
    }

    public List<string> GetDebtors()
    {
        var result = new List<string>();
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("select FullName from Visitors where HasDebt = 1", conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
        }
        return result;
    }

    public List<string> GetAuthorsByBook(string title)
    {
        var result = new List<string>();
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand(@"
                select a.Name 
                from Authors a
                join BookAuthors ba on a.AuthorId = ba.AuthorId
                join Books b on b.BookId = ba.BookId
                where b.Title = @title", conn);
            cmd.Parameters.AddWithValue("@title", title);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
        }
        return result;
    }

    public List<string> GetAvailableBooks()
    {
        var result = new List<string>();
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("select Title from Books where Available = 1", conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
        }
        return result;
    }

    public List<string> GetBooksByVisitor(string visitorName)
    {
        var result = new List<string>();
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand(@"
                select b.Title
                from Books b
                join BorrowedBooks bb on bb.BookId = b.BookId
                join Visitors v on v.VisitorId = bb.VisitorId
                where v.FullName = @name", conn);
            cmd.Parameters.AddWithValue("@name", visitorName);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
        }
        return result;
    }

    public void ClearAllDebts()
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("update Visitors set HasDebt = 0 where HasDebt = 1", conn);
            cmd.ExecuteNonQuery();
        }
    }
}
