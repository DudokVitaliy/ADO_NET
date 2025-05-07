using System;
using System.Data;
using System.Data.SqlClient;

namespace HospitalApp
{
    internal class Program
    {
        static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Hospital;Integrated Security=True";

        static void GetTotalCapacityByDepartment(string departmentName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT SUM(Capacity) 
                         FROM Wards w
                         JOIN Departments d ON w.DepartmentId = d.Id
                         WHERE d.Name = @depName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@depName", departmentName);

                object result = cmd.ExecuteScalar();
                Console.WriteLine($"Total number of seats in the department '{departmentName}': {result ?? 0}");
            }
        }
        static void GetAllExaminations()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT e.Id, p.Name + ' ' + p.Surname AS Patient,
                                d.Name + ' ' + d.Surname AS Doctor,
                                w.Name AS Ward, e.Date
                         FROM Examinations e
                         JOIN Patients p ON e.PatientId = p.Id
                         JOIN Doctors d ON e.DoctorId = d.Id
                         JOIN Wards w ON e.WardId = w.Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Id"]}: {reader["Patient"]}, {reader["Doctor"]}, {reader["Ward"]}, {reader["Date"]}");
                }
            }
        }
        static void DeleteExaminationsBefore(DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Examinations WHERE Date < @date";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", date);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"Deleted {rows} examination to {date:yyyy-MM-dd}");
            }
        }
        static void GetDoctorsWithSalaryAbove(decimal amount)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Name, Surname, Salary FROM Doctors WHERE Salary > @amount";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", amount);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Name"]} {reader["Surname"]} - {reader["Salary"]}");
                }
            }
        }
        static void GetMaxDonation()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT TOP 1 s.Name AS SponsorName, d.Amount
            FROM Donations d
            JOIN Sponsors s ON d.SponsorId = s.Id
            ORDER BY d.Amount DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader["SponsorName"].ToString();
                        decimal amount = (decimal)reader["Amount"];
                        Console.WriteLine($"Max donation: {amount} UAH — from {name}");
                    }
                    else
                    {
                        Console.WriteLine("Don't have any donations.");
                    }
                }
            }
        }


        static void AddExamination(int patientId, int doctorId, int wardId, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Examinations (PatientId, DoctorId, WardId, Date)
                         VALUES (@pId, @dId, @wId, @date)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@pId", patientId);
                cmd.Parameters.AddWithValue("@dId", doctorId);
                cmd.Parameters.AddWithValue("@wId", wardId);
                cmd.Parameters.AddWithValue("@date", date);

                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"Added examination: {rows} records");
            }
        }
        static void DeleteInactiveSponsors()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            DELETE FROM Sponsors
            WHERE Id NOT IN (SELECT DISTINCT SponsorId FROM Donations)";
                SqlCommand cmd = new SqlCommand(query, conn);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"Deleted {rows} inactive sponsors");
            }
        }


        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            GetTotalCapacityByDepartment("Кардіологія");
            GetAllExaminations();
            DeleteExaminationsBefore(new DateTime(2024, 5, 3));
            GetDoctorsWithSalaryAbove(16000);
            GetMaxDonation();
            AddExamination(1, 2, 3, DateTime.Now);
            DeleteInactiveSponsors();

        }
    }
}
