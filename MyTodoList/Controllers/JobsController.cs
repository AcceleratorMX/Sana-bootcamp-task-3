using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyTodoList.Data.Models;
using MyTodoList.Data.Repository;

namespace MyTodoList.Controllers;

public class JobsController : Controller
{
    private readonly JobRepository _jobRepository;
    private readonly CategoryRepository _categoryRepository;

    public JobsController(JobRepository jobRepository, CategoryRepository categoryRepository)
    {
        _jobRepository = jobRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryRepository.GetCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(new Job());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormCollection collection)
    {
        var job = new Job
        {
            Name = collection["Name"].ToString(),
            IsDone = false
        };

        if (string.IsNullOrEmpty(job.Name))
        {
            ModelState.AddModelError("Name", "Введіть назву завдання!");
        }

        if (int.TryParse(collection["CategoryId"], out int categoryId))
        {
            job.CategoryId = categoryId;
        }
        else
        {
            ModelState.AddModelError("CategoryId", "Оберіть категорію!");
        }

        if (ModelState.IsValid)
        {
            await _jobRepository.AddJob(job);
            return RedirectToAction("Todo");
        }

        var categories = await _categoryRepository.GetCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", job.CategoryId);
        return View(job);
    }

    public async Task<IActionResult> Todo()
    {
        var jobs = (await _jobRepository.GetJobs())
            .OrderByDescending(job => job.IsDone)
            .ThenByDescending(job => job.Id);
        return View(jobs);
    }


    [HttpPost]
    public async Task<IActionResult> Todo(int id)
    {
        var job = await _jobRepository.GetJob(id);
        if (job.IsDone) return RedirectToAction("Todo");
        job.IsDone = true;
        await _jobRepository.UpdateJob(job);
        return RedirectToAction("Todo");
    }
}