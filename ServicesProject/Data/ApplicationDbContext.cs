using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ServicesProject.Models.Domain;

namespace ServicesProject.Data {
    public class ApplicationDbContext {
        public static MySqlConnection connection = new MySqlConnection();

        static string connectionString = "server=localhost;uid=root;pwd=yaroslav2005;database=services";

        public static MySqlConnection dataSource() {
            connection = new MySqlConnection(connectionString);
            return connection;
        }

        public void connOpen() {
            dataSource();
            connection.Open();
        }

        public void connClose() {
            dataSource();
            connection.Close();
        }

        public List<Client> GetAllClients() {
            List<Client> clients = new List<Client>();

            using(MySqlConnection connection = dataSource()) {
                using(MySqlCommand cmd = new MySqlCommand("SELECT * FROM clients", connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Client client = new Client {
                                ClientId = Convert.ToInt32(reader["client_id"]),
                                FirstName = reader["first_name"].ToString(),
                                LastName = reader["last_name"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Email = reader["email"].ToString(),
                                Address = reader["address"].ToString(),
                                City = reader["city"].ToString(),
                                State = reader["state"].ToString()
                            };
                            clients.Add(client);
                        }
                    }
                }
            }

            return clients;
        }

    }
}
