using TodoList.Models;
using TodoList.DTOs;

namespace TodoList.Services;

public class TodoService : ITodoService
{
  private readonly List<Todo> _todos = new()
  {
    new(1, "เรียน .NET Core", false),
    new(2, "สร้าง API", true)
  };

public Task<IEnumerable<Todo>> GetAllAsync() {
    return Task.FromResult(_todos.AsEnumerable());
  }

  public Task<Todo?> GetByIdAsync(int id) {
    var todo = _todos.FirstOrDefault(t => t.Id == id);
    return Task.FromResult(todo);
  }

  public Task<Todo> CreateAsync(CreateTodoRequest request) {
    var newTodo = new Todo(_todos.Count + 1, request.Title, false);
    _todos.Add(newTodo);
    return Task.FromResult(newTodo);
  }

  public Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request) {
    var todo = _todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Task.FromResult<Todo?>(null);

    var updatedTodo = todo with { Title = request.Title, IsCompleted = request.IsCompleted };
    _todos[_todos.FindIndex(t => t.Id == id)] = updatedTodo;
    return Task.FromResult<Todo?>(updatedTodo);
  }

  public Task<bool> DeleteAsync(int id) {
    var todo = _todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Task.FromResult(false);

    _todos.Remove(todo);
    return Task.FromResult(true);
  }
}