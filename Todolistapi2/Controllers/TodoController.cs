using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todolistapi2.Models;

namespace Todolistapi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly AppDb Db;
        public TodoController(AppDb db)
        {
            Db = db;
        }

        [HttpGet("get-all-list")]
        public async Task<IActionResult> GetLatest()
        {
            await Db.Connection.OpenAsync();
            var query = new TodoQuery(Db);
            var result = await query.LatestPostsAsync();
            return new OkObjectResult(result);
        }

        [HttpGet("get-todo-by-id/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new TodoQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        [HttpPost("add-to-do-list")]
        public async Task<IActionResult> Post([FromBody] Todo body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            await body.InsertAsync();
            return new OkObjectResult(body);
        }

        [HttpPut("update-todo-by-id/{id}")]
        public async Task<IActionResult> PutOne(int id, [FromBody] Todo body)
        {
            await Db.Connection.OpenAsync();
            var query = new TodoQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.Status = body.Status;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE api/blog/5
        [HttpDelete("delete-todo-id/{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new TodoQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkResult();
        }

        // DELETE api/blog
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll()
        {
            await Db.Connection.OpenAsync();
            var query = new TodoQuery(Db);
            await query.DeleteAllAsync();
            return new OkResult();
        }
    }
}
