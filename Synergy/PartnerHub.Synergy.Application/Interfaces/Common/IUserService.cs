using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Interfaces.Common
{
    public interface IUserService
    {
        Guid CurrentUserId { get; }
    }
}
