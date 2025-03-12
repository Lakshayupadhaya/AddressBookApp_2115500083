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

        List<AddressBookEntity> GetAllContactsBL();

        AddressBookDTO GetContactByIDBL(int id);

        AddressBookDTO UpdateContactByIDBL(int id, AddressBookDTO updateContact);

        CreateContactDTO AddContactBL(AddressBookDTO createContact);


        AddressBookDTO DeleteContactByIDBL(int id);
    }
}
