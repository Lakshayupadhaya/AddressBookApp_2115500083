using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {

        (List<AddressBookEntity>, bool authorised) GetAllContactsBL(string token);

        AddressBookDTO GetContactByIDBL(int id);

        AddressBookDTO UpdateContactByIDBL(int id, AddressBookDTO updateContact);

        CreateContactDTO AddContactBL(AddressBookDTO createContact, string token);

        AddressBookDTO DeleteContactByIDBL(int id);

        bool AuthariseToken(string token);

        (bool authorised, bool found) AuthariseToken(string token, int id);
    }
}
