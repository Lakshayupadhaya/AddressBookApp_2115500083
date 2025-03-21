﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {

        List<AddressBookEntity> GetAllContactsRL(string role, int userId);

        AddressBookEntity GetContactByIDRL(int id);

        AddressBookEntity UpdateContactByID(int id, AddressBookEntity addressBookEntity);

        AddressBookEntity AddContactRL(AddressBookEntity addressBookEntity);

        AddressBookEntity DeleteContactByID(int id);

        (bool authorised, bool found) AuthoriseAndFindRL(int userId, int id);
    }
}
