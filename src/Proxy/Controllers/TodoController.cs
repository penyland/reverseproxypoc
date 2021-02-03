using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ReverseProxyPOC.Proxy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private static readonly TodoItem[] TodoItems = new[]
        {
            new TodoItem { Id = 1, IsComplete = false, Name = "Todo1" },
            new TodoItem { Id = 2, IsComplete = false, Name = "Todo2" },
            new TodoItem { Id = 3, IsComplete = false, Name = "Todo3" }
        };

        [HttpGet]
        public IEnumerable<TodoItem> GetTodoItems()
        {
            return TodoItems;
        }

        [HttpGet("{id}")]
        public TodoItem GetTodoItem(long id)
        {
            return TodoItems.Single(t => t.Id == id);
        }
    }
}
