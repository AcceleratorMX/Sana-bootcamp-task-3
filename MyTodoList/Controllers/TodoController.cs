using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyTodoList.Data.Models;
using MyTodoList.Data.Repository;

namespace MyTodoList.Controllers;

public class TodoController(JobRepository jobRepository, CategoryRepository categoryRepository)
    : Controller
{
    public async Task<IActionResult> Create()
    {
        var categories = await categoryRepository.GetCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(new Job());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IFormCollection collection)
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
            await jobRepository.AddJob(job);
            return RedirectToAction("Todo");
        }

        var categories = await categoryRepository.GetCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", job.CategoryId);
        return View(job);
    }

    public async Task<IActionResult> Todo()
    {
        var jobs = (await jobRepository.GetJobs())
            .OrderByDescending(job => job.IsDone)
            .ThenByDescending(job => job.Id);
        return View(jobs);
    }


    [HttpPost]
    public async Task<IActionResult> ChangeProgress(int id)
    {
        var job = await jobRepository.GetJob(id);
        if (job.IsDone) return RedirectToAction("Todo");
        job.IsDone = true;
        await jobRepository.UpdateJob(job);
        return RedirectToAction("Todo");
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        await jobRepository.DeleteJob(id);
        return RedirectToAction("Todo");
    }
}