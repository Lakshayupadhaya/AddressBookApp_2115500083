using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IMapper _mapper;
        private readonly IAddressBookRL _addressBookRL;
        private readonly Jwt _jwt;
        public AddressBookBL(IMapper mapper, IAddressBookRL addressBookRL, Jwt jwt) 
        {
            _mapper = mapper;
            _addressBookRL = addressBookRL;
            _jwt = jwt;
        }

        public (List<AddressBookEntity>, bool authorised) GetAllContactsBL(string token) 
        {
            var result = _jwt.GetRoleAndUserId(token);
            if(result == null) 
            {
                return (new List<AddressBookEntity>(), false);
            }
            (string role, int userId) = result.Value;

            return (_addressBookRL.GetAllContactsRL(role, userId), true);
        }

        public AddressBookDTO GetContactByIDBL(int id)
        {
            AddressBookEntity addressBookEntity = _addressBookRL.GetContactByIDRL(id);

            return _mapper.Map<AddressBookDTO>(addressBookEntity);
        }
        
        public CreateContactDTO AddContactBL(AddressBookDTO createContact, string token)
        {
            AddressBookEntity addressBookEntity = _mapper.Map<AddressBookEntity>(createContact);

            var userClaims = _jwt.GetClaimsFromToken(token);
            int userId = _jwt.GetUserIdFromClaims(userClaims);
            addressBookEntity.UserId = userId;

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

        public bool AuthariseToken(string token) 
        {
            return _jwt.ValidateToken(token); 
        }

        public (bool authorised, bool found) AuthariseToken(string token, int id)
        {
            return _jwt.ValidateToken(token, id);
        }
    }
}
