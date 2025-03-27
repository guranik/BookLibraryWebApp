using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryBusinessLogicClassLibrary.DTOs.Authentication
{
    public class RefreshTokenModel
    {
        public required string RefreshToken { get; set; }
    }
}
