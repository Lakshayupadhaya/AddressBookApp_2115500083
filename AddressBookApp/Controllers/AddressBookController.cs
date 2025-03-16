using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;

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
        public ActionResult GetAllContacts([FromQuery] string token)
        {
            bool autharised = _addressBookBL.AuthariseToken(token);
            if (autharised)
            {
                (List<AddressBookEntity> entities, bool authorise) = _addressBookBL.GetAllContactsBL(token);
                if (authorise) //Hamdles the slight variation of token expiration
                {
                    if (entities.Count == 0)
                    {
                        Responce<string> notFoundResponce = new Responce<string>();
                        notFoundResponce.Success = false;
                        notFoundResponce.Message = "Fetching failed";
                        notFoundResponce.Data = $"Not Found";
                        return NotFound(notFoundResponce);
                    }
                    Responce<List<AddressBookEntity>> fetchResponce = new Responce<List<AddressBookEntity>>();
                    fetchResponce.Success = true;
                    fetchResponce.Message = "Contacts Fetched SuccessFully";
                    fetchResponce.Data = entities;
                    return Ok(fetchResponce);
                }
            }
            Responce<string> unauthorisedResponce = new Responce<string>();
            unauthorisedResponce.Success = false;
            unauthorisedResponce.Message = "Fetching Request failed";
            unauthorisedResponce.Data = "You are not Authorised";
            return Unauthorized(unauthorisedResponce);
        }

        /// <summary>
        /// Get contact by ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult GetContactById(int id, [FromQuery] string token)
        {
            (bool autharised, bool found) = _addressBookBL.AuthariseToken(token, id);
            if (!found) 
            {
                Responce<string> notFoundResponce = new Responce<string>();
                notFoundResponce.Success = false;
                notFoundResponce.Message = "Fetching failed";
                notFoundResponce.Data = $"ID : {id} Not Found";
                return NotFound(notFoundResponce);
            }
            if (autharised) 
            {
                AddressBookDTO contact = _addressBookBL.GetContactByIDBL(id);
                Responce<AddressBookDTO> getContactResponce = new Responce<AddressBookDTO>();
                getContactResponce.Success = true;
                getContactResponce.Message = "Contact Fetched SuccessFully";
                getContactResponce.Data = contact;
                return Ok(getContactResponce);
            }
            Responce<string> unauthorisedResponce = new Responce<string>();
            unauthorisedResponce.Success = false;
            unauthorisedResponce.Message = "Fetching Request failed";
            unauthorisedResponce.Data = "You are not Authorised";
            return Unauthorized(unauthorisedResponce);  
        }

        /// <summary>
        /// Add a new contact.
        /// </summary>
        [HttpPost]
        //Method to add contact to the database
        public ActionResult AddContact([FromBody] AddressBookDTO addContact, [FromQuery] string token)
        {
            bool autharised = _addressBookBL.AuthariseToken(token);
            if (autharised)
            {
                CreateContactDTO createdContact = _addressBookBL.AddContactBL(addContact, token);
                Responce<CreateContactDTO> addResponce = new Responce<CreateContactDTO>();
                addResponce.Success = true;
                addResponce.Message = "Contact Added SuccessFully";
                addResponce.Data = createdContact;
                return Ok(addResponce);
            }
            Responce<string> unauthorisedResponce = new Responce<string>();
            unauthorisedResponce.Success = false;
            unauthorisedResponce.Message = "Addition Request failed";
            unauthorisedResponce.Data = "You are not Authorised To Add";
            return Unauthorized(unauthorisedResponce);
        }

        /// <summary>
        /// Update an existing contact.
        /// </summary>
        [HttpPut("{id}")]
        //Method to Update Contact in AddressBook By ID
        public ActionResult UpdateContactByID(int id, [FromBody] AddressBookDTO updateConntact, [FromQuery] string token)
        {
            (bool autharised, bool found) = _addressBookBL.AuthariseToken(token, id);
            if (!found) 
            {
                Responce<string> notFoundResponce = new Responce<string>();
                notFoundResponce.Success = false;
                notFoundResponce.Message = "Updation Not Successfull";
                notFoundResponce.Data = $"ID : {id} Not Found";
                return NotFound(notFoundResponce);
            }
            if (autharised) 
            {
                AddressBookDTO UpdatedContact = _addressBookBL.UpdateContactByIDBL(id, updateConntact);
                Responce<AddressBookDTO> updateResponce = new Responce<AddressBookDTO>();
                updateResponce.Success = true;
                updateResponce.Message = "Contact Updated SuccessFully";
                updateResponce.Data = updateConntact;

                return Ok(updateResponce);
            }
            Responce<string> unauthorisedResponce = new Responce<string>();
            unauthorisedResponce.Success = false;
            unauthorisedResponce.Message = "Updation Request failed";
            unauthorisedResponce.Data = "You are not Authorised To Update";
            return Unauthorized(unauthorisedResponce);
        }

        /// <summary>
        /// Delete a contact by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeleteContactByID(int id, [FromQuery] string token)
        {
            (bool autharised, bool found) = _addressBookBL.AuthariseToken(token, id);
            if (!found)
            {
                Responce<string> notFoundResponce = new Responce<string>();
                notFoundResponce.Success = false;
                notFoundResponce.Message = "Deletetion Not Successfull";
                notFoundResponce.Data = $"ID : {id} Not Found";
                return NotFound(notFoundResponce);
            }
            if (autharised)
            {
                AddressBookDTO deletedContact = _addressBookBL.DeleteContactByIDBL(id);
                Responce<AddressBookDTO> deleteResponce = new Responce<AddressBookDTO>();
                deleteResponce.Success = true;
                deleteResponce.Message = "Contact Deleted SuccessFully";
                deleteResponce.Data = deletedContact;
                return Ok(deleteResponce);
            }
            Responce<string> unauthorisedResponce = new Responce<string>();
            unauthorisedResponce.Success = false;
            unauthorisedResponce.Message = "Deletion Request failed";
            unauthorisedResponce.Data = "You are not Authorised To Delete";
            return Unauthorized(unauthorisedResponce);
        }


    }
}
