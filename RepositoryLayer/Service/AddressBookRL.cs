using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
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
    }
}
