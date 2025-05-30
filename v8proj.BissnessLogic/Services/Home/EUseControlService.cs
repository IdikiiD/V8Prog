using System.Collections.Generic;
using System.Linq;
using v8proj.BissnessLogic.Interfaces.Home;
using v8proj.Core.Entities; 
using v8proj.DAL;
using v8proj.Web.Model; 

namespace v8proj.BissnessLogic.Services.Home 
{
    public class EUseControlService : IEUseControlService
    {
        private readonly ApplicationDbContext _context;

        public EUseControlService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public List<eUseControl> GetAllCars()
        {
            return _context.eUseControl.ToList();
        }
    }
}