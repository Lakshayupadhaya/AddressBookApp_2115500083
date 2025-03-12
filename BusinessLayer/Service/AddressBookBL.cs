using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IMapper _mapper;
        private readonly IAddressBookRL _addressBookRL;
        public AddressBookBL(IMapper mapper, IAddressBookRL addressBookRL) 
        {
            _mapper = mapper;
            _addressBookRL = addressBookRL;
        }

        public List<AddressBookEntity> GetAllContactsBL() 
        {
            return _addressBookRL.GetAllContactsRL();
        }

        public AddressBookDTO GetContactByIDBL(int id)
        {
            AddressBookEntity addressBookEntity = _addressBookRL.GetContactByIDRL(id);

            return _mapper.Map<AddressBookDTO>(addressBookEntity);
        }
        
        public CreateContactDTO AddContactBL(AddressBookDTO createContact)
        {
            AddressBookEntity addressBookEntity = _mapper.Map<AddressBookEntity>(createContact);

            AddressBookEntity createdEntity = _addressBookRL.AddContactRL(addressBookEntity);

            return _mapper.Map<CreateContactDTO>(createdEntity);
        }

        public AddressBookDTO UpdateContactByIDBL(int id, AddressBookDTO updateContact)
        {
            AddressBookEntity addressBookEntity = _mapper.Map<AddressBookEntity>(updateContact);

            AddressBookEntity UpdatedEntity = _addressBookRL.UpdateContactByID(id, addressBookEntity);

            return _mapper.Map<AddressBookDTO>(UpdatedEntity);
        }

        public AddressBookDTO DeleteContactByIDBL(int id) 
        {
            AddressBookEntity deletedEntity = _addressBookRL.DeleteContactByID(id);

            return _mapper.Map<AddressBookDTO>(deletedEntity);


























































































        }
    }
}
