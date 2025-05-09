using System.Linq;
using System.Web.Mvc;
using v8proj.Core.Model.ViewModels; // Измените пространство имен на v8proj.Core.Model.ViewModels
using v8proj.DAL;

public class ProductDescriptionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductDescriptionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public ActionResult Details(int id)
    {
        var car = _context.eUseControls.Find(id); // Используйте eUseControls

        if (car == null)
        {
            return HttpNotFound();
        }

        return View(car);
    }
}