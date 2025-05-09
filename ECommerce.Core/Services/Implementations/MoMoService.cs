using AutoMapper;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Momo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class MoMoService : BaseService<IMoMoService>, IMoMoService
    {
        public MoMoService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<IMoMoService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public string CreatePayment(MoMoRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
