using System.Collections.Generic;
using v8proj.Core.Entities;
using v8proj.Web.Model; 

namespace v8proj.BissnessLogic.Interfaces.Home 
{
    public interface IEUseControlService
    {
        List<eUseControl> GetAllCars();
    }
}