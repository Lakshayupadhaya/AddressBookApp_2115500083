using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;

namespace AddressBookApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        /// <summary>
        /// Fetch all contacts.
        /// </summary>
        [HttpGet]
        public ActionResult GetAll()
        {
            return Ok();
        }

        /// <summary>
        /// Get contact by ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            return Ok();
        }

        /// <summary>
        /// Add a new contact.
        /// </summary>
        [HttpPost]
        public ActionResult AddContact([FromBody] AddressBookDTO addContact)
        {

            return Ok();
        }

        /// <summary>
        /// Update an existing contact.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateContactByID(int id, [FromBody] AddressBookDTO updateConntactByID)
        {

            return Ok();
        }

        /// <summary>
        /// Delete a contact by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }


    }
}
