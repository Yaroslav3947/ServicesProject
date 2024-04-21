using ServicesProject.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServicesProject {
    public partial class Form1 : Form {
        Data.ApplicationDbContext _dbContext = new Data.ApplicationDbContext();
        public Form1() {
            InitializeComponent();
        }

        private void connectBnt_Click(object sender, EventArgs e) {
            try {
                Data.ApplicationDbContext.dataSource();
                _dbContext.connOpen();
                successlbl.Text = "Success";



                _dbContext.UpdateEventsViewVenue(34, "NewVenue");

                _dbContext.GetEventFromViewById(34);

                _dbContext.connClose();
            }
            catch(Exception) {
                successlbl.Text = "Failed";
                _dbContext.connClose();
            }
            finally {

            }
        }

        private void getClientBtn_Click(object sender, EventArgs e) {
            List<Client> clients = _dbContext.getAllClients();

            if(clients.Count > 0) {
                foreach(Client client in clients) {
                    Console.WriteLine($"Client ID: {client.ClientId}, Name: {client.FirstName} {client.LastName}");
                    Console.WriteLine($"Phone: {client.Phone}, Email: {client.Email}");
                    Console.WriteLine($"Address: {client.Address}, City: {client.City}, State: {client.State}\n");
                }
            } else {
                Console.WriteLine("No clients found.");
            }
        }
    }
}
