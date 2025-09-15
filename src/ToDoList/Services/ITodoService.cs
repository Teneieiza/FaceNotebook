using TodoList.Models;
using TodoList.DTOs;

namespace TodoList.Services;

public interface ITodoService
{
  Task<IEnumerable<Todo>> GetAllAsync();
  Task<Todo?> GetByIdAsync(int id);
  Task<Todo> CreateAsync(CreateTodoRequest request);
  Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request);
  Task<bool> DeleteAsync(int id);
}