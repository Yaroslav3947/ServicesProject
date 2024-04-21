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

        static string connectionString = "server=localhost;uid=root;pwd=Maxym_20045;database=services";

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

        public List<(int clientId, string lastName, decimal? amount)> GetClientPayments() {
            List<(int, string, decimal?)> clientPayments = new List<(int, string, decimal?)>();

            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT 
                                    c.client_id, 
                                    c.last_name, 
                                    p.amount
                                FROM clients c
                                LEFT JOIN payments p USING(client_id)
                                UNION SELECT 
	                                c.client_id, 
                                    c.last_name, 
                                    p.amount
                                FROM clients c
                                RIGHT JOIN payments p USING (client_id)
                                WHERE c.client_id IS NULL";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            int clientId = Convert.ToInt32(reader["client_id"]);
                            string lastName = reader["last_name"].ToString();
                            decimal? amount = reader["amount"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["amount"]);

                            clientPayments.Add((clientId, lastName, amount));
                        }
                    }
                }
            }

            return clientPayments;
        }

        public List<(string eventName, int totalGuests, int? quantity)> GetEventServiceQuantities() {
            List<(string, int, int?)> eventServiceQuantities = new List<(string, int, int?)>();

            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT 
                                    e.event_name,
                                    e.total_guests,
                                    es.quantity
                                FROM events e
                                RIGHT JOIN event_services es ON e.event_id = es.event_id";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            string eventName = reader["event_name"].ToString();
                            int totalGuests = Convert.ToInt32(reader["total_guests"]);
                            int? quantity = reader["quantity"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["quantity"]);

                            eventServiceQuantities.Add((eventName, totalGuests, quantity));
                        }
                    }
                }
            }

            return eventServiceQuantities;
        }

        public List<(int paymentId, decimal? amount, string paymentMethodName)> GetPaymentsWithMethod() {
            List<(int, decimal?, string)> paymentsWithMethod = new List<(int, decimal?, string)>();

            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT 
                                    p.payment_id,
                                    p.amount,
                                    pm.name
                                FROM payments p
                                LEFT JOIN payment_method pm ON p.payment_method_id = pm.payment_method_id";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            int paymentId = reader["payment_id"] == DBNull.Value ? -1 : Convert.ToInt32(reader["payment_id"]);
                            decimal? amount = reader["amount"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["amount"]);
                            string paymentMethodName = reader["name"].ToString();

                            paymentsWithMethod.Add((paymentId, amount, paymentMethodName));
                        }
                    }
                }
            }

            return paymentsWithMethod;
        }

        public List<(int vendorServiceId, string vendorName, string serviceName)> GetVendorServices() {
            List<(int, string, string)> vendorServices = new List<(int, string, string)>();

            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT 
                                    vs.vendor_service_id,
                                    v.vendor_name,
                                    s.service_name
                                FROM vendor_services vs
                                RIGHT JOIN services s USING(service_id)
                                RIGHT JOIN vendors v USING(vendor_id)";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            int vendorServiceId = reader["vendor_service_id"] == DBNull.Value ? -1 : Convert.ToInt32(reader["vendor_service_id"]);
                            string vendorName = reader["vendor_name"].ToString();
                            string serviceName = reader["service_name"].ToString();

                            vendorServices.Add((vendorServiceId, vendorName, serviceName));
                        }
                    }
                }
            }

            return vendorServices;
        }

        public void CreateVendorsStartingWithAView() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"
                    CREATE VIEW VendorsStartingWithA1 AS
                    SELECT * FROM vendors
                    WHERE vendor_name LIKE 'A%';
                ";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateVendorsStartingWithAEmail(int vendorId, string newEmail) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"UPDATE VendorsStartingWithA 
                                 SET email = @newEmail
                                 WHERE vendor_id = @vendorId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@newEmail", newEmail);
                    cmd.Parameters.AddWithValue("@vendorId", vendorId);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetVendorStartingWithAById(int vendorId) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM VendorsStartingWithA WHERE vendor_id = @vendorId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@vendorId", vendorId);

                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Console.WriteLine($"{reader["vendor_id"]}, {reader["vendor_name"]}, {reader["contact_person"]}, {reader["phone"]}, {reader["email"]}");
                        }
                    }
                }
            }
        }

        public void DeleteVendorStartingWithA(int vendorId) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"DELETE FROM VendorsStartingWithA WHERE vendor_id = @vendorId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@vendorId", vendorId);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetVendorsStartingWithA() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM VendorsStartingWithA";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Console.WriteLine($" {reader["vendor_id"]}, {reader["vendor_name"]}, {reader["contact_person"]}, {reader["phone"]}, {reader["email"]}");
                        }
                    }
                }
            }
        }

        public void CreatePaymentsView() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"
                    CREATE VIEW PaymentsView1 AS
                    SELECT * FROM payments
                    WHERE payment_method_id = 5;
                ";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetPaymentsView() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM PaymentsView1";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Console.WriteLine($"{reader["payment_id"]}, {reader["client_id"]}, {reader["amount"]}, {reader["payment_date"]}, Payment Method ID: {reader["payment_method_id"]}");
                        }
                    }
                }
            }
        }

        public void CreateServicesView() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"
                    CREATE VIEW ServicesView2 AS
                    SELECT * FROM services
                    WHERE cost > 2000.0 AND cost < 3500.0;
                ";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetServicesView() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM ServicesView2";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {

                            Console.WriteLine($"{reader["service_id"]}, {reader["service_name"]}, {reader["description"]}, {reader["cost"]}");
                        }
                    }
                }
            }
        }

        public void UpdateServicesView1Cost(int serviceId, decimal newCost) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"UPDATE ServicesView1 
                                 SET cost = @newCost
                                 WHERE service_id = @serviceId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@newCost", newCost);
                    cmd.Parameters.AddWithValue("@serviceId", serviceId);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetServicesView1() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM ServicesView1";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Console.WriteLine($", {reader["service_id"]}, Service Name: {reader["service_name"]}, Description: {reader["description"]}, Cost: {reader["cost"]}");
                        }
                    }
                }
            }
        }

        public void DeleteFromServicesView1(int serviceId) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"DELETE FROM ServicesView1 WHERE service_id = @serviceId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@serviceId", serviceId);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetServicesView1ById(int serviceId) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM ServicesView1 WHERE service_id = @serviceId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@serviceId", serviceId);

                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Console.WriteLine($"{reader["service_id"]}, {reader["service_name"]}, {reader["description"]}, {reader["cost"]}");
                        }
                    }
                }
            }
        }

        public void CreateEventsView() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"
                    CREATE VIEW EventsView1 AS
                    SELECT * FROM events
                    ORDER BY event_name;
                ";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetEventsView() {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM EventsView1";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Console.WriteLine($" {reader["event_id"]}, {reader["client_id"]}, {reader["event_name"]}, {reader["event_date"]}, {reader["venue"]}, {reader["total_guests"]}");
                        }
                    }
                }
            }
        }

        public void UpdateEventsViewVenue(int eventId, string newVenue) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"UPDATE EventsView 
                                 SET venue = @newVenue
                                 WHERE event_id = @eventId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@newVenue", newVenue);
                    cmd.Parameters.AddWithValue("@eventId", eventId);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void GetEventFromViewById(int eventId) {
            using(MySqlConnection connection = new MySqlConnection(connectionString)) {
                string query = @"SELECT * FROM EventsView WHERE event_id = @eventId";

                using(MySqlCommand cmd = new MySqlCommand(query, connection)) {
                    cmd.Parameters.AddWithValue("@eventId", eventId);

                    connection.Open();
                    using(MySqlDataReader reader = cmd.ExecuteReader()) {
                        while(reader.Read()) {
                            Console.WriteLine($"{reader["event_id"]}, {reader["client_id"]}, {reader["event_name"]}, {reader["event_date"]}, {reader["venue"]}, {reader["total_guests"]}");
                        }
                    }
                }
            }
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
                .RuleFor(vs => vs.ServiceId, f => f.Random.Number(1, 137));

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
                .RuleFor(es => es.EventId, f => f.Random.Number(1, 136))
                .RuleFor(es => es.ServiceId, f => f.Random.Number(1, 138))
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
