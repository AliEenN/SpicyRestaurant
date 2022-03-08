﻿namespace SpicyRestaurant.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;

        public SubCategoriesController(IUnitOfWork unitOfWork, IMapper mapper, IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<IndexSubCategoryViewModel>>(await _unitOfWork.SubCategories.GetAllAsync(e => e.Name, new[] { "Category" })));
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new SubCategoryFormViewModel() { CategoriesList = await _unitOfWork.Categories.GetAllAsync(e => e.Name) });
            else
            {
                var result = await _unitOfWork.SubCategories.GetByObjectIdAsync(id);

                if (result == null)
                    return NotFound();

                return View(_mapper.Map<SubCategoryFormViewModel>(result));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(int id, SubCategoryFormViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, nameof(CreateOrEdit), model) });

            if (id == 0)
            {
                if (await _unitOfWork.SubCategories.FindAsync(e => e.Name == model.Name) != null)
                {
                    ModelState.AddModelError("Name", "This Sub Category Is Already Exist!");
                    return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, nameof(CreateOrEdit), model) });
                }

                await _unitOfWork.SubCategories.AddAsync(_mapper.Map<SubCategory>(model));
                _unitOfWork.Complete();
            }
            else
            {
                if (await _unitOfWork.SubCategories.FindAsync(e => e.Name == model.Name && e.Id != model.Id) != null)
                {
                    ModelState.AddModelError("Name", "This Sub Category Is Already Exist");
                    return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, nameof(CreateOrEdit), model) });
                }

                _unitOfWork.SubCategories.Edit(_mapper.Map<SubCategory>(model));
                _unitOfWork.Complete();
            }

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _mapper.Map<IEnumerable<IndexSubCategoryViewModel>>(await _unitOfWork.SubCategories.GetAllAsync(e => e.Name))) });
        }

        //public async Task<IActionResult> Delete(byte? id)
        //{
        //    if (id == null)
        //        return BadRequest();

        //    var category = await _unitOfWork.Categories.GetByObjectIdAsync(id);

        //    if (category == null)
        //        return NotFound();

        //    _unitOfWork.Categories.Delete(category);
        //    _unitOfWork.Complete();

        //    return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _mapper.Map<IEnumerable<CategoryViewModel>>(await _unitOfWork.Categories.GetAllAsync(e => e.Name))) });
        //}

        //public async Task<IActionResult> DeleteAll(IEnumerable<string> ID)
        //{
        //    ViewBag.msg = string.Empty;

        //    try
        //    {
        //        List<string> idsList = ID.ToList();
        //        List<Category> categoriesList = new List<Category>();

        //        if (idsList.Count > 0)
        //        {
        //            foreach (var id in idsList)
        //            {
        //                int deleteId = 0;
        //                try
        //                {
        //                    deleteId = int.Parse(id);
        //                }
        //                catch { }

        //                if (deleteId > 0)
        //                {
        //                    var category = await _unitOfWork.Categories.FindAsync(e => e.Id == deleteId);
        //                    if (category != null)
        //                    {
        //                        categoriesList.Add(category);
        //                    }
        //                }
        //            }

        //            _unitOfWork.Categories.DeleteRange(categoriesList);
        //            _unitOfWork.Complete();
        //        }
        //        else
        //        {
        //            ViewBag.msg = "This Category Has One Or More Sub Category Under Him You Must Delete It First";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.msg = ex.Message;
        //    }

        //    _toastNotification.AddSuccessToastMessage("Category(s) Deleted Successfly.");

        //    return RedirectToAction(nameof(Index));
        //}

        //public async Task<IActionResult> DownloadExcel()
        //{
        //    var categoriesList = await _unitOfWork.Categories.GetAllAsync(e => e.Name);

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Categories");
        //        var currentRow = 1;

        //        #region Header
        //        worksheet.Cell(currentRow, 1).Value = "Category Id";
        //        worksheet.Cell(currentRow, 2).Value = "Category Name";
        //        #endregion

        //        #region Body
        //        foreach (var category in categoriesList)
        //        {
        //            currentRow++;
        //            worksheet.Cell(currentRow, 1).Value = category.Id;
        //            worksheet.Cell(currentRow, 2).Value = category.Name;
        //        }
        //        #endregion

        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();

        //            return File(
        //                content,
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                "Categories.xlsx"
        //            );
        //        }
        //    }
        //}

        // GET
        public async Task<IActionResult> GetSubCategories(int id)
        {
            var subCategories = await _unitOfWork.SubCategories.GetAllAsync(e => e.CategoryId == id, e => e.Name);

            return Json(new SelectList(subCategories, "Id", "Name"));
        }
    }
}
