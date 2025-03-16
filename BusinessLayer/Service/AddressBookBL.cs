using System;
using System.Collections.Generic;
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
        private readonly IRedisCacheService _redisCache;

        public AddressBookBL(IMapper mapper, IAddressBookRL addressBookRL, Jwt jwt, IRedisCacheService redisCache)
        {
            _mapper = mapper;
            _addressBookRL = addressBookRL;
            _jwt = jwt;
            _redisCache = redisCache;
        }

        // ✅ GET ALL CONTACTS with Redis Caching
        public (List<AddressBookEntity>, bool authorised) GetAllContactsBL(string token)
        {
            var result = _jwt.GetRoleAndUserId(token);
            if (result == null)
            {
                return (new List<AddressBookEntity>(), false);
            }

            (string role, int userId) = result.Value;
            string cacheKey = $"Contacts_{userId}";

            // Check Redis Cache first
            var cachedData = _redisCache.GetData<List<AddressBookEntity>>(cacheKey);
            if (cachedData != null)
            {
                return (cachedData, true);
            }

            // If not found in cache, fetch from DB
            var contacts = _addressBookRL.GetAllContactsRL(role, userId);

            // Store in Redis with expiration time of 10 minutes
            _redisCache.SetData(cacheKey, contacts, TimeSpan.FromMinutes(10));

            return (contacts, true);
        }

        // ✅ GET CONTACT BY ID with Redis Caching
        public AddressBookDTO GetContactByIDBL(int id)
        {
            string cacheKey = $"Contact_{id}";

            // Check if data exists in cache
            var cachedData = _redisCache.GetData<AddressBookEntity>(cacheKey);
            if (cachedData != null)
            {
                return _mapper.Map<AddressBookDTO>(cachedData);
            }

            // If cache is empty, fetch from database
            AddressBookEntity addressBookEntity = _addressBookRL.GetContactByIDRL(id);

            // Store in Redis with expiration time of 10 minutes
            _redisCache.SetData(cacheKey, addressBookEntity, TimeSpan.FromMinutes(10));

            return _mapper.Map<AddressBookDTO>(addressBookEntity);
        }

        // ✅ ADD CONTACT: Clears Cache on Insert
        public CreateContactDTO AddContactBL(AddressBookDTO createContact, string token)
        {
            AddressBookEntity addressBookEntity = _mapper.Map<AddressBookEntity>(createContact);

            var userClaims = _jwt.GetClaimsFromToken(token);
            int userId = _jwt.GetUserIdFromClaims(userClaims);
            addressBookEntity.UserId = userId;

            AddressBookEntity createdEntity = _addressBookRL.AddContactRL(addressBookEntity);

            // Remove old cache for this user
            _redisCache.RemoveData($"Contacts_{userId}");

            return _mapper.Map<CreateContactDTO>(createdEntity);
        }

        // ✅ UPDATE CONTACT: Clears Cache on Update
        public AddressBookDTO UpdateContactByIDBL(int id, AddressBookDTO updateContact)
        {
            AddressBookEntity addressBookEntity = _mapper.Map<AddressBookEntity>(updateContact);

            AddressBookEntity updatedEntity = _addressBookRL.UpdateContactByID(id, addressBookEntity);

            // Remove old cache
            _redisCache.RemoveData($"Contacts_{updatedEntity.UserId}");
            _redisCache.RemoveData($"Contact_{id}");

            return _mapper.Map<AddressBookDTO>(updatedEntity);
        }

        // ✅ DELETE CONTACT: Clears Cache on Delete
        public AddressBookDTO DeleteContactByIDBL(int id)
        {
            AddressBookEntity deletedEntity = _addressBookRL.DeleteContactByID(id);

            // Remove cache
            _redisCache.RemoveData($"Contacts_{deletedEntity.UserId}");
            _redisCache.RemoveData($"Contact_{id}");

            return _mapper.Map<AddressBookDTO>(deletedEntity);
        }

        // ✅ TOKEN AUTHORIZATION
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
