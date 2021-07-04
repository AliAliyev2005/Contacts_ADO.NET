using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts_ADO.NET.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Contact() { }
        public Contact(string name, string surname, string phone, string email)
        {
            Name = name;
            Surname = surname;
            Phone = phone;
            Email = email;
        }
        public Contact(int id, string name, string surname, string phone, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Phone = phone;
            Email = email;
        }
    }
}
