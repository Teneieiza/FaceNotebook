using Microsoft.AspNetCore.Mvc;
using TodoList.Services;
using TodoList.DTOs;
using TodoList.Models;

namespace TodoList.Controllers;

[ApiController]
[Route("api/todo")]

public class TodoController : ControllerBase
{
  private readonly ITodoService _todoService;

  public TodoController(ITodoService todoService)
  {
    _todoService = todoService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
  {
    var todos = await _todoService.GetAllAsync();
    return Ok(todos);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Todo>> GetTodo(int id)
  {
    var todo = await _todoService.GetByIdAsync(id);
    return todo is not null ? Ok(todo) : NotFound();
  }

  [HttpPost]
  public async Task<ActionResult<Todo>> CreateTodo(CreateTodoRequest request)
  {
    var todo = await _todoService.CreateAsync(request);
    return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Todo>> UpdateTodo(int id, UpdateTodoRequest request)
  {
    var todo = await _todoService.UpdateAsync(id, request);
    return todo is not null ? Ok(todo) : NotFound();
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteTodo(int id)
  {
    var success = await _todoService.DeleteAsync(id);
    
      if (!success)
        return NotFound(new { message = $"Todo with id {id} not found." });

      return Ok(new { message = $"Todo with id {id} has been deleted." });
  }
}