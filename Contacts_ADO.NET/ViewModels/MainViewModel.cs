using Contacts_ADO.NET.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Contacts_ADO.NET.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }
        private string surname;
        public string Surname
        {
            get { return surname; }
            set { surname = value; OnPropertyChanged(); }
        }
        private string phone;
        public string Phone
        {
            get { return phone; }
            set { phone = value; OnPropertyChanged(); }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Contact> contacts;
        public ObservableCollection<Contact> Contacts
        {
            get { return contacts; }
            set { contacts = value; OnPropertyChanged(); }
        }

        public RelayCommand GetContactsCommand { get; set; }
        public RelayCommand AddContactCommand { get; set; }
        public RelayCommand<DataGrid> DeleteContactCommand { get; set; }
        public RelayCommand<DataGrid> UpdateContactCommand { get; set; }

        public MainViewModel()
        {
            GetContactsCommand = new RelayCommand(GetContacts);
            AddContactCommand = new RelayCommand(AddContacts);
            DeleteContactCommand = new RelayCommand<DataGrid>(DeleteContact);
            UpdateContactCommand = new RelayCommand<DataGrid>(UpdateContact);
        }

        private bool Check()
        {
            Regex regexEmail = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"); // Email regex
            Regex regexPhone = new Regex(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$"); // Phone regex
            if (Name == null) MessageBox.Show("Name can't be emtpy !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (Surname == null) MessageBox.Show("Surname can't be emtpy !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (Email == null) MessageBox.Show("Email can't be emtpy !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (!regexEmail.IsMatch(Email)) MessageBox.Show("Email is not valid !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (Phone == null) MessageBox.Show("Phone can't be emtpy !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (!regexPhone.IsMatch(Phone)) MessageBox.Show("Phone is not valid !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else return true;
            return false;
        }

        public void GetContacts()
        {
            Contacts = new ObservableCollection<Contact>();
            var connectionString = ConfigurationManager.ConnectionStrings["ContactsDatabase"].ConnectionString;
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM Contacts";
            var data = command.ExecuteReader();

            while (data.Read())
            {
                Contacts.Add(new Contact(data.GetInt32(0), data.GetString(1), data.GetString(2), data.GetString(3), data.GetString(4)));
            }
        }

        public void AddContacts()
        {
            if(Check())
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ContactsDatabase"].ConnectionString;
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"INSERT INTO Contacts (Name,Surname,Phone,Email) VALUES ('{Name}','{Surname}','{Phone}','{Email}')";
                command.ExecuteNonQuery();
                MessageBox.Show("Contact successfully added to list !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                GetContacts();
            }
        }

        public void DeleteContact(DataGrid dg)
        {
            if (dg.SelectedValue != null)
            {
                Contact contact = dg.SelectedValue as Contact;

                var connectionString = ConfigurationManager.ConnectionStrings["ContactsDatabase"].ConnectionString;
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"DELETE FROM Contacts WHERE Id = {contact.Id}";
                command.ExecuteNonQuery();
                MessageBox.Show("Contact successfully deleted from list !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                GetContacts();
            }
            else MessageBox.Show("You must select contact !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void UpdateContact(DataGrid dg)
        {
            if(dg.SelectedValue == null) MessageBox.Show("You must select contact !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (Check())
            {
                Contact contact = dg.SelectedValue as Contact;

                var connectionString = ConfigurationManager.ConnectionStrings["ContactsDatabase"].ConnectionString;
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"UPDATE Contacts SET Name = '{Name}', Surname = '{Surname}', Phone = '{Phone}', Email = '{Email}' WHERE Id = {contact.Id}";
                command.ExecuteNonQuery();
                MessageBox.Show("Contact successfully updated !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                GetContacts();
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
