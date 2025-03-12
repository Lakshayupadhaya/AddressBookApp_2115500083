using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly ApplicationDBContext _dbContext;
        public AddressBookRL(ApplicationDBContext dbContext) 
        {
            _dbContext = dbContext;
        }


        public List<AddressBookEntity> GetAllContactsRL() 
        {
            var Entities = _dbContext.AddressBook.AsNoTracking().ToList();
            return Entities;
        }


        public AddressBookEntity GetContactByIDRL(int id) 
        {
            var addressBookEntity = _dbContext.AddressBook.FirstOrDefault(a =>  a.Id == id);

            return addressBookEntity;
        }


        public AddressBookEntity AddContactRL(AddressBookEntity addressBookEntity) 
        {
            _dbContext.AddressBook.Add(addressBookEntity);
            _dbContext.SaveChanges();
            return addressBookEntity;

        }

        public AddressBookEntity UpdateContactByID(int id, AddressBookEntity addressBookEntity) 
        {
            var entity = _dbContext.AddressBook.FirstOrDefault(a => a.Id == id);
            if(entity == null) 
            {
                return entity;
            }   
            entity.Address = addressBookEntity.Address;
            entity.PhoneNumber = addressBookEntity.PhoneNumber;
            entity.Email = addressBookEntity.Email;
            entity.Name = addressBookEntity.Name;
            _dbContext.SaveChanges();
            return entity;
        }

        public AddressBookEntity DeleteContactByID(int id) 
        {
            var entity = _dbContext.AddressBook.FirstOrDefault(_a => _a.Id == id);
            if (entity == null) 
            {
                return null;
            }
            _dbContext.AddressBook.Remove(entity);
            _dbContext.SaveChanges();
            return entity;
        }
    }
}
