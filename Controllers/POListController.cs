using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Exchange.WebServices.Data;
using MyProject.BusinessLogic;
using MyProject.Models;
using System.Threading.Tasks;
//using InventoryBeginners.Areas.Identity.Pages.Account;

namespace InventoryBeginners.Controllers
{
    public class POListController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISQLRepository _sqlRepository;
        private readonly InventoryContext _context;
        private readonly IBrand _Repo;
  
        public POListController(InventoryContext context, UserManager<IdentityUser> userManager, ISQLRepository sqlRepository, IBrand repo)
        {
            _sqlRepository = sqlRepository;
            _userManager = userManager;
            _context = context;
            _Repo = repo;
        }

        public async Task<IActionResult> POList()
        {
            PoHeader model = new PoHeader();

            LedgerFilter LF = new LedgerFilter();
            LF.CommandType = "List";
            model.POListApprove = await new POTransactionBus(_sqlRepository).GetPOData(LF);
            return View("POList", model);
        }

        public IActionResult Index(string sortExpression = "", string SearchText = "", int pg = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("name");
            sortModel.AddColumn("description");
            sortModel.ApplySort(sortExpression);
            ViewData["sortModel"] = sortModel;

            ViewBag.SearchText = SearchText;

            PaginatedList<Brand> items = _Repo.GetItems(sortModel.SortedProperty, sortModel.SortedOrder, SearchText, pg, pageSize);


            var pager = new PagerModel(items.TotalRecords, pg, pageSize);
            pager.SortExpression = sortExpression;
            this.ViewBag.Pager = pager;


            TempData["CurrentPage"] = pg;


            return View(items);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
