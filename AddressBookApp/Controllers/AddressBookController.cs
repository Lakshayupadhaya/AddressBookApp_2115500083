using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace AddressBookApp.Controllers
{
    [ApiController]
    [Route("api/AddressBook")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookBL;
        public AddressBookController(IAddressBookBL addressBookBL) 
        {
            _addressBookBL = addressBookBL;
        }
        /// <summary>
        /// Fetch all contacts.
        /// </summary>
        [HttpGet]
        public ActionResult GetAllContacts()
        {
            List<AddressBookEntity> entities = _addressBookBL.GetAllContactsBL();
            Responce<List<AddressBookEntity>> fetchResponce = new Responce<List<AddressBookEntity>>();
            fetchResponce.Success = true;
            fetchResponce.Message = "Contacts Fetched SuccessFully";
            fetchResponce.Data = entities;
            return Ok(fetchResponce);
        }

        /// <summary>
        /// Get contact by ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult GetContactById(int id)
        {
            AddressBookDTO contact = _addressBookBL.GetContactByIDBL(id);
            Responce<AddressBookDTO> getContactResponce = new Responce<AddressBookDTO>();
            getContactResponce.Success = true;
            getContactResponce.Message = "Contact Fetched SuccessFully";
            getContactResponce.Data = contact;
            return Ok(getContactResponce);
        }

        /// <summary>
        /// Add a new contact.
        /// </summary>
        [HttpPost]
        //Method to add contact to the database
        public ActionResult AddContact([FromBody] AddressBookDTO addContact)
        {
            CreateContactDTO createdContact = _addressBookBL.AddContactBL(addContact);
            Responce<CreateContactDTO> addResponce = new Responce<CreateContactDTO>();
            addResponce.Success = true;
            addResponce.Message = "Contact Added SuccessFully";
            addResponce.Data = createdContact;
            return Ok(addResponce);
        }

        /// <summary>
        /// Update an existing contact.
        /// </summary>
        [HttpPut("{id}")]
        //Method to Update Contact in AddressBook By ID
        public ActionResult UpdateContactByID(int id, [FromBody] AddressBookDTO updateConntact)
        {
            AddressBookDTO UpdatedContact = _addressBookBL.UpdateContactByIDBL(id, updateConntact);
            Responce<AddressBookDTO> updateResponce = new Responce<AddressBookDTO>();
            updateResponce.Success = true;
            updateResponce.Message = "Contact Updated SuccessFully";
            updateResponce.Data = updateConntact;

            return Ok(updateResponce);

        }

        /// <summary>
        /// Delete a contact by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeleteContactByID(int id)
        {
            AddressBookDTO deletedContact = _addressBookBL.DeleteContactByIDBL(id);
            Responce<AddressBookDTO> deleteResponce = new Responce<AddressBookDTO>();
            deleteResponce.Success = true;
            deleteResponce.Message = "Contact Updated SuccessFully";
            deleteResponce.Data = deletedContact;
            return Ok(deleteResponce);
        }


    }
}
