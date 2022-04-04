using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIrsttime.model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FIrsttime.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IConfiguration Configuration;

        private string connStr;

        public TestController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            connStr = this.Configuration.GetConnectionString("Default");
        }

        [HttpGet("all")] //Get Persons
        public ActionResult GetAllPersons()
        {
            List<Person> persons = new List<Person>();
            string query = "SELECT * FROM Persons";
            MySqlDataReader reader;
            using (MySqlConnection mycon = new MySqlConnection(this.connStr))
            {
                mycon.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, mycon))
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Person person = new Person();
                        person.id = (int)reader["PersonID"];
                        person.LastName = reader["LastName"].ToString();
                        person.FirstName = reader["FirstName"].ToString();
                        person.Address = reader["Address"].ToString();
                        person.City = reader["City"].ToString();


                        persons.Add(person);
                    }
                    reader.Close();
                    mycon.Close();
                }
                return Ok(persons);
            }
        }

        [HttpPost("add")]
        public ActionResult CreatePerson([FromBody] Person body)
        {
            string query = @"INSERT INTO Persons(PersonID,LastName,FirstName,Address,City) VALUES(@id, @last, @first, @add, @city)";
            MySqlDataReader myreader;
            using (MySqlConnection mycon = new MySqlConnection(this.connStr))
            {
                mycon.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, mycon))
                {
                    cmd.Parameters.AddWithValue("@id", (int)(body.id));
                    cmd.Parameters.AddWithValue("@last", body.LastName.ToString());
                    cmd.Parameters.AddWithValue("@first", body.FirstName.ToString());
                    cmd.Parameters.AddWithValue("@add", body.Address.ToString());
                    cmd.Parameters.AddWithValue("@city", body.City.ToString());

                    myreader = cmd.ExecuteReader();

                    myreader.Close();
                    mycon.Close();
                }
            }
            return Ok("Create Pass!");

        }
        [HttpPut("{personId}/update")]
        public ActionResult UpdatePerson(int personId, [FromBody] Person body)
        {
            string query = @"UPDATE Persons SET LastName=@last, FirstName=@first, Address=@add, City=@city WHERE PersonID=@id";
            MySqlDataReader reader;
            using (MySqlConnection mycon = new MySqlConnection(this.connStr))
            {
                mycon.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, mycon))
                {
                    cmd.Parameters.AddWithValue("@id", personId);
                    cmd.Parameters.AddWithValue("@last", body.LastName);
                    cmd.Parameters.AddWithValue("@first", body.FirstName);
                    cmd.Parameters.AddWithValue("@add", body.Address);
                    cmd.Parameters.AddWithValue("@city", body.City);

                    reader = cmd.ExecuteReader();

                    reader.Close();
                    mycon.Close();

                }
                return Ok("Update Pass!");
            }
        }

        [HttpDelete("{personId}")]
        public ActionResult DeletePerson(int personId)
        {
            string query = @"DELETE FROM Persons WHERE PersonID=@id";
            MySqlDataReader reader;

            using (MySqlConnection mycon = new MySqlConnection(this.connStr))
            {
                mycon.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, mycon))
                {
                    cmd.Parameters.AddWithValue("@id", personId);

                    reader = cmd.ExecuteReader();

                    reader.Close();
                    mycon.Close();
                }
            }
            return Ok("Delete Pass!");
        }
    }
}
