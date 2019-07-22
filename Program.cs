using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Ado.net
{

    public class User
    {

        public User()
        {
            Hires = new List<Hire>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public List<Hire> Hires { get; set; }
    }

    public class Hire
    {
        public Book Book { get; set; }
        public User User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Id { get; internal set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISBN { get; set; }
        public bool IsActive { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string _connectionString = "Server=.; Database=Kütüphane; Trusted_Connection=True;";

            try
            {
                //First Level Cache
                
                List<User> users = new List<User>();
                List<Book> books = new List<Book>();
                List<Hire> hires = new List<Hire>();

                List<Hire> result = new List<Hire>();

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    var komut = new SqlCommand();
                    SqlCommand cmd = new SqlCommand(@"
                        select 
                        h.Id hireId,
                        u.Id userId,
                        b.Name bookName,
                        b.Id bookId,
                        u.Name userName,
                        * 
                        from Hire h
                        inner join Users u on h.UserId = u.Id
                        inner join Books b on h.BookId = b.Id
                        ", con);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        var bookId = int.Parse(reader["bookId"].ToString());
                        var book = books.FirstOrDefault(x => x.Id == bookId);
                        if (book == null)
                        {
                            book = new Book();

                            book.Id = bookId;
                            book.Name = reader["bookName"].ToString();
                            book.ISBN = reader["ISBN"].ToString();
                            //book.IsActive = int.Parse(reader["IsActive"].ToString()) == 1;
                            books.Add(book);
                        }

                        var userId = int.Parse(reader["userId"].ToString());
                        var user = users.FirstOrDefault(x => x.Id == userId);
                        if (user == null)
                        {
                            user = new User()
                            {
                                Id = userId,
                                Name = reader["userName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Adress = reader["Adress"].ToString(),
                            };
                            users.Add(user);
                        }
                        var hire = new Hire()
                        {
                            Id = int.Parse(reader["hireId"].ToString()),
                            User = user,
                            Book = book,
                            DeliveryDate = string.IsNullOrEmpty(reader["DeliveryDate"].ToString())  ? default(DateTime) : DateTime.Parse(reader["DeliveryDate"].ToString()),
                            StartDate = DateTime.Parse(reader["StartDate"].ToString()),
                            EndDate = DateTime.Parse(reader["EndDate"].ToString())
                        };
                        hires.Add(hire);
                        user.Hires.Add(hire);

                        result.Add(hire);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


