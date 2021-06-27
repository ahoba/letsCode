using Forcebook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class CreateRebelResponse : RebelDTO
    {
        public CreateRebelResponse(Rebel rebel) : base(rebel)
        {

        }
    }
}
