using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
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

        public List<Client> getAllClients() {
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

        public static void InsertRandomClients() {
            var faker = new Faker<Client>()
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Address, f => f.Address.StreetAddress())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.State, f => f.Address.StateAbbr());

            using(MySqlConnection connection = dataSource()) {
                connection.Open();

                const int numberOfClientsToInsert = 1000;

                for(int i = 0;i < numberOfClientsToInsert;i++) {
                    var client = faker.Generate();

                    using(MySqlCommand cmd = new MySqlCommand("INSERT INTO clients (first_name, last_name, phone, email, address, city, state) " +
                                                                "VALUES (@FirstName, @LastName, @Phone, @Email, @Address, @City, @State)", connection)) {
                        cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", client.LastName);
                        cmd.Parameters.AddWithValue("@Phone", client.Phone);
                        cmd.Parameters.AddWithValue("@Email", client.Email);
                        cmd.Parameters.AddWithValue("@Address", client.Address);
                        cmd.Parameters.AddWithValue("@City", client.City);
                        cmd.Parameters.AddWithValue("@State", client.State);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void InsertRandomEvents() {
            var faker = new Faker<Event>()
                .RuleFor(e => e.ClientId, f => f.IndexFaker)
                .RuleFor(e => e.EventName, f => f.Lorem.Word())
                .RuleFor(e => e.EventDate, f => f.Date.Future())
                .RuleFor(e => e.Venue, f => f.Address.City())
                .RuleFor(e => e.TotalGuests, f => f.Random.Number(10, 500));

            using(MySqlConnection connection = ApplicationDbContext.dataSource()) {
                connection.Open();

                for(int i = 0;i < 136;i++) {
                    var newEvent = faker.Generate();

                    using(MySqlCommand cmd = new MySqlCommand("INSERT INTO events (client_id, event_name, event_date, venue, total_guests) " +
                                                                "VALUES (@ClientId, @EventName, @EventDate, @Venue, @TotalGuests)", connection)) {
                        cmd.Parameters.AddWithValue("@ClientId", newEvent.ClientId);
                        cmd.Parameters.AddWithValue("@EventName", newEvent.EventName);
                        cmd.Parameters.AddWithValue("@EventDate", newEvent.EventDate);
                        cmd.Parameters.AddWithValue("@Venue", newEvent.Venue);
                        cmd.Parameters.AddWithValue("@TotalGuests", newEvent.TotalGuests);

                        cmd.ExecuteNonQuery();
                    }
                }
            }



        }

        public static void InsertRandomVendors() {
            var faker = new Faker<Vendor>()
                .RuleFor(v => v.VendorName, f => f.Company.CompanyName())
                .RuleFor(v => v.ContactPerson, f => f.Person.FullName)
                .RuleFor(v => v.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(v => v.Email, f => f.Internet.Email());

            var numberOfVendors = 1000;

            var vendors = faker.Generate(numberOfVendors);

            using(MySqlConnection connection = dataSource()) {
                connection.Open();

                foreach(var vendor in vendors) {
                    using(MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO vendors (vendor_name, contact_person, phone, email) " +
                        "VALUES (@vendorName, @contactPerson, @phone, @email)", connection)) {
                        cmd.Parameters.AddWithValue("@vendorName", vendor.VendorName);
                        cmd.Parameters.AddWithValue("@contactPerson", vendor.ContactPerson);
                        cmd.Parameters.AddWithValue("@phone", vendor.Phone);
                        cmd.Parameters.AddWithValue("@email", vendor.Email);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        public static void InsertRandomPayments() {
            var faker = new Faker<Payment>()
                .RuleFor(p => p.ClientId, f => f.Random.Number(1, 1000))
                .RuleFor(p => p.Amount, f => f.Finance.Amount())
                .RuleFor(p => p.PaymentDate, f => f.Date.Recent())
                .RuleFor(p => p.PaymentMethodId, f => f.Random.Number(1, 22));

            var numberOfPayments = 1000;
            var payments = faker.Generate(numberOfPayments);

            using(MySqlConnection connection = dataSource()) {
                connection.Open();

                foreach(var payment in payments) {
                    using(MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO payments (client_id, amount, payment_date, payment_method_id) " +
                        "VALUES (@clientId, @amount, @paymentDate, @paymentMethodId)", connection)) {
                        cmd.Parameters.AddWithValue("@clientId", payment.ClientId);
                        cmd.Parameters.AddWithValue("@amount", payment.Amount);
                        cmd.Parameters.AddWithValue("@paymentDate", payment.PaymentDate);
                        cmd.Parameters.AddWithValue("@paymentMethodId", payment.PaymentMethodId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        public static void InsertVendorServices() {
            var faker = new Faker<VendorService>()
                .RuleFor(vs => vs.VendorId, f => f.Random.Number(1, 1000))
                .RuleFor(vs => vs.ServiceId, f => f.Random.Number(1, 139));

            var numberOfVendorServices = 1000;
            var vendorServices = faker.Generate(numberOfVendorServices);

            using(MySqlConnection connection = dataSource()) {
                connection.Open();

                foreach(var vendorService in vendorServices) {
                    using(MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO vendor_services (vendor_id, service_id) " +
                        "VALUES (@vendorId, @serviceId)", connection)) {
                        cmd.Parameters.AddWithValue("@vendorId", vendorService.VendorId);
                        cmd.Parameters.AddWithValue("@serviceId", vendorService.ServiceId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void InsertRandomEventServices() {
            var faker = new Faker<EventService>()
                .RuleFor(es => es.EventId, f => f.Random.Number(1, 32))
                .RuleFor(es => es.ServiceId, f => f.Random.Number(1, 139))
                .RuleFor(es => es.Quantity, f => f.Random.Number(1, 10));

            var numberOfEventServices = 1000;
            var eventServices = faker.Generate(numberOfEventServices);

            using(MySqlConnection connection = dataSource()) {
                connection.Open();

                foreach(var eventService in eventServices) {
                    using(MySqlCommand cmd = new MySqlCommand(
                        "INSERT INTO event_services (event_id, service_id, quantity) " +
                        "VALUES (@eventId, @serviceId, @quantity)", connection)) {
                        cmd.Parameters.AddWithValue("@eventId", eventService.EventId);
                        cmd.Parameters.AddWithValue("@serviceId", eventService.ServiceId);
                        cmd.Parameters.AddWithValue("@quantity", eventService.Quantity);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
