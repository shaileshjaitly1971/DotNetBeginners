using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Exchange.WebServices.Data;
using MyProject.BusinessLogic;
using MyProject.Models;
using System.Threading.Tasks;
//using InventoryBeginners.Areas.Identity.Pages.Account;

namespace InventoryBeginners.Controllers
{
    public class POApporveController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISQLRepository _sqlRepository;
        private readonly InventoryContext _context;


        public POApporveController(InventoryContext context, UserManager<IdentityUser> userManager, ISQLRepository sqlRepository)
        {
            _sqlRepository = sqlRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> POApproved()
        {
            PoHeader model = new PoHeader();

            LedgerFilter LF = new LedgerFilter();
            LF.CommandType = "List";
            model.POListApprove = await new POTransactionBus(_sqlRepository).GetPOData(LF);
            return View("POApproved", model);
        }

        [HttpPost]
        public async Task<IActionResult> SavePOApproved(PoHeader model)
        {
            string currentUser = _userManager.GetUserName(User);

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            ServiceResponse servrespo = new ServiceResponse();
            try
            {
                var CreatedBy = currentUser;
                servrespo.Status = await new POTransactionBus(_sqlRepository).Update_POApprove(model,CreatedBy);

                servrespo.Message = "Approved Successfully.";
                servrespo.Status = 1;
            }//try
            catch (Exception ex)
            {
                servrespo.Message = "Internal server Error.";
            }//exception catch
            return RedirectToAction("POApproved", "POApporve");

        }/// SavePOApproved

        [HttpPost]
        public async Task<IActionResult> CancelPOApproved(PoHeader model)
        {
            string currentUser = _userManager.GetUserName(User);
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            ServiceResponse servrespo = new ServiceResponse();
            try
            {
                var CreatedBy = currentUser;
                servrespo.Status = await new POTransactionBus(_sqlRepository).Update_VoucherCancel(model,CreatedBy);

                servrespo.Message = "Cancel Successfully.";
                servrespo.Status = 1;
            }//try
            catch (Exception ex)
            {
                servrespo.Message = "Internal server Error.";
            }//exception catch
            return RedirectToAction("POApproved", "POApporve");

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
