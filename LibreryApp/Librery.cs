using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace LibraryApp
{
    public class LibraryRepository : ILibrary
    {
        private readonly string _connectionString;

        public LibraryRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Library"].ConnectionString;
        }

        public void AddBook(string title, List<string> authors)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdBook = new SqlCommand(
                        "INSERT INTO Books (Title, IsAvailable) OUTPUT INSERTED.Id VALUES (@title, 1)",
                        conn, transaction);
                    cmdBook.Parameters.AddWithValue("@title", title);
                    int bookId = (int)cmdBook.ExecuteScalar();

                    foreach (var author in authors)
                    {
                        int authorId;
                        SqlCommand cmdFind = new SqlCommand("SELECT Id FROM Authors WHERE Name = @name", conn, transaction);
                        cmdFind.Parameters.AddWithValue("@name", author);
                        var result = cmdFind.ExecuteScalar();

                        if (result != null)
                            authorId = (int)result;
                        else
                        {
                            SqlCommand cmdInsertAuthor = new SqlCommand("INSERT INTO Authors (Name) OUTPUT INSERTED.Id VALUES (@name)", conn, transaction);
                            cmdInsertAuthor.Parameters.AddWithValue("@name", author);
                            authorId = (int)cmdInsertAuthor.ExecuteScalar();
                        }

                        SqlCommand cmdBA = new SqlCommand("INSERT INTO BooksAuthors (BookId, AuthorId) VALUES (@bookId, @authId)", conn, transaction);
                        cmdBA.Parameters.AddWithValue("@bookId", bookId);
                        cmdBA.Parameters.AddWithValue("@authId", authorId);
                        cmdBA.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public int GetVisitorCount()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Visitors", conn);
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<string> GetDebtors()
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Name FROM Visitors WHERE HasDebt = 1", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result;
        }

        public List<string> GetAuthorsByBook(string bookTitle)
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                SELECT a.Name
                FROM Authors a
                JOIN BooksAuthors ba ON a.Id = ba.AuthorId
                JOIN Books b ON b.Id = ba.BookId
                WHERE b.Title = @title", conn);
                cmd.Parameters.AddWithValue("@title", bookTitle);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result;
        }

        public List<string> GetAvailableBooks()
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Title FROM Books WHERE IsAvailable = 1", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result;
        }

        public List<string> GetBooksByVisitor(string visitorName)
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                SELECT b.Title
                FROM Books b
                JOIN Loans l ON l.BookId = b.Id
                JOIN Visitors v ON v.Id = l.VisitorId
                WHERE v.Name = @name AND l.ReturnDate IS NULL", conn);
                cmd.Parameters.AddWithValue("@name", visitorName);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result;
        }

        public void ClearAllDebts()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Visitors SET HasDebt = 0 WHERE HasDebt = 1", conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
